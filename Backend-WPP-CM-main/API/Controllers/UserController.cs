using Azure.Core;
using DomainServices;
using DonainModel;
using DonainModel.dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace API.Controllers; 

[Authorize]
[Route("api/user")]
[Tags("User")]
[ApiController]
public class UserController : ControllerBase {
    private readonly IUserRepository _userRepository;
    
    public UserController(IUserRepository userRepository) {
        _userRepository = userRepository;
    }
    
    [HttpGet]
    public IActionResult GetAll() {
        var users = _userRepository.GetAll();

        List<Object> returnUsers = new();
        foreach (var user in users) {
            returnUsers.Add(new {
                id = user.id,
                name = user.name,
                emailAddress = user.emailAddress,
                employeeNr = user.employeeNr,
                hasLicense = user.hasLicense,
                role = user.role.ToString(),
            });
        }

        return Ok(new {
            statusCode = 200,
            message = "Successfully retrieved all users",
            data = returnUsers
        });
    }
    
    [HttpGet]
    [Route("{id}")]
    public IActionResult GetOne(int id) {
        var user = _userRepository.GetById(id);
        
        if(user == null) {
            return NotFound(new {
                StatusCode = 404,
                Message = $"User with id {id} not found",
                Data = new { }
            });
        }
        
        return Ok(new {
            statusCode = 200,
            message = "Successfully retrieved user",
            data = new {
                id = user.id,
                name = user.name,
                emailAddress = user.emailAddress,
                employeeNr = user.employeeNr,
                hasLicense = user.hasLicense,
                role = user.role.ToString(),
            }
        });
    }
    
    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Create(UserDto user) {
        //Check if dto has errors
        if (!ModelState.IsValid) {
            //get all DtoErrors
            IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
            
            return BadRequest(new {
                StatusCode = 400,
                message = allErrors.ToArray()[0].ErrorMessage,
                data = new { },
            });
        }
        
        //Convert role string to enum
        if (!Enum.TryParse(user.role, out Role role)) {
            return BadRequest(new {
                StatusCode = 400,
                message = "Role is not valid",
                data = new { },
            });
        }
        
        //Check if the validations are correct
        User newUser;
        try {
            newUser = new User{
                name = user.name,
                emailAddress = user.emailAddress,
                employeeNr = user.employeeNr,
                hasLicense = user.hasLicense,
                role = user.role.ToUpper() switch {
                    "USER" => Role.USER,
                    "ADMIN" => Role.ADMIN,
                    _ => throw new Exception("Role is not valid"),
                }
            };
        } catch (Exception ex) {
            return BadRequest(new {
                StatusCode = 400,
                message = ex.Message,
                data = new { },
            });
        }
        
        User resultUser;
        try {
            resultUser = await _userRepository.Create(newUser, user.password);
        } catch (Exception ex) {
            return StatusCode(StatusCodes.Status500InternalServerError, new {
                statusCode = 500,
                message = ex.Message,
                data = new { },
            });
        }


        if (resultUser == null) {
            return StatusCode(StatusCodes.Status500InternalServerError, new{
                statusCode = 500,
                message = "Internal server error",
                data = new{ },
            });
        }


        return Ok(new {
            statusCode = 200,
            message = "Successfully created user",
            data = new{
                id = resultUser.id,
                name = resultUser.name,
                emailAddress = resultUser.emailAddress,
                employeeNr = resultUser.employeeNr,
                hasLicense = resultUser.hasLicense,
                role = resultUser.role.ToString(),
            }
        });
    }

    [HttpPut]
    [Route("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Update(int id, UserUpdateDto user) {
        //Check if dto has errors
        if (!ModelState.IsValid) {
            //get all DtoErrors
            IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
            
            return BadRequest(new {
                StatusCode = 400,
                message = allErrors.ToArray()[0].ErrorMessage,
                data = new { },
            });
        }
        
        //Convert role string to enum
        if (!Enum.TryParse(user.role, out Role role)) {
            return BadRequest(new {
                StatusCode = 400,
                message = "Role is not valid",
                data = new { },
            });
        }
        
        //Check if the validations are correct
        User newUser;
        try {
            newUser = new User{
                name = user.name,
                emailAddress = user.emailAddress,
                employeeNr = user.employeeNr,
                hasLicense = user.hasLicense,
                role = user.role.ToUpper() switch {
                    "USER" => Role.USER,
                    "ADMIN" => Role.ADMIN,
                    _ => throw new Exception("Role is not valid"),
                }
            };
        } catch (Exception ex) {
            return BadRequest(new {
                StatusCode = 400,
                message = ex.Message,
                data = new { },
            });
        }
        
        User resultUser;
        try {
            resultUser = await _userRepository.Update(id, newUser);
        } catch (Exception ex) {
            return StatusCode(StatusCodes.Status500InternalServerError, new {
                statusCode = 500,
                message = ex.Message,
                data = new { },
            });
        }
        
        //If result equals null than user not found
        if (resultUser == null) {
            return NotFound(new {
                statusCode = 404,
                message = $"User with id {id} cannot be found",
                data = new { },
            });
        }
        
        //not null and no exception and than succes
        return Ok(new {
            statusCode = 200,
            message = "Successfully updated user",
            data = new {
                id = resultUser.id,
                name = resultUser.name,
                emailAddress = resultUser.emailAddress,
                employeeNr = resultUser.employeeNr,
                hasLicense = resultUser.hasLicense,
                role = resultUser.role.ToString(),
            }
        });
    }
    
    [HttpDelete]
    [Route("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Delete(int id) {
        if (_userRepository.GetById(id) == null) {
            return NotFound(new {
                statusCode = 404,
                message = $"User with id {id} cannot be found",
                data = new { },
            });
        }

        await _userRepository.Delete(id);

        if (_userRepository.GetById(id) != null) {
            return StatusCode(StatusCodes.Status500InternalServerError, new {
                statusCode = 500,
                message = $"Internal server error",
                data = new { },
            });
        }

        return Ok(new {
            statusCode = 200,
            message = "Successfully deleted user",
            data = new { },
        });
    }
}