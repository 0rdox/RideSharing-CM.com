using DomainServices;
using DonainModel;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Net.Sockets;

namespace Infrastructure.tests {
    public class CarRepositoryTest : IDisposable {
        private AppDbContext _context { get; set; }
        private ICarRepository _repository { get; set; }


        public CarRepositoryTest() {
            //setup db
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseInMemoryDatabase(databaseName: "AuthorDb");

            //set up context
            _context = new AppDbContext(optionsBuilder.Options);

            //set up repo
            _repository = new CarRepository(_context);
        }

        public void Dispose() {
            _context.ChangeTracker.Clear();
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public void get_all_with_empty_db_returns_empty_list() {
            //Arrange


            //Act
            var result = _repository.GetAll();

            //Assert
            Assert.Empty(result);
        }

        [Fact]
        public async void get_all_returns_list() {
            //Arrange
            Car car = new Car(){
                brand = "",
                id = 0,
                imageUrl = "",
                isAvailable = true,
                licensePlate = "999-XX-9",
                location = "",
                model = "",
                seats = 5
            };
            _context.Cars.Add(car);
            await _context.SaveChangesAsync();

            //Act
            var result = _repository.GetAll();
            
            //Assert
            Assert.NotEmpty(result);
            Assert.Equal(car, result.First());
        }

        [Fact]
        public async void get_all_with_numberOfSeats_filter_return_correct_list() {
            //Arrange
            Car[] cars = new Car[] {
                new Car() {
                    brand = "",
                    imageUrl = "",
                    isAvailable = true,
                    licensePlate = "999-XX-9",
                    location = "",
                    model = "",
                    seats = 2,
                },
                new Car() {
                    brand = "",
                    imageUrl = "",
                    isAvailable = true,
                    licensePlate = "999-XX-9",
                    location = "",
                    model = "",
                    seats = 3,
                },
                new Car() {
                    brand = "",
                    imageUrl = "",
                    isAvailable = true,
                    licensePlate = "999-XX-9",
                    location = "",
                    model = "",
                    seats = 5,
                },
            };

            User user = new User() {
                emailAddress = "user@user.nl",
                employeeNr = "",
                hasLicense = true,
                name = "",
                role = Role.USER
            };

            Reservation[] reservations = new Reservation[] {
                new Reservation {
                    departureDate = DateTime.Now.AddMinutes(1),
                    arrivalDate = DateTime.Now.AddHours(5),
                    destination = "destination",
                    willReturn = true,
                    car = cars[0],
                    seats = 1,
                    user = user
                },
                new Reservation {
                    departureDate = DateTime.Now.AddDays(1),
                    arrivalDate = DateTime.Now.AddDays(1).AddHours(5),
                    destination = "Otherdestination",
                    willReturn = true,
                    car = cars[1],
                    seats = 2,
                    user = user
                },
            };

            Request[] requests = new Request[] {
                new Request() {
                    creationDate = DateTime.Now,
                    reservation = reservations[1],
                    seats = 1,
                    user = user,
                    status = Status.ACCEPTED 
                },
                new Request() {
                    creationDate = DateTime.Now,
                    reservation = reservations[0],
                    seats = 1,
                    user = user,
                    status = Status.PENDING
                },
            };

            //add reservations
            cars[0].reservations.Add(reservations[0]);
            cars[1].reservations.Add(reservations[1]);

            //add requests
            reservations[0].requests.Add(requests[1]);
            reservations[1].requests.Add(requests[0]);

            //add to db
            _context.Cars.AddRange(cars);
            _context.Requests.AddRange(requests);
            _context.Users.Add(user);
            _context.Reservations.AddRange(user.reservations);
            await _context.SaveChangesAsync();

            //Act
            var result = _repository.GetAll(numberOfSeats: 1);

            //Assert result contains car at index 1 and 3
            Assert.NotEmpty(result);
            Assert.Contains(cars[0], result);
            Assert.Contains(cars[2], result);
            Assert.DoesNotContain(cars[1], result);
        }
        
        [Fact]
        public async void get_all_with_destination_filter_return_correct_list() {
            //Arrange
            Car[] cars = new Car[] {
                new Car() {
                    brand = "",
                    imageUrl = "",
                    isAvailable = true,
                    licensePlate = "999-XX-9",
                    location = "",
                    model = "",
                    seats = 2,
                },
                new Car() {
                    brand = "",
                    imageUrl = "",
                    isAvailable = true,
                    licensePlate = "999-XX-9",
                    location = "",
                    model = "",
                    seats = 3,
                },
                new Car() {
                    brand = "",
                    imageUrl = "",
                    isAvailable = true,
                    licensePlate = "999-XX-9",
                    location = "",
                    model = "",
                    seats = 5,
                },
            };

            User user = new User() {
                emailAddress = "test@test.com",
                employeeNr = "",
                hasLicense = true,
                name = "",
                role = Role.USER
            };

            Reservation[] reservations = new Reservation[] {
                new Reservation {
                    departureDate = DateTime.Now.AddMinutes(1),
                    arrivalDate = DateTime.Now.AddHours(5),
                    destination = "destination",
                    willReturn = true,
                    car = cars[0],
                    seats = 1,
                    user = user
                },
                new Reservation {
                    departureDate = DateTime.Now.AddDays(1),
                    arrivalDate = DateTime.Now.AddDays(1).AddHours(5),
                    destination = "Otherdestination",
                    willReturn = true,
                    car = cars[1],
                    seats = 2,
                    user = user
                },
            };

            Request[] requests = new Request[] {
                new Request() {
                    creationDate = DateTime.Now,
                    reservation = reservations[1],
                    seats = 1,
                    user = user,
                    status = Status.ACCEPTED
                },
                new Request() {
                    creationDate = DateTime.Now,
                    reservation = reservations[0],
                    seats = 1,
                    user = user,
                    status = Status.PENDING
                },
            };


            //add reservations
            cars[0].reservations.Add(reservations[0]);
            cars[1].reservations.Add(reservations[1]);

            //add requests
            reservations[0].requests.Add(requests[1]);
            reservations[1].requests.Add(requests[0]);

            //add to db
            _context.Cars.AddRange(cars);
            _context.Requests.AddRange(requests);
            _context.Users.Add(user);
            _context.Reservations.AddRange(user.reservations);
            await _context.SaveChangesAsync();

            //Act
            var result = _repository.GetAll(numberOfSeats: 1);

            //Assert result contains car at index 1 and 3
            Assert.NotEmpty(result);
            Assert.Contains(cars[0], result);
            Assert.Contains(cars[2], result);
            Assert.DoesNotContain(cars[1], result);
        }


        [Fact]
        public async void get_all_with_numberOfSeats_and_destination_filter_return_correct_list() {
            //Arrange
            Car[] cars = new Car[] {
                new Car() {
                    brand = "",
                    imageUrl = "",
                    isAvailable = true,
                    licensePlate = "999-XX-9",
                    location = "",
                    model = "",
                    seats = 2,
                },
                new Car() {
                    brand = "",
                    imageUrl = "",
                    isAvailable = true,
                    licensePlate = "999-XX-9",
                    location = "",
                    model = "",
                    seats = 3,
                },
                new Car() {
                    brand = "",
                    imageUrl = "",
                    isAvailable = true,
                    licensePlate = "999-XX-9",
                    location = "",
                    model = "",
                    seats = 5,
                },
            };

            User user = new User() {
                emailAddress = "test@test.com",
                employeeNr = "",
                hasLicense = true,
                name = "",
                role = Role.USER
            };

            Reservation[] reservations = new Reservation[] {
                new Reservation {
                    departureDate = DateTime.Now.AddMinutes(1),
                    arrivalDate = DateTime.Now.AddHours(5),
                    destination = "destination",
                    willReturn = true,
                    car = cars[0],
                    seats = 1,
                    user = user
                },
                new Reservation {
                    departureDate = DateTime.Now.AddDays(1),
                    arrivalDate = DateTime.Now.AddDays(1).AddHours(5),
                    destination = "Otherdestination",
                    willReturn = true,
                    car = cars[1],
                    seats = 2,
                    user = user
                },
            };

            Request[] requests = new Request[] {
                new Request() {
                    creationDate = DateTime.Now,
                    reservation = reservations[1],
                    seats = 1,
                    user = user,
                    status = Status.ACCEPTED
                },
                new Request() {
                    creationDate = DateTime.Now,
                    reservation = reservations[0],
                    seats = 1,
                    user = user,
                    status = Status.PENDING
                },
            };


            //add reservations
            cars[0].reservations.Add(reservations[0]);
            cars[1].reservations.Add(reservations[1]);

            //add requests
            reservations[0].requests.Add(requests[1]);
            reservations[1].requests.Add(requests[0]);

            //add to db
            _context.Cars.AddRange(cars);
            _context.Requests.AddRange(requests);
            _context.Users.Add(user);
            _context.Reservations.AddRange(user.reservations);
            await _context.SaveChangesAsync();

            //Act
            var result = _repository.GetAll(destination: "Otherdestination", numberOfSeats: 1);

            //Assert result contains car at index 1 and 3
            Assert.NotEmpty(result);
            Assert.Contains(cars[2], result);
            Assert.DoesNotContain(cars[0], result);
            Assert.DoesNotContain(cars[1], result);
        }

        [Fact]
        public async void get_all_with_departureDate_filter_return_correct_list() {
            //Arrange
            Car[] cars = new Car[] {
                new Car() {
                    brand = "",
                    imageUrl = "",
                    isAvailable = true,
                    licensePlate = "999-XX-9",
                    location = "",
                    model = "",
                    seats = 2,
                },
                new Car() {
                    brand = "",
                    imageUrl = "",
                    isAvailable = true,
                    licensePlate = "999-XX-9",
                    location = "",
                    model = "",
                    seats = 3,
                },
                new Car() {
                    brand = "",
                    imageUrl = "",
                    isAvailable = true,
                    licensePlate = "999-XX-9",
                    location = "",
                    model = "",
                    seats = 5,
                },
            };

            User user = new User() {
                emailAddress = "test@test.com",
                employeeNr = "",
                hasLicense = true,
                name = "",
                role = Role.USER
            };

            Reservation[] reservations = new Reservation[] {
                new Reservation {
                    departureDate = DateTime.Now.AddMinutes(1),
                    arrivalDate = DateTime.Now.AddHours(5),
                    destination = "destination",
                    willReturn = true,
                    car = cars[0],
                    seats = 1,
                    user = user
                },
                new Reservation {
                    departureDate = DateTime.Now.AddDays(1),
                    arrivalDate = DateTime.Now.AddDays(1).AddHours(5),
                    destination = "Otherdestination",
                    willReturn = true,
                    car = cars[1],
                    seats = 2,
                    user = user
                },
                 new Reservation {
                    departureDate = DateTime.Now.AddDays(1),
                    arrivalDate = DateTime.Now.AddDays(1).AddHours(5),
                    destination = "Otherdestination",
                    willReturn = true,
                    car = cars[2],
                    seats = 2,
                    user = user
                },
            };

            Request[] requests = new Request[] {
                new Request() {
                    creationDate = DateTime.Now,
                    reservation = reservations[1],
                    seats = 1,
                    user = user,
                    status = Status.ACCEPTED
                },
                new Request() {
                    creationDate = DateTime.Now,
                    reservation = reservations[0],
                    seats = 1,
                    user = user,
                    status = Status.PENDING
                },
            };


            //add reservations
            cars[0].reservations.Add(reservations[0]);
            cars[1].reservations.Add(reservations[1]);
            cars[2].reservations.Add(reservations[2]);

            //add requests
            reservations[0].requests.Add(requests[1]);
            reservations[1].requests.Add(requests[0]);

            //add to db
            _context.Cars.AddRange(cars);
            _context.Requests.AddRange(requests);
            _context.Users.Add(user);
            _context.Reservations.AddRange(user.reservations);
            await _context.SaveChangesAsync();

            //Act
            var result = _repository.GetAll(departureDate: DateTime.Now);

            //Assert result contains car at index 1 and 3
            Assert.NotEmpty(result);
            Assert.Contains(cars[0], result);
            Assert.Contains(cars[1], result);
            Assert.Contains(cars[2], result);
        }

        [Fact]
        public async void get_all_with_departureDate_and_destination_filter_return_correct_list() {
            //Arrange
            Car[] cars = new Car[] {
                new Car() {
                    brand = "",
                    imageUrl = "",
                    isAvailable = true,
                    licensePlate = "999-XX-9",
                    location = "",
                    model = "",
                    seats = 2,
                },
                new Car() {
                    brand = "",
                    imageUrl = "",
                    isAvailable = true,
                    licensePlate = "999-XX-9",
                    location = "",
                    model = "",
                    seats = 3,
                },
                new Car() {
                    brand = "",
                    imageUrl = "",
                    isAvailable = true,
                    licensePlate = "999-XX-9",
                    location = "",
                    model = "",
                    seats = 5,
                },
            };

            User user = new User() {
                emailAddress = "test@test.com",
                employeeNr = "",
                hasLicense = true,
                name = "",
                role = Role.USER
            };

            Reservation[] reservations = new Reservation[] {
                new Reservation {
                    departureDate = DateTime.Now.AddMinutes(1),
                    arrivalDate = DateTime.Now.AddHours(5),
                    destination = "destination",
                    willReturn = true,
                    car = cars[0],
                    seats = 1,
                    user = user
                },
                new Reservation {
                    departureDate = DateTime.Now.AddDays(1),
                    arrivalDate = DateTime.Now.AddDays(1).AddHours(5),
                    destination = "Otherdestination",
                    willReturn = true,
                    car = cars[1],
                    seats = 2,
                    user = user
                },
                 new Reservation {
                    departureDate = DateTime.Now.AddMinutes(1),
                    arrivalDate = DateTime.Now.AddHours(5),
                    destination = "Otherdestination",
                    willReturn = true,
                    car = cars[2],
                    seats = 2,
                    user = user
                },
            };

            Request[] requests = new Request[] {
                new Request() {
                    creationDate = DateTime.Now,
                    reservation = reservations[1],
                    seats = 1,
                    user = user,
                    status = Status.ACCEPTED
                },
                new Request() {
                    creationDate = DateTime.Now,
                    reservation = reservations[0],
                    seats = 1,
                    user = user,
                    status = Status.PENDING
                },
            };


            //add reservations
            cars[0].reservations.Add(reservations[0]);
            cars[1].reservations.Add(reservations[1]);
            cars[2].reservations.Add(reservations[2]);

            //add requests
            reservations[0].requests.Add(requests[1]);
            reservations[1].requests.Add(requests[0]);

            //add to db
            _context.Cars.AddRange(cars);
            _context.Requests.AddRange(requests);
            _context.Users.Add(user);
            _context.Reservations.AddRange(user.reservations);
            await _context.SaveChangesAsync();

            //Act
            var result = _repository.GetAll(departureDate: DateTime.Now.AddMinutes(1), destination: "destination");

            //Assert result contains car at index 1 and 3
            Assert.NotEmpty(result);
            Assert.Contains(cars[0], result);
            Assert.Contains(cars[1], result);
            Assert.DoesNotContain(cars[2], result);
        }

        [Fact]
        public async void get_all_with_departureDate_and_numberOfSeats_filter_return_correct_list() {
            //Arrange
            Car[] cars = new Car[] {
                new Car() {
                    brand = "",
                    imageUrl = "",
                    isAvailable = true,
                    licensePlate = "999-XX-9",
                    location = "",
                    model = "",
                    seats = 2,
                },
                new Car() {
                    brand = "",
                    imageUrl = "",
                    isAvailable = true,
                    licensePlate = "999-XX-9",
                    location = "",
                    model = "",
                    seats = 3,
                },
                new Car() {
                    brand = "",
                    imageUrl = "",
                    isAvailable = true,
                    licensePlate = "999-XX-9",
                    location = "",
                    model = "",
                    seats = 5,
                },
            };

            User user = new User() {
                emailAddress = "test@test.com",
                employeeNr = "",
                hasLicense = true,
                name = "",
                role = Role.USER
            };

            Reservation[] reservations = new Reservation[] {
                new Reservation {
                    departureDate = DateTime.Now.AddMinutes(1),
                    arrivalDate = DateTime.Now.AddHours(5),
                    destination = "destination",
                    willReturn = true,
                    car = cars[0],
                    seats = 1,
                    user = user
                },
                new Reservation {
                    departureDate = DateTime.Now.AddDays(1),
                    arrivalDate = DateTime.Now.AddDays(1).AddHours(5),
                    destination = "Otherdestination",
                    willReturn = true,
                    car = cars[1],
                    seats = 2,
                    user = user
                },
                 new Reservation {
                    departureDate = DateTime.Now.AddDays(1),
                    arrivalDate = DateTime.Now.AddDays(1).AddHours(5),
                    destination = "Otherdestination",
                    willReturn = true,
                    car = cars[2],
                    seats = 2,
                    user = user
                },
            };

            Request[] requests = new Request[] {
                new Request() {
                    creationDate = DateTime.Now,
                    reservation = reservations[1],
                    seats = 1,
                    user = user,
                    status = Status.ACCEPTED
                },
                new Request() {
                    creationDate = DateTime.Now,
                    reservation = reservations[0],
                    seats = 1,
                    user = user,
                    status = Status.PENDING
                },
            };


            //add reservations
            cars[0].reservations.Add(reservations[0]);
            cars[1].reservations.Add(reservations[1]);
            cars[2].reservations.Add(reservations[2]);

            //add requests
            reservations[0].requests.Add(requests[1]);
            reservations[1].requests.Add(requests[0]);

            //add to db
            _context.Cars.AddRange(cars);
            _context.Requests.AddRange(requests);
            _context.Users.Add(user);
            _context.Reservations.AddRange(user.reservations);
            await _context.SaveChangesAsync();

            //Act
            var result = _repository.GetAll(departureDate: DateTime.Now, numberOfSeats: 1);

            //Assert result contains car at index 1 and 3
            Assert.NotEmpty(result);
            Assert.Contains(cars[0], result);
            Assert.Contains(cars[2], result);
            Assert.DoesNotContain(cars[1], result);
        }

        [Fact]
        public async void get_all_with_departureDate_and_numberOfSeats_and_destination_filter_return_correct_list() {
            //Arrange
            Car[] cars = new Car[] {
                new Car() {
                    brand = "",
                    imageUrl = "",
                    isAvailable = true,
                    licensePlate = "999-XX-9",
                    location = "",
                    model = "",
                    seats = 2,
                },
                new Car() {
                    brand = "",
                    imageUrl = "",
                    isAvailable = true,
                    licensePlate = "999-XX-9",
                    location = "",
                    model = "",
                    seats = 3,
                },
                new Car() {
                    brand = "",
                    imageUrl = "",
                    isAvailable = true,
                    licensePlate = "999-XX-9",
                    location = "",
                    model = "",
                    seats = 5,
                },
            };

            User user = new User() {
                emailAddress = "test@test.com",
                employeeNr = "",
                hasLicense = true,
                name = "",
                role = Role.USER
            };

            Reservation[] reservations = new Reservation[] {
                new Reservation {
                    departureDate = DateTime.Now.AddMinutes(1),
                    arrivalDate = DateTime.Now.AddHours(5),
                    destination = "destination",
                    willReturn = true,
                    car = cars[0],
                    seats = 1,
                    user = user
                },
                new Reservation {
                    departureDate = DateTime.Now.AddDays(1),
                    arrivalDate = DateTime.Now.AddDays(1).AddHours(5),
                    destination = "Otherdestination",
                    willReturn = true,
                    car = cars[1],
                    seats = 2,
                    user = user
                },
                 new Reservation {
                    departureDate = DateTime.Now.AddDays(1),
                    arrivalDate = DateTime.Now.AddDays(1).AddHours(5),
                    destination = "Otherdestination",
                    willReturn = true,
                    car = cars[2],
                    seats = 2,
                    user = user
                },
            };

            Request[] requests = new Request[] {
                new Request() {
                    creationDate = DateTime.Now,
                    reservation = reservations[1],
                    seats = 1,
                    user = user,
                    status = Status.ACCEPTED
                },
                new Request() {
                    creationDate = DateTime.Now,
                    reservation = reservations[0],
                    seats = 1,
                    user = user,
                    status = Status.PENDING
                },
            };


            //add reservations
            cars[0].reservations.Add(reservations[0]);
            cars[1].reservations.Add(reservations[1]);
            cars[2].reservations.Add(reservations[2]);

            //add requests
            reservations[0].requests.Add(requests[1]);
            reservations[1].requests.Add(requests[0]);

            //add to db
            _context.Cars.AddRange(cars);
            _context.Requests.AddRange(requests);
            _context.Users.Add(user);
            _context.Reservations.AddRange(user.reservations);
            await _context.SaveChangesAsync();

            //Act
            var result = _repository.GetAll(departureDate: DateTime.Now, numberOfSeats: 1, destination: "destination");

            //Assert result contains car at index 1 and 3
            Assert.NotEmpty(result);
            Assert.Contains(cars[0], result);
            Assert.Contains(cars[2], result);
            Assert.DoesNotContain(cars[1], result);
        }

        [Fact]
        async public void create_returns_with_id() {
            //Arrange
            Car car = new Car() {
                brand = "",
                imageUrl = "",
                isAvailable = true,
                licensePlate = "999-XX-9",
                location = "",
                model = "",
                seats = 5
            };

            //Act
            var result = await _repository.Create(car);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(car.licensePlate, result.licensePlate);
        }

        [Fact]
        async public void create_with_duplicatie_licensePlate_returns_exception() {
            //Arrange
            Car car = new Car() {
                brand = "",
                imageUrl = "",
                isAvailable = true,
                licensePlate = "999-XX-9",
                location = "",
                model = "",
                seats = 5
            };
            _context.Cars.Add(car);
            await _context.SaveChangesAsync();

            Car? result = null;
            Exception? exception = null;
          
            //Act
            try {
                result = await _repository.Create(car);
            } catch (Exception exc) {
                exception = exc;
            }

            //Assert
            Assert.Null(result);
            Assert.NotNull(exception);
        }


        [Fact]
        async public void update_with_duplicatie_licensePlate_returns_exception() {
            //Arrange
            Car car = new Car() {
                brand = "",
                imageUrl = "",
                isAvailable = true,
                licensePlate = "999-XX-9",
                location = "",
                model = "",
                seats = 5
            };
            Car car2 = new Car() {
                brand = "",
                imageUrl = "",
                isAvailable = true,
                licensePlate = "99-XX-9",
                location = "",
                model = "",
                seats = 5
            };

            _context.Cars.Add(car);
            _context.Cars.Add(car2);
            await _context.SaveChangesAsync();

            Car? result = null;
            Exception? exception = null;

            //change licensePlate to that of car1
            car2.licensePlate = car.licensePlate;

            //Act
            try {
                result = await _repository.Update(2, car2);
            } catch (Exception exc) {
                exception = exc;
            }

            //Assert
            Assert.Null(result);
            Assert.NotNull(exception);
        }

        [Fact]
        async public void update_with_no_duplicatie_licensePlate_returns_no_exception() {
            //Arrange
            Car car = new Car() {
                brand = "",
                imageUrl = "",
                isAvailable = true,
                licensePlate = "999-XX-9",
                location = "",
                model = "",
                seats = 5
            };
            Car car2 = new Car() {
                brand = "",
                imageUrl = "",
                isAvailable = true,
                licensePlate = "99-XX-9",
                location = "",
                model = "",
                seats = 5
            };

            _context.Cars.Add(car);
            _context.Cars.Add(car2);
            await _context.SaveChangesAsync();

            Car? result = null;
            Exception? exception = null;

            //change licensePlate to that of car1
            car2.licensePlate = "9-XX-9";

            //Act
            try {
                result = await _repository.Update(2, car2);
            } catch (Exception exc) {
                Console.WriteLine(exc);
                exception = exc;
            }

            //Assert
            Assert.NotNull(result);
            Assert.Equal(car2.licensePlate, result.licensePlate);
            Assert.Null(exception);
        }
        
        [Fact]
        async public void delete_with_id_returns_true() {
            //Arrange
            Car car = new Car() {
                brand = "",
                imageUrl = "",
                isAvailable = true,
                licensePlate = "999-XX-9",
                location = "",
                model = "",
                seats = 5
            };

            _context.Cars.Add(car);
            await _context.SaveChangesAsync();

            //Act
            _repository.Delete(1);

            //Assert
            Assert.Empty(_context.Cars);
        }
        
        [Fact]
        async public void delete_with_id_returns_false() {
            //Arrange
            Car car = new Car() {
                brand = "",
                imageUrl = "",
                isAvailable = true,
                licensePlate = "999-XX-9",
                location = "",
                model = "",
                seats = 5
            };

            _context.Cars.Add(car);
            await _context.SaveChangesAsync();

            //Act
            _repository.Delete(2);

            //Assert
            Assert.NotEmpty(_context.Cars);
        }
    }
}