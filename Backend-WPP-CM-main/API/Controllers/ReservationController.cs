using System.Globalization;
using DomainServices;
using DonainModel;
using DonainModel.dto;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Infrastructure;

namespace API.Controllers {
    [Authorize]
    [Route("api/reservation")]
    [ApiController]
    public class ReservationController : ControllerBase {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IReservationRepository _reservationRepository;
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;

        public ReservationController(UserManager<IdentityUser> userManager, IReservationRepository reservationRepository, IUserRepository userRepository, IEmailService emailService) {
            _reservationRepository = reservationRepository;
            _userRepository = userRepository;
            _userManager = userManager;
            _emailService = emailService;
        }

        [HttpGet]
        [Produces("application/json")]
        public ActionResult Get([FromQuery] string? userId, [FromQuery] string? carId, [FromQuery] string? beginDate, [FromQuery] string? endDate) {
            //Check params
            CheckGetQueryParams(ModelState, userId, carId, beginDate, endDate);

            if (!ModelState.IsValid){
                return BadRequest(new {
                    statusCode = 400,
                    message = ModelState.Values.SelectMany(v => v.Errors).ToArray()[0].ErrorMessage,
                    data = new { }
                });
            }

            int? parsedUserId = null;
            if (userId != null) {
                parsedUserId = int.Parse(userId);
            }

            int? parsedCarId = null;
            if (carId != null) {
                parsedCarId = int.Parse(carId);
            }

            DateTime? parsedBeginDate = null;
            if(beginDate != null) {
                parsedBeginDate = DateTime.Parse(beginDate);
            }

            DateTime? parsedEndDate = null;
            if (endDate != null) {
                parsedEndDate = DateTime.Parse(endDate);
            }

            var reservations = _reservationRepository.GetAll(parsedUserId, parsedCarId, parsedBeginDate, parsedEndDate);

            List<Object> returnReservations = new();
            foreach (var reservation in reservations) {
                returnReservations.Add(formatReservation(reservation));
            }

            return Ok(new {
                statusCode = 200,
                message = GetAllReturnMessage(userId, carId),
                data = returnReservations
            });
        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult GetOne(int id) {
            var reservation = _reservationRepository.GetById(id);

            if (reservation == null) {
                return NotFound(new {
                    StatusCode = 404,
                    Message = $"Reservation with id {id} not found",
                    Data = new { }
                });
            }

            Object returnObject = formatReservation(reservation);

            return Ok(new {
                StatusCode = 200,
                Message = $"Successfully retrieved Reservation with id: {id}",
                data = returnObject
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ReservationDto reservation) {
            //return Dto errors
            CheckDtoDateFormat(ModelState, reservation);
            if (!ModelState.IsValid) {
                return BadRequest(new {
                    statusCode = 400,
                    message = GetModelStateError(ModelState),
                    data = new { },
                });
            }

    
            //find user
            var identityUser = await _userManager.FindByIdAsync(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var user = _userRepository.GetBySecurityId(identityUser.Id);

            if (user == null) return NotFound(new {
                StatusCode = 404,
                Message = "Logged in user does not exist",
                Data= new { }
            });


            //Catch exception and sent back in bad request
            Reservation? addedReservation = null;
            try {
                addedReservation = await _reservationRepository.Create(reservation, user.id);
            } catch (Exception ex) {
                if (ex.Message == ReservationRepository.internalServerErrorException) {
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

            Object returnObject = formatReservation(addedReservation!);

            return Ok(new {
                statusCode = 200,
                message = "Successfully added reservation",
                data = returnObject
            });
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete(int id) {
            
            try {
                //Send email to the owner and all people who made requests
                var emails = new List<string>();
                
                var reservation = _reservationRepository.GetById(id);

                if (reservation == null) {
                    return NotFound(new {
                        statusCode = 400,
                        message = "Reservation cannot be found",
                        data = new { },
                    });
                }

                emails.Add(reservation.user.emailAddress);

                foreach (var request in reservation.requests) {
                    emails.Add(request.user.emailAddress);
                }

                if (emails.Count > 0) {
                    await _emailService.SendDeletionEmailAsync(emails.ToArray(), reservation.user.name,
                        reservation.destination, reservation.departureDate.ToString(CultureInfo.CurrentCulture));
                }

                await _reservationRepository.Delete(id);
            } catch (Exception ex) {
                return BadRequest(new {
                    statusCode = 400,
                    message = ex.Message,
                    data = new { },
                });
            }
            
            

            return Ok(new {
                statusCode = 200,
                message = "Succesvully deleted reservation and connected requests",
                data = new { },
            });
        }

        private ModelStateDictionary CheckGetQueryParams(
            ModelStateDictionary modelState, 
            string? userId, 
            string? carId, 
            string? beginDate, 
            string? endDate) {

            //check userId
            if (userId != null) {
                bool succes = int.TryParse(userId, out int outParse);
                if (!succes) {
                    modelState.AddModelError("userId", "userId needs to be an integer");
                }
            }

            //check carId
            if (carId != null) {
                bool succes = int.TryParse(carId, out int outParse);
                if (!succes) {
                    modelState.AddModelError("carId", "carId needs to be an integer");
                }
            }

            //check beginDate
            if (beginDate != null) {
                bool succes = DateTime.TryParse(beginDate, out DateTime outParse);
                if (!succes) {
                    modelState.AddModelError("beginDate", "beginDate needs to DateTime formatted");
                }
            }

            //check endDate
            if (endDate != null) {
                bool succes = DateTime.TryParse(endDate, out DateTime outParse);
                if (!succes) {
                    modelState.AddModelError("endDate", "endDate needs to DateTime formatted");
                }
            }

            return modelState;
        }

        private ModelStateDictionary CheckDtoDateFormat(ModelStateDictionary modelState, ReservationDto reservation) {
            //if already invalid return
            if (!modelState.IsValid) {
                return modelState;
            }
            
            //departureDate 
            var result = DateTime.TryParse(reservation.departureDate, out DateTime departureDate);
            if (!result) modelState.AddModelError("departureDate", "departureDate needs to be formatted as a DateTime object");

            //arrivalDate
            result = DateTime.TryParse(reservation.arrivalDate, out DateTime arivalDate);
            if (!result) modelState.AddModelError("departureDate", "arrivalDate needs to be formatted as a DateTime object");

            return modelState;
        }

        private string GetModelStateError(ModelStateDictionary modelState) {
            //get all DtoErrors
            IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);

            //check if seats is given as a string
            if (allErrors.Where(error => error.ErrorMessage.Contains("The JSON value could not be converted to System.Int32")).Count() == 1) {
                return "Seats, userId and carId need to be an integers";
            }

            //else return the error message
            return allErrors.ToArray()[0].ErrorMessage;
        }

        private string GetAllReturnMessage(string? userId, string? carId) {
            string message = "Successfully retrieved all reservations";

            //userId message
            if(userId != null) {
                message += $". Filter userId: {userId}";
            }

            //carId message
            if (carId != null) {
                message += $". Filter carId: {carId}";
            }

            return message;
        }

        private Object formatReservation(Reservation reservation) {
            return new {
                id = reservation.id,
                departureDate = reservation.departureDate,
                arrivalDate = reservation.arrivalDate,
                destination = reservation.destination,
                seats = reservation.seats,
                creationDate = reservation.creationDate,
                car = new {
                    id = reservation.car?.id,
                    licensePlate = reservation.car?.licensePlate,
                    brand = reservation.car?.brand,
                    model = reservation.car?.model,
                    seats = reservation.car?.seats,
                    imageUrl = reservation.car?.imageUrl,
                    location = reservation.car?.location,
                    isAvailable = reservation.car?.isAvailable,
                },
                user = new {
                    id = reservation.user?.id,
                    name = reservation.user?.name,
                    emailAddress = reservation.user?.emailAddress,
                    employeeNr = reservation.user?.employeeNr,
                    hasLicense = reservation.user?.hasLicense,
                    role = reservation.user?.role.ToString(),
                },
            };
        }
    }
}
