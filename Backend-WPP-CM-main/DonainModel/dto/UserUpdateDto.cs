using System.ComponentModel.DataAnnotations;

namespace DonainModel.dto; 

public class UserUpdateDto {
    [Required (ErrorMessage = "email is required")]
    [EmailAddress(ErrorMessage = "email is not valid")]
    public string emailAddress { get; set; } = string.Empty;
    
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