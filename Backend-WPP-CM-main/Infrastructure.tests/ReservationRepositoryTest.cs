
using DomainServices;
using DonainModel;
using DonainModel.dto;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.tests {
    public class ReservationRepositoryTest : IDisposable {
        private AppDbContext _context { get; set; }
        private IReservationRepository _repository { get; set; }


        public ReservationRepositoryTest() {
            //setup db
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseInMemoryDatabase(databaseName: "ReservationDb");

            //set up context
            _context = new AppDbContext(optionsBuilder.Options);

            //set up repo
            _repository = new ReservationRepository(_context);
        }

        public void Dispose() {
            _context.ChangeTracker.Clear();
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public void get_all_with_empty_list_returns_no_reservations() {
            //Arrange


            //Act
            var result = _repository.GetAll();

            //Assert
            Assert.Empty(result);
        }


        [Fact]
        public async void get_all_without_filters_returns_all_reservations() {
            //Arrange
            Car car = new Car() {
                brand = "",
                id = 0,
                imageUrl = "",
                isAvailable = true,
                licensePlate = "999-XX-9",
                location = "",
                model = "",
                seats = 5
            };

            User user = new User() {
                name = "Test",
                emailAddress = "Email@email.nl",
                employeeNr = "123",
                hasLicense = true,
                role = Role.USER
            };

            Reservation reservation = new Reservation() {
                departureDate = DateTime.Now.AddMinutes(1),
                arrivalDate = DateTime.Now.AddHours(1),
                destination = "",
                willReturn = true,
                car = car,
                seats = 2,
                user = user
            };
            _context.Cars.Add(car);
            _context.Reservations.Add(reservation);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            //Act
            var result = _repository.GetAll();

            //Assert
            Assert.NotEmpty(result);
            Assert.Contains(reservation, result);
        }

        [Fact]
        public async void get_with_user_filters_returns_all_reservations() {
            //Arrange
            Car car = new Car() {
                brand = "",
                id = 0,
                imageUrl = "",
                isAvailable = true,
                licensePlate = "999-XX-9",
                location = "",
                model = "",
                seats = 5
            };

            User[] users = {
                new User {
                    id = 1,
                    name = "Test",
                    emailAddress = "Email@email.nl",
                    employeeNr = "123",
                    hasLicense = true,
                    role = Role.USER
                },
                new User {
                    id = 2,
                    name = "Test",
                    emailAddress = "Email2@email.nl",
                    employeeNr = "123",
                    hasLicense = true,
                    role = Role.USER
                },
            };

            Reservation[] reservations = {
                new Reservation{
                    departureDate = DateTime.Now.AddMinutes(1),
                    arrivalDate = DateTime.Now.AddHours(1),
                    destination = "",
                    willReturn = true,
                    car = car,
                    seats = 2,
                    user = users[0]
                },
                new Reservation{
                    departureDate = DateTime.Now.AddDays(1),
                    arrivalDate = DateTime.Now.AddDays(1).AddHours(1),
                    destination = "",
                    willReturn = true,
                    car = car,
                    seats = 2,
                    user = users[1]
                },
            };

            _context.Cars.Add(car);
            _context.Reservations.AddRange(reservations);
            _context.Users.AddRange(users);
            await _context.SaveChangesAsync();

            //Act
            var result = _repository.GetAll(userId: users[0].id);

            //Assert
            Assert.NotEmpty(result);
            Assert.Contains(reservations[0], result);
            Assert.DoesNotContain(reservations[1], result);
        }

        [Fact]
        public async void get_with_car_filters_returns_all_reservations() {
            //Arrange
            Car[] cars = {
                new Car {
                    brand = "",
                    id = 1,
                    imageUrl = "",
                    isAvailable = true,
                    licensePlate = "999-XX-9",
                    location = "",
                    model = "",
                    seats = 5
                },
                new Car {
                    brand = "",
                    id = 2,
                    imageUrl = "",
                    isAvailable = true,
                    licensePlate = "99-XX-99",
                    location = "",
                    model = "",
                    seats = 5
                },
            };

            User[] users = {
                new User {
                    id = 1,
                    name = "Test",
                    emailAddress = "Email@email.nl",
                    employeeNr = "123",
                    hasLicense = true,
                    role = Role.USER
                },
                new User {
                    id = 2,
                    name = "Test",
                    emailAddress = "Email2@email.nl",
                    employeeNr = "123",
                    hasLicense = true,
                    role = Role.USER
                },
            };

            Reservation[] reservations = {
                new Reservation{
                    departureDate = DateTime.Now.AddMinutes(1),
                    arrivalDate = DateTime.Now.AddHours(1),
                    destination = "",
                    willReturn = true,
                    car = cars[0],
                    seats = 2,
                    user = users[0]
                },
                new Reservation{
                    departureDate = DateTime.Now.AddDays(1),
                    arrivalDate = DateTime.Now.AddDays(1).AddHours(1),
                    destination = "",
                    willReturn = true,
                    car = cars[1],
                    seats = 2,
                    user = users[0]
                },
            };

            _context.Cars.AddRange(cars);
            _context.Reservations.AddRange(reservations);
            _context.Users.AddRange(users);
            await _context.SaveChangesAsync();

            //Act
            var result = _repository.GetAll(carId: cars[0].id);

            //Assert
            Assert.NotEmpty(result);
            Assert.Contains(reservations[0], result);
            Assert.DoesNotContain(reservations[1], result);
        }

        [Fact]
        public async void get_with_car_and_user_filters_returns_all_reservations() {
            //Arrange
            Car[] cars = {
                new Car {
                    brand = "",
                    id = 1,
                    imageUrl = "",
                    isAvailable = true,
                    licensePlate = "999-XX-9",
                    location = "",
                    model = "",
                    seats = 5
                },
                new Car {
                    brand = "",
                    id = 2,
                    imageUrl = "",
                    isAvailable = true,
                    licensePlate = "99-XX-99",
                    location = "",
                    model = "",
                    seats = 5
                },
            };

            User[] users = {
                new User {
                    id = 1,
                    name = "Test",
                    emailAddress = "Email@email.nl",
                    employeeNr = "123",
                    hasLicense = true,
                    role = Role.USER
                },
                new User {
                    id = 2,
                    name = "Test",
                    emailAddress = "Email2@email.nl",
                    employeeNr = "123",
                    hasLicense = true,
                    role = Role.USER
                },
            };

            Reservation[] reservations = {
                new Reservation{
                    departureDate = DateTime.Now.AddMinutes(1),
                    arrivalDate = DateTime.Now.AddHours(1),
                    destination = "",
                    willReturn = true,
                    car = cars[0],
                    seats = 2,
                    user = users[0]
                },
                new Reservation{
                    departureDate = DateTime.Now.AddDays(1),
                    arrivalDate = DateTime.Now.AddDays(1).AddHours(1),
                    destination = "",
                    willReturn = true,
                    car = cars[1],
                    seats = 2,
                    user = users[1]
                },
            };

            _context.Cars.AddRange(cars);
            _context.Reservations.AddRange(reservations);
            _context.Users.AddRange(users);
            await _context.SaveChangesAsync();

            //Act
            var result = _repository.GetAll(userId: users[0].id ,carId: cars[0].id);

            //Assert
            Assert.NotEmpty(result);
            Assert.Contains(reservations[0], result);
            Assert.DoesNotContain(reservations[1], result);
        }

        [Fact]
        public async void get_with_beginDate_filter_between_departure_and_arrival_returns_all_reservations() {
            //Arrange
            Car[] cars = {
                new Car {
                    brand = "",
                    id = 1,
                    imageUrl = "",
                    isAvailable = true,
                    licensePlate = "999-XX-9",
                    location = "",
                    model = "",
                    seats = 5
                },
                new Car {
                    brand = "",
                    id = 2,
                    imageUrl = "",
                    isAvailable = true,
                    licensePlate = "99-XX-99",
                    location = "",
                    model = "",
                    seats = 5
                },
            };

            User[] users = {
                new User {
                    id = 1,
                    name = "Test",
                    emailAddress = "Email@email.nl",
                    employeeNr = "123",
                    hasLicense = true,
                    role = Role.USER
                },
                new User {
                    id = 2,
                    name = "Test",
                    emailAddress = "Email2@email.nl",
                    employeeNr = "123",
                    hasLicense = true,
                    role = Role.USER
                },
            };

            Reservation[] reservations = {
                new Reservation{
                    departureDate = DateTime.Now.AddMinutes(1),
                    arrivalDate = DateTime.Now.AddHours(1),
                    destination = "",
                    willReturn = true,
                    car = cars[0],
                    seats = 2,
                    user = users[0]
                },
                new Reservation{
                    departureDate = DateTime.Now.AddDays(1),
                    arrivalDate = DateTime.Now.AddDays(1).AddHours(1),
                    destination = "",
                    willReturn = true,
                    car = cars[1],
                    seats = 2,
                    user = users[1]
                },
            };

            _context.Cars.AddRange(cars);
            _context.Reservations.AddRange(reservations);
            _context.Users.AddRange(users);
            await _context.SaveChangesAsync();

            //Act
            var result = _repository.GetAll(beginDate: DateTime.Now.AddMinutes(5));

            //Assert
            Assert.NotEmpty(result);
            Assert.Contains(reservations[0], result);
            Assert.Contains(reservations[1], result);
        }

        [Fact]
        public async void get_with_beginDate_filter_after_arrival_of_one_but_before_departure_of_another_returns_all_reservations() {
            //Arrange
            Car[] cars = {
                new Car {
                    brand = "",
                    id = 1,
                    imageUrl = "",
                    isAvailable = true,
                    licensePlate = "999-XX-9",
                    location = "",
                    model = "",
                    seats = 5
                },
                new Car {
                    brand = "",
                    id = 2,
                    imageUrl = "",
                    isAvailable = true,
                    licensePlate = "99-XX-99",
                    location = "",
                    model = "",
                    seats = 5
                },
            };

            User[] users = {
                new User {
                    id = 1,
                    name = "Test",
                    emailAddress = "Email@email.nl",
                    employeeNr = "123",
                    hasLicense = true,
                    role = Role.USER
                },
                new User {
                    id = 2,
                    name = "Test",
                    emailAddress = "Email2@email.nl",
                    employeeNr = "123",
                    hasLicense = true,
                    role = Role.USER
                },
            };

            Reservation[] reservations = {
                new Reservation{
                    departureDate = DateTime.Now.AddMinutes(1),
                    arrivalDate = DateTime.Now.AddHours(1),
                    destination = "",
                    willReturn = true,
                    car = cars[0],
                    seats = 2,
                    user = users[0]
                },
                new Reservation{
                    departureDate = DateTime.Now.AddDays(1),
                    arrivalDate = DateTime.Now.AddDays(1).AddHours(1),
                    destination = "",
                    willReturn = true,
                    car = cars[1],
                    seats = 2,
                    user = users[1]
                },
            };

            _context.Cars.AddRange(cars);
            _context.Reservations.AddRange(reservations);
            _context.Users.AddRange(users);
            await _context.SaveChangesAsync();

            //Act
            var result = _repository.GetAll(beginDate: DateTime.Now.AddHours(1).AddMinutes(5));

            //Assert
            Assert.NotEmpty(result);
            Assert.DoesNotContain(reservations[0], result);
            Assert.Contains(reservations[1], result);
        }

        [Fact]
        public async void get_with_endDate_filter_after_arrival_of_one_but_before_arrival_of_another_returns_all_reservations() {
            //Arrange
            Car[] cars = {
                new Car {
                    brand = "",
                    id = 1,
                    imageUrl = "",
                    isAvailable = true,
                    licensePlate = "999-XX-9",
                    location = "",
                    model = "",
                    seats = 5
                },
                new Car {
                    brand = "",
                    id = 2,
                    imageUrl = "",
                    isAvailable = true,
                    licensePlate = "99-XX-99",
                    location = "",
                    model = "",
                    seats = 5
                },
            };

            User[] users = {
                new User {
                    id = 1,
                    name = "Test",
                    emailAddress = "Email@email.nl",
                    employeeNr = "123",
                    hasLicense = true,
                    role = Role.USER
                },
                new User {
                    id = 2,
                    name = "Test",
                    emailAddress = "Email2@email.nl",
                    employeeNr = "123",
                    hasLicense = true,
                    role = Role.USER
                },
            };

            Reservation[] reservations = {
                new Reservation{
                    departureDate = DateTime.Now.AddMinutes(1),
                    arrivalDate = DateTime.Now.AddHours(1),
                    destination = "",
                    willReturn = true,
                    car = cars[0],
                    seats = 2,
                    user = users[0]
                },
                new Reservation{
                    departureDate = DateTime.Now.AddDays(1),
                    arrivalDate = DateTime.Now.AddDays(1).AddHours(1),
                    destination = "",
                    willReturn = true,
                    car = cars[1],
                    seats = 2,
                    user = users[1]
                },
            };

            _context.Cars.AddRange(cars);
            _context.Reservations.AddRange(reservations);
            _context.Users.AddRange(users);
            await _context.SaveChangesAsync();

            //Act
            var result = _repository.GetAll(endDate: DateTime.Now.AddHours(1).AddMinutes(5));

            //Assert
            Assert.NotEmpty(result);
            Assert.Contains(reservations[0], result);
            Assert.DoesNotContain(reservations[1], result);
        }

        [Fact]
        public async void get_with_endDate_filter_between_departure_and_arrival_returns_all_reservations() {
            //Arrange
            Car[] cars = {
                new Car {
                    brand = "",
                    id = 1,
                    imageUrl = "",
                    isAvailable = true,
                    licensePlate = "999-XX-9",
                    location = "",
                    model = "",
                    seats = 5
                },
                new Car {
                    brand = "",
                    id = 2,
                    imageUrl = "",
                    isAvailable = true,
                    licensePlate = "99-XX-99",
                    location = "",
                    model = "",
                    seats = 5
                },
            };

            User[] users = {
                new User {
                    id = 1,
                    name = "Test",
                    emailAddress = "Email@email.nl",
                    employeeNr = "123",
                    hasLicense = true,
                    role = Role.USER
                },
                new User {
                    id = 2,
                    name = "Test",
                    emailAddress = "Email2@email.nl",
                    employeeNr = "123",
                    hasLicense = true,
                    role = Role.USER
                },
            };

            Reservation[] reservations = {
                new Reservation{
                    departureDate = DateTime.Now.AddMinutes(1),
                    arrivalDate = DateTime.Now.AddHours(1),
                    destination = "",
                    willReturn = true,
                    car = cars[0],
                    seats = 2,
                    user = users[0]
                },
                new Reservation{
                    departureDate = DateTime.Now.AddDays(1),
                    arrivalDate = DateTime.Now.AddDays(1).AddHours(1),
                    destination = "",
                    willReturn = true,
                    car = cars[1],
                    seats = 2,
                    user = users[1]
                },
            };

            _context.Cars.AddRange(cars);
            _context.Reservations.AddRange(reservations);
            _context.Users.AddRange(users);
            await _context.SaveChangesAsync();

            //Act
            var result = _repository.GetAll(endDate: DateTime.Now.AddMinutes(5));

            //Assert
            Assert.NotEmpty(result);
            Assert.Contains(reservations[0], result);
            Assert.DoesNotContain(reservations[1], result);
        }

        [Fact]
        public async void get_with_endDate_and_startDate_filters_returns_all_reservations() {
            //Arrange
            Car[] cars = {
                new Car {
                    brand = "",
                    id = 1,
                    imageUrl = "",
                    isAvailable = true,
                    licensePlate = "999-XX-9",
                    location = "",
                    model = "",
                    seats = 5
                },
                new Car {
                    brand = "",
                    id = 2,
                    imageUrl = "",
                    isAvailable = true,
                    licensePlate = "99-XX-99",
                    location = "",
                    model = "",
                    seats = 5
                },
            };

            User[] users = {
                new User {
                    id = 1,
                    name = "Test",
                    emailAddress = "Email@email.nl",
                    employeeNr = "123",
                    hasLicense = true,
                    role = Role.USER
                },
                new User {
                    id = 2,
                    name = "Test",
                    emailAddress = "Email2@email.nl",
                    employeeNr = "123",
                    hasLicense = true,
                    role = Role.USER
                },
            };

            Reservation[] reservations = {
                new Reservation{
                    departureDate = DateTime.Now.AddMinutes(1),
                    arrivalDate = DateTime.Now.AddHours(1),
                    destination = "",
                    willReturn = true,
                    car = cars[0],
                    seats = 2,
                    user = users[0]
                },
                new Reservation{
                    departureDate = DateTime.Now.AddDays(1),
                    arrivalDate = DateTime.Now.AddDays(1).AddHours(1),
                    destination = "",
                    willReturn = true,
                    car = cars[1],
                    seats = 2,
                    user = users[1]
                },
            };

            _context.Cars.AddRange(cars);
            _context.Reservations.AddRange(reservations);
            _context.Users.AddRange(users);
            await _context.SaveChangesAsync();

            //Act
            var result = _repository.GetAll(beginDate: DateTime.Now.AddHours(1).AddMinutes(5), endDate: DateTime.Now.AddDays(1).AddMinutes(5));

            //Assert
            Assert.NotEmpty(result);
            Assert.DoesNotContain(reservations[0], result);
            Assert.Contains(reservations[1], result);
        }

        [Fact]
        public async void create_with_departureDate_inbetween_other_reservations_throws_exception() {
            //Arrange
            var user = await fillDbForCreateAndGetUser();

            ReservationDto dto = new ReservationDto() {
                departureDate = DateTime.Now.AddHours(1).ToString(),
                arrivalDate = DateTime.Now.AddHours(2).ToString(),
                seats = 2,
                destination = "",
                willReturn = true,
                carId = 1,
            };
            Exception? excepiton = null;

            //Act
            try {
                var result = await _repository.Create(dto, 1);
            } catch(Exception e) {
                excepiton = e;
            }

            //Assert
            Assert.NotNull(excepiton);
        }

        [Fact]
        public async void create_with_arrivalDate_inbetween_other_reservations_throws_exception() {
            //Arrange
            var user = await fillDbForCreateAndGetUser();

            ReservationDto dto = new ReservationDto() {
                departureDate = DateTime.Now.AddDays(1).AddHours(1).ToString(),
                arrivalDate = DateTime.Now.AddDays(2).AddHours(2).ToString(),
                seats = 2,
                destination = "",
                willReturn = true,
                carId = 1,
            };
            Exception? excepiton = null;

            //Act
            try {
                var result = await _repository.Create(dto, 1);
            } catch (Exception e) {
                excepiton = e;
            }

            //Assert
            Assert.NotNull(excepiton);
        }

        [Fact]
        public async void create_with_arrivalDate_and_departureDate_inbetween_other_reservations_throws_exception() {
            //Arrange
            var user = await fillDbForCreateAndGetUser();

            ReservationDto dto = new ReservationDto() {
                departureDate = DateTime.Now.AddHours(1).ToString(),
                arrivalDate = DateTime.Now.AddDays(2).ToString(),
                seats = 2,
                destination = "",
                willReturn = true,
                carId = 1,
            };
            Exception? excepiton = null;

            //Act
            try {
                var result = await _repository.Create(dto, 1);
            } catch (Exception e) {
                excepiton = e;
            }

            //Assert
            Assert.NotNull(excepiton);
        }

        [Fact]
        public async void create_with_unknown_user_returns_exceptions() {
            var user = await fillDbForCreateAndGetUser();

            ReservationDto dto = new ReservationDto() {
                departureDate = DateTime.Now.AddDays(12).AddHours(1).ToString(),
                arrivalDate = DateTime.Now.AddDays(12).AddDays(2).ToString(),
                seats = 2,
                destination = "",
                willReturn = true,
                carId = 1,
            };
            Exception? excepiton = null;

            //Act
            try {
                var result = await _repository.Create(dto, 12);
            } catch (Exception e) {
                excepiton = e;
            }

            //Assert
            Assert.NotNull(excepiton);
        }

        [Fact]
        public async void create_with_unknown_car_returns_exceptions() {
            var user = await fillDbForCreateAndGetUser();

            ReservationDto dto = new ReservationDto() {
                departureDate = DateTime.Now.AddDays(12).AddHours(1).ToString(),
                arrivalDate = DateTime.Now.AddDays(12).AddDays(2).ToString(),
                seats = 2,
                destination = "",
                willReturn = true,
                carId = 12,
            };
            Exception? excepiton = null;

            //Act
            try {
                var result = await _repository.Create(dto, 1);
            } catch (Exception e) {
                excepiton = e;
            }

            //Assert
            Assert.NotNull(excepiton);
        }

        [Fact]
        public async void create_with_data_model_error_returns_exceptions() {
            var user = await fillDbForCreateAndGetUser();

            ReservationDto dto = new ReservationDto() {
                departureDate = DateTime.Now.AddDays(12).AddHours(1).ToString(),
                arrivalDate = DateTime.Now.AddDays(12).AddDays(2).ToString(),
                seats = 12,
                destination = "",
                willReturn = true,
                carId = 1,
            };
            Exception? excepiton = null;

            //Act
            try {
                var result = await _repository.Create(dto, 1);
            } catch (Exception e) {
                excepiton = e;
            }

            //Assert
            Assert.NotNull(excepiton);
        }

        [Fact]
        public async void create_with_wrong_departure_date_returns_exceptions() {
            var user = await fillDbForCreateAndGetUser();

            ReservationDto dto = new ReservationDto() {
                departureDate = "",
                arrivalDate = DateTime.Now.AddDays(12).AddDays(2).ToString(),
                seats = 2,
                destination = "",
                willReturn = true,
                carId = 1,
            };
            Exception? excepiton = null;

            //Act
            try {
                var result = await _repository.Create(dto, 1);
            } catch (Exception e) {
                excepiton = e;
            }

            //Assert
            Assert.NotNull(excepiton);
        }

        [Fact]
        public async void create_with_wrong_arrival_date_returns_exceptions() {
            var user = await fillDbForCreateAndGetUser();

            ReservationDto dto = new ReservationDto() {
                departureDate = DateTime.Now.AddDays(12).AddDays(2).ToString(),
                arrivalDate = "ss",
                seats = 2,
                destination = "",
                willReturn = true,
                carId = 1,
            };
            Exception? excepiton = null;

            //Act
            try {
                var result = await _repository.Create(dto, 1);
            } catch (Exception e) {
                excepiton = e;
            }

            //Assert
            Assert.NotNull(excepiton);
        }

        [Fact]
        public async void create_correct_info_adds_to_db() {
            var user = await fillDbForCreateAndGetUser();

            ReservationDto dto = new ReservationDto() {
                departureDate = DateTime.Now.AddDays(12).ToString(),
                arrivalDate = DateTime.Now.AddDays(12).AddHours(12).ToString(),
                seats = 2,
                destination = "",
                willReturn = true,
                carId = 1,
            };
            Exception? excepiton = null;

            //Act
            Reservation? result = null;
            try {
                result = await _repository.Create(dto, user.id);
            } catch (Exception e) {
                excepiton = e;
            }

            //Assert
            Assert.Null(excepiton);
            Assert.NotNull(result);
            Assert.Equal(user, result.user);
            Assert.Contains(result, _context.Reservations);
        }

        [Fact]
        public async void delete_with_not_existing_entry_gives_error() {
            //Arrange            
            Exception? excepiton = null;

            //Act
            try {
                await _repository.Delete(1);
            } catch (Exception e) {
                excepiton = e;
            }

            //Assert
            Assert.NotNull(excepiton);
        }

        [Fact]
        public async void delete_with_existing_id_deleted_entry() {
            //Arrange  
            await fillDbForCreateAndGetUser();
            Exception? excepiton = null;

            //Act
            try {
                await _repository.Delete(1);
            } catch (Exception e) {
                excepiton = e;
            }

            //Assert
            Assert.Null(excepiton);
            Assert.Null(_context.Reservations.Find(1));
        }

        [Fact]
        public async void delete_with_existing_id_deleted_entry_and_connected_requests() {
            //Arrange  
            var user = await fillDbForCreateAndGetUser();
            var reservation = _context.Reservations.Find(1);

            Request[] requests = {
                new Request {
                    id = 1,
                    seats = 1,
                    status = Status.ACCEPTED,
                    creationDate = DateTime.Now.AddMinutes(1),
                    reservation = reservation,
                    user = user
                },
            };

            _context.Requests.AddRange(requests);
            await _context.SaveChangesAsync();

            Exception? excepiton = null;

            //Act
            try {
                await _repository.Delete(1);
            } catch (Exception e) {
                excepiton = e;
            }

            //Assert
            Assert.Null(excepiton);
            Assert.Null(_context.Reservations.Find(1));
            Assert.Null(_context.Requests.Find(1));
        }


        private async Task<User> fillDbForCreateAndGetUser() {
            Car[] cars = {
                new Car {
                    brand = "",
                    id = 1,
                    imageUrl = "",
                    isAvailable = true,
                    licensePlate = "999-XX-9",
                    location = "",
                    model = "",
                    seats = 5
                },
            };

            User[] users = {
                new User {
                    id = 1,
                    name = "Test",
                    emailAddress = "Email@email.nl",
                    employeeNr = "123",
                    hasLicense = true,
                    role = Role.USER
                },
                new User {
                    id = 2,
                    name = "Test",
                    emailAddress = "Email2@email.nl",
                    employeeNr = "123",
                    hasLicense = true,
                    role = Role.USER
                },
            };

            Reservation[] reservations = {
                new Reservation{
                    id = 1,
                    departureDate = DateTime.Now.AddMinutes(1),
                    arrivalDate = DateTime.Now.AddHours(12),
                    destination = "",
                    willReturn = true,
                    car = cars[0],
                    seats = 2,
                    user = users[0]
                },
                new Reservation{
                    id = 2,
                    departureDate = DateTime.Now.AddDays(2),
                    arrivalDate = DateTime.Now.AddDays(2).AddHours(12),
                    destination = "",
                    willReturn = true,
                    car = cars[0],
                    seats = 2,
                    user = users[1]
                },
            };

            _context.Cars.AddRange(cars);
            _context.Users.AddRange(users);
            _context.Reservations.AddRange(reservations);
            await _context.SaveChangesAsync();

            return users[0];
        }
    }
}
