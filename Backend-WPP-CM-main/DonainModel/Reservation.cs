using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace DonainModel {
    public class Reservation {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        private DateTime _departureDate;
        public required DateTime departureDate {
            get {
                return _departureDate;
            }
            set {
                //If departureDate is not now today throw exception
                if(value >= DateTime.Now) {
                    _departureDate = value;
                } else {
                    throw new ArgumentOutOfRangeException("departureDate cannot be earlier time than now");
                }
            }
        }

        private DateTime _arrivalDate;
        public required DateTime arrivalDate {
            get {
                return _arrivalDate;
            }
            set {

                //If departureDate is not now today throw exception
                if (value > _departureDate) {
                    _arrivalDate = value;
                } else {
                    throw new ArgumentOutOfRangeException($"departureDate cannot be earlier or at the same time as departureDate. DepartureDate has value: {_departureDate}, ArrivalDate has value: {value}");
                }
            }
        }

        public required string destination { get; set; }
        public required bool willReturn { get; set; }
        public required int seats { get; set; }
        public DateTime creationDate { get; set; } = DateTime.Now;
        public ICollection<Request> requests { get; set; } = new List<Request>();
        public required Car car { get; set; }
        public required User user { get; set; }

        //returns total amount of available seats
        public int getTotalAvailalbeSeats() {
            int total = this.seats;
            foreach (var request in requests)
            {
                if(request.status == Status.ACCEPTED) {
                    total += request.seats;                
                }
            }
            return car.seats - total;
        }
    }
}
