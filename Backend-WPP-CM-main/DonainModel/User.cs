using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;

namespace DonainModel {
    public class User {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        
        public string securityId { get; set; } = string.Empty;
        public required string name { get; set; }
        
        private string _emailAddress = string.Empty;
        public required string emailAddress {
            get {
                return _emailAddress;
            }
            set {
                //If emailAddress is not valid throw exception
                Match match = Regex.Match(value, "^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$");
                if(match.Success) {
                    _emailAddress = value;
                } else {
                    throw new ArgumentException($"emailAddress does not have a valid value, value of emailAddress: {value}");
                }
            }
        }

        public required string employeeNr { get; set; }

        public required bool hasLicense { get; set; }

        public required Role role { get; set; }


        //TODO: aanpassen naar goede waarde bij entity
        public ICollection<Reservation> reservations { get; set; } = new List<Reservation>();
        public ICollection<Request> requests { get; set; } = new List<Request>();
    }
}
