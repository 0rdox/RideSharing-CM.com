using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DonainModel {
    public class Car {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        private string _licensePlate { get; set; } = string.Empty;
        public required string licensePlate {
            get {
                return _licensePlate;
            }
            set {
                //sets to upper case if no aleady
                value = value.ToUpper();

                //Check licensePlate
                try {
                    Car.checkLicensePlate(value);
                } catch (Exception ex) {
                    throw new ArgumentException(ex.Message);
                }

                //Set value
                _licensePlate = value;
            }
        }
        public required string brand { get; set; }

        public required string model { get; set; }

        private int _seats;
        public required int seats {
            get {
                return _seats;
            }
            set {
                //If seats is not more than 0 throw exception
                if (value < 0) {
                    throw new ArgumentOutOfRangeException($"Seats cannot be lower than 0, given value is: {value}");
                }
                _seats = value;
            }
        }

        public required string imageUrl { get; set; }

        public required string location { get; set; }

        public required bool isAvailable { get; set; }

        public ICollection<Reservation> reservations { get; set; } = new List<Reservation>();

        public static void checkLicensePlate(string licensePlate) {
             //check for lower case
            if (licensePlate.Any(char.IsLower)) {
                throw new ArgumentException($"LicensePlate cannot have lowerCase values");
            }

            //check forbidden characters
            char[] vowels = { 'A', 'E', 'I', 'O', 'U' };
            for (int i = 0; i < vowels.Length; i++) {
                if (licensePlate.Contains(vowels[i])) {
                    throw new ArgumentException($"LicensePlate has forbidden character, character {vowels[i]}");
                }
            }

            //Check for forbidden characters combinations
            string[] combination = { "GVD", "KKK", "KVT", "LPF", "NSB", "PKK", "PSV", "TBS", "SS", "SD", "PVV", "SGP", "VVD" };
            for (int i = 0; i < combination.Length; i++) {
                if (licensePlate.Contains(combination[i])) {
                    throw new ArgumentException($"LicensePlate has forbidden character combination, combination {combination[i]}");
                }
            }


            //Check number of -
            string[] arr = licensePlate.Split("-");
            if (arr.Length != 3) {
                throw new ArgumentException($"LicensePlate is not correct, check https://www.rdw.nl/particulier/voertuigen/auto/de-kentekenplaat/cijfers-en-letters-op-de-kentekenplaat for allowed license plates.");
            }
        }
    }
}
