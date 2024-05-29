using System.ComponentModel.DataAnnotations;

namespace DonainModel.dto {
    public class CarDto {

        [Required(ErrorMessage = "licensePlate is required")]
        public string licensePlate { get; set; } = string.Empty;

        [Required(ErrorMessage = "brand is required")]
        public string brand { get; set; } = string.Empty;


        [Required(ErrorMessage = "model is required")]
        public string model { get; set; } = string.Empty;


        [Required(ErrorMessage = "seats is required")]
        [RegularExpression("([0-9]+)", ErrorMessage = "seats needs to be an integer")]
        [Range(1, int.MaxValue, ErrorMessage = "There need to be atleast 1 seat")]
        public int seats { get; set; } 


        [Required(ErrorMessage = "imageUrl is required")]
        public string imageUrl { get; set; } = string.Empty;


        [Required(ErrorMessage = "location is required")]
        public string location { get; set; } = string.Empty;


        [Required(ErrorMessage = "isAvailable  is required")]
        public bool isAvailable { get; set; }
    }
}
