using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DomainServices;
using DonainModel.dto;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace API.Controllers; 

[ApiController]
public class AuthController : ControllerBase {
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;
    
    public AuthController(UserManager<IdentityUser> userManager, IUserRepository userRepository, IConfiguration configuration) {
        _userManager = userManager;
        _userRepository = userRepository;
        _configuration = configuration;
    }

    [HttpPost]
    [Route("api/auth/login")]
    [Produces("application/json")]
    public async Task<ActionResult> Login([FromBody] LoginDto dto) {
        //check modelstate
        if (!ModelState.IsValid) {
            var dtoErrors = ModelState.Values.SelectMany(v => v.Errors);
            return BadRequest(new{
                StatusCode = 400,
                Message = dtoErrors.ToArray()[0].ErrorMessage,
                data = new{ }
            });
        }
    
        var user = await _userManager.FindByEmailAsync(dto.email);
    
        if (user == null) {
            return Unauthorized(new {
                StatusCode = 401,
                Message = "Email or password are incorrect",
                data = new { }
            });
        }
    
        var checkPasswordResult = await _userManager.CheckPasswordAsync(user, dto.password);
        if (!checkPasswordResult)
            return Unauthorized(new {
                StatusCode = 401,
                Message = "Email or password are incorrect",
                Data = new { }
            });
    
        var token = await CreateToken(user);
    
        return Ok(new {
            StatusCode = 200,
            Message = "User logged in successful",
            data = new {
                bearerToken = token
            }
        });
    
    }

    [HttpGet]
    [Route("api/auth/profile")]
    [Produces("application/json")]
    [Authorize]
    public async Task<ActionResult> Profile() {
        var identityUser = await _userManager.FindByIdAsync(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var user = _userRepository.GetBySecurityId(identityUser.Id);
        
        if(user == null) return NotFound(new {
            StatusCode = 404,
            Message = "User not found",
            Data = new { }
        });
        
        return Ok(new {
            StatusCode = 200,
            Message = "User profile retrieved successful",
            data = new{
                id = user.id,
                securityId = user.securityId,
                name = user.name,
                emailAddress = user.emailAddress,
                employeeNr = user.employeeNr,
                hasLicense = user.hasLicense,
                role = user.role.ToString(),
            }
        });
    }
    
    private async Task<string> CreateToken(IdentityUser user) {
        var roles = await _userManager.GetRolesAsync(user);
        List<Claim> claims = new List<Claim> { 
            new Claim(ClaimTypes.NameIdentifier, user.Id),
        };
        
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }
        
        //get key
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            _configuration.GetSection("Jwt:Key").Value!));
        
        //get credentials
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        //get expiration date
        var expiration = DateTime.Now.AddDays(1);
        
        //create token
        var token = new JwtSecurityToken(
            issuer: _configuration.GetSection("Jwt:Issuer").Value!,
            audience: _configuration.GetSection("Jwt:Audience").Value!,
            claims: claims,
            expires: expiration,
            signingCredentials: credentials
        );
        
        //return token
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}