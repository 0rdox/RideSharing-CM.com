using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DonainModel {
    public class Request {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public required int seats { get; set; }
        public required Status status { get; set; }
        public DateTime creationDate { get; set; } = DateTime.Now;
        
        public required Reservation? reservation { get; set; }
        public required User user { get; set; }

        public string? token { get; set; } = null;
    }
}
