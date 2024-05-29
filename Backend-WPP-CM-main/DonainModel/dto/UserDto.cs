using System.ComponentModel.DataAnnotations;

namespace DonainModel.dto; 

public class UserDto {
    [Required (ErrorMessage = "email is required")]
    [EmailAddress(ErrorMessage = "email is not valid")]
    public string emailAddress { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "password is required")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).{8,}$", ErrorMessage = "Password must contain at least one lowercase letter, one uppercase letter, one digit, one special character and be at least 8 characters long.")]
    public string password { get; set; } = string.Empty;

    [Required(ErrorMessage = "name is required")]
    public string name { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "employeeNr is required")]
    public string employeeNr { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "hasLicense is required")]
    public bool hasLicense { get; set; }
    
    [Required(ErrorMessage = "role is required")]
    [RegularExpression(@"^(USER|ADMIN)$", ErrorMessage = "role must be USER or ADMIN")]
    public string role { get; set; }

}