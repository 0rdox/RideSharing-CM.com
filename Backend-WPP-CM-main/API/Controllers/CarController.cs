using DomainServices;
using DonainModel;
using DonainModel.dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Azure.Core;

namespace API.Controllers {
    [Authorize]
    [Route("api/car")]
    [Tags("Cars")]
    [ApiController]
    public class CarController : ControllerBase {
        private readonly ICarRepository _carRepository;

        public CarController(ICarRepository carRepository) {
            _carRepository = carRepository;
        }

        [HttpGet]
        public IActionResult GetAll(
            [FromQuery] string? departureDate, 
            [FromQuery] string? destination,
            [FromQuery] string? numberOfSeats) {

            //if departureDate exists tryParse
            DateTime? departureDateParsed = null;
            if (departureDate != null) {
                bool succes = DateTime.TryParse(departureDate, out DateTime outParse);
                if (succes) departureDateParsed = outParse;
                else {
                    return BadRequest(new {
                        statusCode = 400,
                        message = "departureDate needs to be dateTime formatted",
                        data = new { }
                    });
                }
            }

            //if numberOfSeats exists tryParse
            int? seats = null;
            if(numberOfSeats != null) {
                bool succes = int.TryParse(numberOfSeats, out int outParse);
                if (succes) seats = outParse;
                else {
                    return BadRequest(new {
                        statusCode = 400,
                        message = "seats needs to be an integer",
                        data = new { }
                    });
                }
            }

            var cars = _carRepository.GetAll(departureDateParsed, destination, seats);

            List<Object> returnCars = new();
            foreach (var car in cars) {
                returnCars.Add(new {
                    id = car.id,
                    licensePlate = car.licensePlate,
                    brand = car.brand,
                    model = car.model,
                    seats = car.seats,
                    imageUrl = car.imageUrl,
                    location = car.location,
                    isAvailable = car.isAvailable,
                });
            }

            return Ok(new {
                statusCode = 200,
                message = "Successfully retrieved all cars",
                data = returnCars
            });
        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult GetOne(int id) {
            var car = _carRepository.GetById(id);
            
            if(car == null) {
                return NotFound(new {
                    StatusCode = 404,
                    Message = $"Car with id {id} not found",
                    Data = new { }
                });
            }
            
            return Ok(new {
                StatusCode = 200,
                Message = $"Successfully retrieved car with id: {id}",
                data = new {
                    id = car.id,
                    licensePlate = car.licensePlate,
                    brand = car.brand,
                    model = car.model,
                    seats = car.seats,
                    imageUrl = car.imageUrl,
                    location = car.location,
                    isAvailable = car.isAvailable,
                }
            });
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Create(CarDto car) {
            //Check if dto has errors
            if (!ModelState.IsValid) {
                return BadRequest(new {
                    statusCode = 400,
                    message = GetModelStateError(ModelState),
                    data = new { },
                });
            }

            //Check if car validations are corrrect
            Car newCar;
            try {
                newCar = new Car {
                    licensePlate = car.licensePlate,
                    brand = car.brand,
                    model = car.model,
                    seats = car.seats,
                    imageUrl = car.imageUrl,
                    location = car.location,
                    isAvailable = car.isAvailable,
                };
            } catch (Exception ex) {
                return BadRequest(new {
                    statusCode = 400,
                    message = ex.Message,
                    data = new { },
                });
            }

            //If exception when adding to db dan duplicate license plate
            Car? resultCar;
            try {
                resultCar = await _carRepository.Create(newCar);
            } catch (Exception ex) {
                return Conflict(new {
                    statusCode = 409,
                    message = ex.Message,
                    data = new {}
                });
            }
            
            //If result equals null than system error
            if (resultCar == null) {
                return StatusCode(StatusCodes.Status500InternalServerError, new {
                    statusCode = 500,
                    message = "Internal server error",
                    data = new { },
                });
            }

            //not null and no exception and than succes
            return Ok(new {
                statusCode = 200,
                message = "Successfully created car",
                data = new {
                    id = resultCar.id,
                    licensePlate = resultCar.licensePlate,
                    brand = resultCar.brand,
                    model = resultCar.model,
                    seats = resultCar.seats,
                    imageUrl = resultCar.imageUrl,
                    location = resultCar.location,
                    isAvailable = resultCar.isAvailable,
                }
            });
        }

        [HttpPut]
        [Route("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Update(int id, [FromBody] CarDto car) {
            //Check if dto has errors
            if (!ModelState.IsValid) {
                return BadRequest(new {
                    statusCode = 400,
                    message = GetModelStateError(ModelState),
                    data = new { },
                });
            }

            //Check if car validations are corrrect
            Car newCar;
            try {
                newCar = new Car {
                    licensePlate = car.licensePlate,
                    brand = car.brand,
                    model = car.model,
                    seats = car.seats,
                    imageUrl = car.imageUrl,
                    location = car.location,
                    isAvailable = car.isAvailable,
                };
            } catch (Exception ex) {
                return BadRequest(new {
                    statusCode = 400,
                    message = ex.Message,
                    data = new { },
                });
            }

            //If exception when adding to db dan duplicate license plate
            Car resultCar;
            try {
                resultCar = await _carRepository.Update(id, newCar);
            } catch (Exception ex) {
                return Conflict(new {
                    statusCode = 409,
                    message = ex.Message,
                    data = new { }
                });
            }

            //If result equals null than user not found
            if (resultCar == null) {
                return NotFound(new {
                    statusCode = 404,
                    message = $"Car with id {id} cannot be found",
                    data = new { },
                });
            }

            //not null and no exception and than succes
            return Ok(new {
                statusCode = 200,
                message = "Successfully updated car",
                data = new {
                    id = resultCar.id,
                    licensePlate = resultCar.licensePlate,
                    brand = resultCar.brand,
                    model = resultCar.model,
                    seats = resultCar.seats,
                    imageUrl = resultCar.imageUrl,
                    location = resultCar.location,
                    isAvailable = resultCar.isAvailable,
                }
            });
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Delete(int id) {
            if (_carRepository.GetById(id) == null) {
                return NotFound(new {
                    statusCode = 404,
                    message = $"Car with id {id} cannot be found",
                    data = new { },
                });
            }

            await _carRepository.Delete(id);

            if (_carRepository.GetById(id) != null) {
                return StatusCode(StatusCodes.Status500InternalServerError, new {
                    statusCode = 500,
                    message = $"Internal server error",
                    data = new { },
                });
                
            }
            return Ok(new {
                statusCode = 200,
                message = $"Successfully deleted car with id {id}",
                data = new { },
            });
        }
        private string GetModelStateError(ModelStateDictionary modelState) {
            //get all DtoErrors
            IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);

            //check if seats is given as a string
            if (allErrors.Where(error => error.ErrorMessage.Contains("The JSON value could not be converted to System.Int32")).Count() == 1) {
                return "Seats needs to be an integer";
            }

            //else return the error message
            return allErrors.ToArray()[0].ErrorMessage;
        }

    }
}
