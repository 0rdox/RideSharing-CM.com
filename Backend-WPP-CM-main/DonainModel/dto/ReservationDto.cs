using System.ComponentModel.DataAnnotations;

namespace DonainModel.dto {
    public class ReservationDto {
        [Required(ErrorMessage = "departureDate is required")]
        public string departureDate {get;set;} = string.Empty;

        [Required(ErrorMessage = "arrivalDate is required")]
        public string arrivalDate { get; set; } = string.Empty;

        [Required(ErrorMessage = "destination is required")]
        public string destination { get; set; } = string.Empty;

        [Required(ErrorMessage = "willReturn is required")]
        public bool willReturn { get; set; } 

        [Required(ErrorMessage = "seats is required")]
        [RegularExpression("([0-9]+)", ErrorMessage = "seats needs to be an integer")]
        [Range(1, int.MaxValue, ErrorMessage = "There need to be atleast 1 seat")]
        public int seats { get; set; }

        [Required(ErrorMessage = "carId is required")]
        [RegularExpression("([0-9]+)", ErrorMessage = "carId needs to be an integer")]
        [Range(1, int.MaxValue, ErrorMessage = "carId cannot be lower than 1")]
        public int carId { get; set; }
    }
}

