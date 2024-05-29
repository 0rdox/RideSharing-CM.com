using DonainModel;
using System.Net.Mail;

namespace DomainModel.tests {
    public class ReservationTest {

        [Fact]
        public void departureTime_with_dateTime_before_now_gives_exception() {
            //Arrange
            Exception? exception = null;
            Car car = new Car() {
                brand = "",
                id = 0,
                imageUrl = "",
                isAvailable = true,
                licensePlate = "999-XX-9",
                location = "",
                model = "",
                seats = 2
            };

            User user = new User() {
                emailAddress = "test@test.com",
                    employeeNr = "",
                hasLicense = true,
                id = 0,
                name = "",
                role = Role.USER
            };

            //Act
            try {
                Reservation reservation = new Reservation() {
                    departureDate = DateTime.Now.AddDays(-1),
                    arrivalDate = DateTime.Now.AddDays(5),
                    car = car,
                    user = user,
                    id = 0,
                    seats = 1,
                    destination = "",
                    willReturn = false
                };
            } catch (Exception ex) {
                exception = ex;
            }


            //Assert
            Assert.NotNull(exception);
        }

        [Fact]
        public void departureTime_with_dateTime_same_as_now_gives_exception() {
            //Arrange
            Exception? exception = null;
            Car car = new Car() {
                brand = "",
                id = 0,
                imageUrl = "",
                isAvailable = true,
                licensePlate = "999-XX-9",
                location = "",
                model = "",
                seats = 2
            };

            User user = new User() {
                emailAddress = "test@test.com",
                employeeNr = "",
                hasLicense = true,
                id = 0,
                name = "",
                role = Role.USER
            };

            //Act
            try {
                Reservation reservation = new Reservation() {
                    departureDate = DateTime.Now.AddMinutes(1),
                    arrivalDate = DateTime.Now.AddDays(5),
                    car = car,
                    user = user,
                    id = 0,
                    seats = 1,
                    destination = "",
                    willReturn = false
                };
            } catch (Exception ex) {
                exception = ex;
            }


            //Assert
            Assert.Null(exception);
        }

        [Fact]
        public void departureTime_with_dateTime_after_now_gives_no_exception() {
            //Arrange
            Exception? exception = null;
            Car car = new Car() {
                brand = "",
                id = 0,
                imageUrl = "",
                isAvailable = true,
                licensePlate = "999-XX-9",
                location = "",
                model = "",
                seats = 2
            };

            User user = new User() {
                emailAddress = "test@test.com",
                employeeNr = "",
                hasLicense = true,
                id = 0,
                name = "",
                role = Role.USER
            };

            //Act
            try {
                Reservation reservation = new Reservation() {
                    departureDate = DateTime.Now.AddDays(1),
                    arrivalDate = DateTime.Now.AddDays(5),
                    car = car,
                    user = user,
                    id = 0,
                    seats = 1,
                    destination = "",
                    willReturn = false
                };
            } catch (Exception ex) {
                exception = ex;
            }


            //Assert
            Assert.Null(exception);
        }


        [Fact]
        public void arrivalTime_with_dateTime_before_departureTime_gives_exception() {
            //Arrange
            Exception? exception = null;
            Car car = new Car() {
                brand = "",
                id = 0,
                imageUrl = "",
                isAvailable = true,
                licensePlate = "999-XX-9",
                location = "",
                model = "",
                seats = 2
            };

            User user = new User() {
                emailAddress = "test@test.com",
                employeeNr = "",
                hasLicense = true,
                id = 0,
                name = "",
                role = Role.USER
            };

            //Act
            try {
                Reservation reservation = new Reservation() {
                    departureDate = DateTime.Now.AddDays(2),
                    arrivalDate = DateTime.Now.AddDays(1),
                    car = car,
                    user = user,
                    id = 0,
                    seats = 1,
                    destination = "",
                    willReturn = false
                };
            } catch (Exception ex) {
                exception = ex;
            }


            //Assert
            Assert.NotNull(exception);
        }

        [Fact]
        public void arrivalTime_with_dateTime_same_as_departureTime_gives_no_exception() {
            //Arrange
            Exception? exception = null;
            Car car = new Car() {
                brand = "",
                id = 0,
                imageUrl = "",
                isAvailable = true,
                licensePlate = "999-XX-9",
                location = "",
                model = "",
                seats = 2
            };

            User user = new User() {
                emailAddress = "test@test.com",
                employeeNr = "",
                hasLicense = true,
                id = 0,
                name = "",
                role = Role.USER
            };

            //Act
            try {
                Reservation reservation = new Reservation() {
                    departureDate = DateTime.Now.AddMinutes(1),
                    arrivalDate = DateTime.Now.AddMinutes(1),
                    car = car,
                    user = user,
                    id = 0,
                    seats = 1,
                    destination = "",
                    willReturn = false
                };
            } catch (Exception ex) {
                exception = ex;
            }


            //Assert
            Assert.Null(exception);
        }

        [Fact]
        public void arrivalTime_with_dateTime_after_departureTime_gives_no_exception() {
            //Arrange
            Exception? exception = null;
            Car car = new Car() {
                brand = "",
                id = 0,
                imageUrl = "",
                isAvailable = true,
                licensePlate = "999-XX-9",
                location = "",
                model = "",
                seats = 2
            };

            User user = new User() {
                emailAddress = "test@test.com",
                employeeNr = "",
                hasLicense = true,
                id = 0,
                name = "",
                role = Role.USER
            };

            //Act
            try {
                Reservation reservation = new Reservation() {
                    departureDate = DateTime.Now.AddDays(1),
                    arrivalDate = DateTime.Now.AddDays(5),
                    car = car,
                    user = user,
                    id = 0,
                    seats = 1,
                    destination = "",
                    willReturn = false
                };
            } catch (Exception ex) {
                exception = ex;
            }


            //Assert
            Assert.Null(exception);
        }
    }
}
