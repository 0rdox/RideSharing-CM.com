using System.ComponentModel.DataAnnotations;

namespace DonainModel.dto {
    public class LoginDto {
        [Required(ErrorMessage = "email is required")]
        [EmailAddress(ErrorMessage = "email is not valid")]
        public string email {  get; set; } 

        [Required(ErrorMessage = "password is required")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).{8,}$", ErrorMessage = "Password must contain at least one lowercase letter, one uppercase letter, one digit, one special character and be at least 8 characters long.")]
        public string password { get; set; }

    }
}
