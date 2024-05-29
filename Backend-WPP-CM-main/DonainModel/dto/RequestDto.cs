using System.ComponentModel.DataAnnotations;

namespace DonainModel.dto {
    public class RequestDto {
        [Required(ErrorMessage = "seats is required")]
        [RegularExpression("([0-9]+)", ErrorMessage = "seats needs to be an integer")]
        [Range(1, int.MaxValue, ErrorMessage = "seats cannot be lower than 1")]

        public int seats { get; set; }

        [Required(ErrorMessage = "reservationId is required")]
        [RegularExpression("([0-9]+)", ErrorMessage = "reservationId needs to be an integer")]
        [Range(1, int.MaxValue, ErrorMessage = "reservationId cannot be lower than 1")]
        public int reservationId { get; set; }   
    }
}

