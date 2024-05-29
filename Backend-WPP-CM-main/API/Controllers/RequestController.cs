using DomainServices;
using DonainModel;
using DonainModel.dto;
using Infrastructure;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Security.Claims;
using API.Services;

namespace API.Controllers {
    [Authorize]
    [Route("api/request")]
    [Tags("Requests")]
    [ApiController]
    public class RequestController : ControllerBase {
        private readonly IRequestRepository _requestRepository;
        private readonly IUserRepository _userRepository;
        private readonly IReservationRepository _reservationRepository;
        private readonly IConfiguration _configuration;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailService _emailService;

        public RequestController(IRequestRepository requestRepository, UserManager<IdentityUser> userManager, IUserRepository userRepository, IConfiguration configuration, IReservationRepository reservationRepository, IEmailService emailService) {
            _requestRepository = requestRepository;
            _userManager = userManager;
            _userRepository = userRepository;
            _configuration = configuration;
            _reservationRepository = reservationRepository;
            _emailService = emailService;
        }

        [HttpGet]
        public IActionResult GetAll(
            [FromQuery] string? reservationId,
            [FromQuery] string? requestStatus,
            [FromQuery] string? creationDateStart,
            [FromQuery] string? creationDateEnd,
            [FromQuery] string? userId) {

            //check query
            CheckGetQueryParams(ModelState, reservationId, requestStatus, creationDateStart, creationDateEnd, userId);

            if (!ModelState.IsValid) {
                return BadRequest(new {
                    statusCode = 400,
                    message = ModelState.Values.SelectMany(v => v.Errors).ToArray()[0].ErrorMessage,
                    data = new { }
                });
            }

            //parse query
            int? parsedReservationId = null;
            if (reservationId != null) {
                parsedReservationId = int.Parse(reservationId);
            }

            int? parsedUserId = null;
            if (userId != null) {
                parsedUserId = int.Parse(userId);
            }

            Status? parsedRequestStatus = null;
            if (requestStatus != null) {
                bool succes = Status.TryParse(requestStatus, out Status outParse);
                if(succes) {
                    parsedRequestStatus = outParse;
                }
            }

            DateTime? parsedCreationDateStart = null;
            if (creationDateStart != null) {
                parsedCreationDateStart = DateTime.Parse(creationDateStart);
            }

            DateTime? parsedCreationDateEnd = null;
            if (creationDateEnd != null) {
                parsedCreationDateEnd = DateTime.Parse(creationDateEnd);
            }

            //get all cars with query
            var requests = _requestRepository.GetAll(
                reservationId: parsedReservationId, 
                requestStatus: parsedRequestStatus,
                creationDateStart: parsedCreationDateStart,
                creationDateEnd: parsedCreationDateEnd,
                userId: parsedUserId);

            List<Object> returnRequests = new();
            foreach (var request in requests) {
                returnRequests.Add(formatRequest(request));
            }

            return Ok(new {
                statusCode = 200,
                message = "Successfully retrieved all requests",
                data = returnRequests
            });
        }
        
        //Method to get all requests other users have made on any reservations the user has created
        [HttpGet]
        [Route("user")]
        public IActionResult GetAllPersonalRequests() {
            //Get all reservations the user has created
            var identityUser = _userManager.FindByIdAsync(User.FindFirst(ClaimTypes.NameIdentifier)?.Value).Result;
            var user = _userRepository.GetBySecurityId(identityUser.Id);
            var reservations = _reservationRepository.GetAll(userId: user.id);

            if (!reservations.Any()) {

                return Ok(new {
                    StatusCode = 200,
                    Message = $"User with id {user.id} has not created any reservations",
                    data = new List<Request>()
                }) ;
               
            }
            
            //Get all requests on those reservations
            List<Request> requests = new();
            foreach (var reservation in reservations) {
                var reservationRequests = _requestRepository.GetAll(reservationId: reservation.id);
                foreach (var request in reservationRequests) {
                    requests.Add(request);
                }
            }
            
            if (!requests.Any()) {
                return Ok(new {
                    StatusCode = 200,
                    Message = $"User with id {user.id} has not created any reservations",
                    data = new List<Request>()
                });
            }
            
            //Format requests
            List<Object> returnRequests = new();
            foreach (var request in requests) {
                returnRequests.Add(formatRequestWithToken(request));
            }
            
            if (!returnRequests.Any()) {
                return StatusCode(StatusCodes.Status500InternalServerError, new {
                    StatusCode = 500,
                    Message = "Internal Server Error",
                    Data = new { }
                });
            }
            
            return Ok(new {
                StatusCode = 200,
                Message = "Successfully retrieved all requests",
                data = returnRequests
            });
        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult GetOne(int id) {
            var request = _requestRepository.GetById(id);

            if (request == null) {
                return NotFound(new {
                    StatusCode = 404,
                    Message = $"Request with id {id} not found",
                    Data = new { }
                });
            }

            return Ok(new {
                StatusCode = 200,
                Message = $"Successfully retrieved request with id: {id}",
                data = formatRequest(request)
            });
        }
        
        
        


        //Token is a verify string to check if right user has done action,
        //this is generated when request is created
        //Action is DENIED or ACCEPTED
        [HttpGet]
        [AllowAnonymous]
        [Route("{id}/verify")]
        public async Task<IActionResult> verifyRequest(int id, [FromQuery] string? token, [FromQuery] string? status) {
            //check status
            bool succes = Status.TryParse(status, out Status parsedStatus);
            if (!succes) {
                return BadRequest(new {
                    StatusCode = 400,
                    Message = "Status needs to be either ACCEPTED or DENIED",
                    Data = new { }
                });
            }

            //check token
            if(token == null) {
                return Unauthorized(new {
                    StatusCode = 401,
                    Message = "User is not allowed to verify request",
                    Data = new { }
                });
            }

            Request request;
            try {
                request = await _requestRepository.VerifyRequest(id, token, parsedStatus);
            } catch (Exception ex) {
                //check if the exception is that of wrong verify token
                if(ex.Message == RequestRepository.wrongVerifyToken) {
                    return Unauthorized(new {
                        StatusCode = 401,
                        Message = "User is not allowed to veriry request",
                        Data = new { }
                    });
                }

                return BadRequest(new {
                    StatusCode = 400,
                    Message = ex.Message,
                    Data = new { }
                });
            }

            string message;
            if(request.status == Status.ACCEPTED) {
                message = "Request has been accepted";
            } else {
                message = "Request has been denied";
            }
            
            var reservation = _reservationRepository.GetById(request.reservation!.id);
            
            await _emailService.SendRequestStatusAsync(request.user.emailAddress, reservation!.user.name, request.status, reservation.destination, reservation.departureDate.ToString());
            
            return Ok(new {
                StatusCode = 200,
                Message = message,
                Data = new { }
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RequestDto request) {
            var identityUser = await _userManager.FindByIdAsync(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var user = _userRepository.GetBySecurityId(identityUser.Id);

            if (user == null) return NotFound(new {
                StatusCode = 404,
                Message = "User not found",
                Data = new { }
            });

            //return Dto errors
            if (!ModelState.IsValid) {
                return BadRequest(new {
                    statusCode = 400,
                    message = GetModelStateError(ModelState),
                    data = new { },
                });
            }

            Request addedRequest;
            try {
                addedRequest = await _requestRepository.Create(request, user.id);
            } catch (Exception ex) {
                if(ex.Message == RequestRepository.internalServerErrorException) {
                    return StatusCode(StatusCodes.Status500InternalServerError, new {
                        statusCode = 500,
                        message = ex.Message,
                        data = new { },
                    });
                }

                return BadRequest(new {
                    statusCode = 400,
                    message = ex.Message,
                    data = new { }
                });
            }
            
            var reservation = _reservationRepository.GetById(request.reservationId);
            
            await _emailService.SendRequestAsync(user.name, reservation!.user.emailAddress, reservation.destination, reservation.departureDate.ToString(), addedRequest.id.ToString(), addedRequest.token!);
            
            return Ok(new {
                statusCode = 200,
                message = "Successfully added reservation",
                data = formatRequest(addedRequest)
            });
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete(int id) {
            try {
                await _requestRepository.Delete(id);
            } catch (Exception ex) {
                return BadRequest(new {
                    statusCode = 400,
                    message = ex.Message,
                    data = new { },
                });
            }
            return Ok(new {
                statusCode = 200,
                message = "Succesvully deleted request",
                data = new { },
            });
        }

        private ModelStateDictionary CheckGetQueryParams(
            ModelStateDictionary modelState,
            string? reservationId,
            string? requestStatus,
            string? creationDateStart,
            string? creationDateEnd,
            string? userId) {
            //check reservationId
            if (reservationId != null) {
                bool succes = int.TryParse(reservationId, out int outParse);
                if (!succes) {
                    modelState.AddModelError("reservationId", "reservationId needs to be an integer");
                }
            }

            //check userId
            if (userId != null) {
                bool succes = int.TryParse(userId, out int outParse);
                if (!succes) {
                    modelState.AddModelError("userId", "userId needs to be an integer");
                }
            }

            //check requestStatus
            if (requestStatus != null) {
                bool succes = Status.TryParse(requestStatus, out Status outParse);
                if (!succes) {
                    modelState.AddModelError("requestStatus", "requestStatus needs to be either PENDING, ACCEPTED or DENIED");
                }
            }

            //check creationDateStart
            if (creationDateStart != null) {
                bool succes = DateTime.TryParse(creationDateStart, out DateTime outParse);
                if (!succes) {
                    modelState.AddModelError("creationDateStart", "creationDateStart needs to DateTime formatted");
                }
            }

            //check creationDateEnd
            if (creationDateEnd != null) {
                bool succes = DateTime.TryParse(creationDateEnd, out DateTime outParse);
                if (!succes) {
                    modelState.AddModelError("creationDateEnd", "creationDateEnd needs to DateTime formatted");
                }
            }

            return modelState;
        }


        private string GetModelStateError(ModelStateDictionary modelState) {
            //get all DtoErrors
            IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);

            //check if seats is given as a string
            if (allErrors.Where(error => error.ErrorMessage.Contains("The JSON value could not be converted to System.Int32")).Count() == 1) {
                return "Seats and reservationId need to be an integers";
            }

            //else return the error message
            return allErrors.ToArray()[0].ErrorMessage;
        }

        private Object formatRequest(Request request) {
            return new {
                id = request.id,
                seats = request.seats,
                status = request.status.ToString(),
                creationDate = request.creationDate,
                reservation = new {
                    id = request.reservation?.id,
                    departureDate = request.reservation?.departureDate,
                    arrivalDate = request.reservation?.arrivalDate,
                    destination = request.reservation?.destination,
                    seats = request.reservation?.seats,
                    creationDate = request.reservation?.creationDate,
                    car = new {
                        id = request.reservation?.car?.id,
                        licensePlate = request.reservation?.car?.licensePlate,
                        brand = request.reservation?.car?.brand,
                        model = request.reservation?.car?.model,
                        seats = request.reservation?.car?.seats,
                        imageUrl = request.reservation?.car?.imageUrl,
                        location = request.reservation?.car?.location,
                        isAvailable = request.reservation?.car?.isAvailable,
                    },
                },
                user = new {
                    id = request.user?.id,
                    name = request.user?.name,
                    emailAddress = request.user?.emailAddress,
                    employeeNr = request.user?.employeeNr,
                    hasLicense = request.user?.hasLicense,
                    role = request.user?.role.ToString(),
                },
            };
        }
        
        private Object formatRequestWithToken(Request request) {
            return new {
                id = request.id,
                seats = request.seats,
                status = request.status.ToString(),
                creationDate = request.creationDate,
                reservation = new {
                    id = request.reservation?.id,
                    departureDate = request.reservation?.departureDate,
                    arrivalDate = request.reservation?.arrivalDate,
                    destination = request.reservation?.destination,
                    seats = request.reservation?.seats,
                    creationDate = request.reservation?.creationDate,
                    car = new {
                        id = request.reservation?.car?.id,
                        licensePlate = request.reservation?.car?.licensePlate,
                        brand = request.reservation?.car?.brand,
                        model = request.reservation?.car?.model,
                        seats = request.reservation?.car?.seats,
                        imageUrl = request.reservation?.car?.imageUrl,
                        location = request.reservation?.car?.location,
                        isAvailable = request.reservation?.car?.isAvailable,
                    },
                },
                user = new {
                    id = request.user?.id,
                    name = request.user?.name,
                    emailAddress = request.user?.emailAddress,
                    employeeNr = request.user?.employeeNr,
                    hasLicense = request.user?.hasLicense,
                    role = request.user?.role.ToString(),
                },
                token = request.token,
            };
        }
    }

}
