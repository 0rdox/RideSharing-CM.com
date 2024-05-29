using DomainServices;
using DonainModel;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.tests {
    public class RequestRepositoryTest : IDisposable {
        private AppDbContext _context { get; set; }
        private IRequestRepository _repository { get; set; }

        public RequestRepositoryTest() {
            //setup db
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseInMemoryDatabase(databaseName: "RequestDb");

            //set up context
            _context = new AppDbContext(optionsBuilder.Options);

            //set up repo
            _repository = new RequestRepository(_context);
        }

        public void Dispose() {
            _context.ChangeTracker.Clear();
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Theory]
        [InlineData(500)]
        [InlineData(-500)]
        public async void delete_with_not_found_request_gives_error(int id) {
            //Arrange
            await fillDbForCreateAndGetUser();
            Exception? exception = null;

            //Act
            try {
                await _repository.Delete(id);
            } catch (Exception e) {
                exception = e;
            }

            //Assert
            Assert.NotNull(exception);
        }

        [Fact]
        public async void delete_with_found_request_gives_exception() {
            //Arrange
            await fillDbForCreateAndGetUser();
            Exception? exception = null;

            //Act
            try {
                await _repository.Delete(1);
            } catch (Exception e) {
                exception = e;
            }

            //Assert
            Assert.Null(exception);
        }


        [Fact]
        public async void get_all_without_filters_returns_all() {
            //Arrange
            await fillDbForCreateAndGetUser();

            //Act
            var result = _repository.GetAll();

            //Assert
            Assert.NotEmpty(result);
        }

        [Fact]
        public async void get_all_with_reservationId_filter_returns_all_with_filter() {
            //Arrange
            await fillDbForCreateAndGetUser();
            var reservationsId = 2;

            //Act
            var result = _repository.GetAll(reservationId: reservationsId);

            //Assert
            Assert.NotEmpty(result);
            Assert.Equal(1, result?.Count());
        }

        [Fact]
        public async void get_all_with_requestStatus_filter_returns_all_with_filter() {
            //Arrange
            await fillDbForCreateAndGetUser();
            var reservationsId = 2;

            //Act
            var result = _repository.GetAll(requestStatus: Status.PENDING);

            //Assert
            Assert.NotEmpty(result);
            Assert.Equal(2, result?.Count());
        }

        [Fact]
        public async void get_all_with_userId_filter_returns_all_with_filter() {
            //Arrange
            await fillDbForCreateAndGetUser();

            //Act
            var result = _repository.GetAll(userId: 1);

            //Assert
            Assert.NotEmpty(result);
            Assert.Equal(3, result?.Count());
        }


        [Fact]
        public async void get_all_with_requestStatus_and_reservationId_filter_returns_all_with_filter() {
            //Arrange
            await fillDbForCreateAndGetUser();
            var reservationsId = 2;

            //Act
            var result = _repository.GetAll(reservationId: 1,requestStatus: Status.PENDING);

            //Assert
            Assert.NotEmpty(result);
            Assert.Equal(1, result?.Count());
        }

        [Fact]
        public async void create_unknown_user_gives_error() {
            //Arrange
            await fillDbForCreateAndGetUser();
            Exception? exception = null;

            //Act
            try {
                var result = await _repository.Create(new DonainModel.dto.RequestDto {
                    seats = 1,
                    reservationId = 1,
                }, 100);
            } catch (Exception ex){
                exception = ex;
            }
 

            //Assert
            Assert.NotNull(exception);
        }

        [Fact]
        public async void create_unknown_reservation_gives_error() {
            //Arrange
            await fillDbForCreateAndGetUser();
            Exception? exception = null;

            //Act
            try {
                var result = await _repository.Create(new DonainModel.dto.RequestDto {
                    seats = 1,
                    reservationId = 100,
                }, 1);
            } catch (Exception ex) {
                exception = ex;
            }


            //Assert
            Assert.NotNull(exception);
        }

        [Fact]
        public async void create_with_to_much_seats_gives_error() {
            //Arrange
            await fillDbForCreateAndGetUser();
            Exception? exception = null;

            //Act
            try {
                var result = await _repository.Create(new DonainModel.dto.RequestDto {
                    seats = 5,
                    reservationId = 1,
                }, 1);
            } catch (Exception ex) {
                exception = ex;
            }


            //Assert
            Assert.NotNull(exception);
        }

        [Fact]
        public async void create_gives_no_error() {
            //Arrange
            await fillDbForCreateAndGetUser();
            Exception? exception = null;
            Request? result = null;

            //Act
            try {
                result = await _repository.Create(new DonainModel.dto.RequestDto {
                    seats = 2,
                    reservationId = 1,
                }, 1);
            } catch (Exception ex) {
                exception = ex;
            }


            //Assert
            Assert.Null(exception);
            Assert.NotNull(result);
        }

        [Fact]
        public async void verifyRequest_with_incorrect_requestId_gives_error() {
            //Arrange
            await fillDbForCreateAndGetUser();
            Exception? exception = null;
            Request? result = null;

            //Act
            try {
                result = await _repository.VerifyRequest(100, "token", Status.ACCEPTED);
            } catch (Exception ex) {
                exception = ex;
            }


            //Assert
            Assert.NotNull(exception);
            Assert.Null(result);
        }

        [Fact]
        public async void verifyRequest_with_requestId_of_already_verified_request_gives_error() {
            //Arrange
            await fillDbForCreateAndGetUser();
            Exception? exception = null;
            Request? result = null;

            //Act
            try {
                result = await _repository.VerifyRequest(2, "token", Status.ACCEPTED);
            } catch (Exception ex) {
                exception = ex;
            }


            //Assert
            Assert.NotNull(exception);
            Assert.Null(result);
        }

        [Fact]
        public async void verifyRequest_with_incorrect_token_gives_error() {
            //Arrange
            await fillDbForCreateAndGetUser();
            Exception? exception = null;
            Request? result = null;

            //Act
            try {
                result = await _repository.VerifyRequest(1, "tokenWrong", Status.ACCEPTED);
            } catch (Exception ex) {
                exception = ex;
            }


            //Assert
            Assert.NotNull(exception);
            Assert.Null(result);
        }

        [Fact]
        public async void verifyRequest_with_newStatus_of_PENDING_gives_error() {
            //Arrange
            await fillDbForCreateAndGetUser();
            Exception? exception = null;
            Request? result = null;

            //Act
            try {
                result = await _repository.VerifyRequest(1, "token", Status.PENDING);
            } catch (Exception ex) {
                exception = ex;
            }


            //Assert
            Assert.NotNull(exception);
            Assert.Null(result);
        }

        [Fact]
        public async void verifyRequest_with_to_many_seats_gives_error() {
            //Arrange
            var user = await fillDbForCreateAndGetUser();
            var reservation = _context.Reservations.Find(2);

            _context.Requests.Add(new Request { 
                id = 5, 
                status = Status.PENDING, 
                reservation = reservation, 
                seats = 10, 
                user = user, 
                token = "token" 
            });
            await _context.SaveChangesAsync();
           
            Exception? exception = null;
            Request? result = null;

            //Act
            try {
                result = await _repository.VerifyRequest(5, "token", Status.ACCEPTED);
            } catch (Exception ex) {
                exception = ex;
            }

            //Assert
            Assert.NotNull(exception);
            Assert.Null(result);
        }

        [Fact]
        public async void verifyRequest_with_correct_info_that_sets_to_ACCEPTED_returns_request() {
            //Arrange
            await fillDbForCreateAndGetUser();
            Exception? exception = null;
            Request? result = null;

            //Act
            try {
                result = await _repository.VerifyRequest(1, "token", Status.ACCEPTED);
            } catch (Exception ex) {
                exception = ex;
            }

            //Assert
            Assert.Null(exception);
            Assert.NotNull(result);
            Assert.Equal(Status.ACCEPTED, result.status);
            Assert.Null(result.token);
        }

        [Fact]
        public async void verifyRequest_with_correct_info_that_sets_to_DENIED_returns_request() {
            //Arrange
            await fillDbForCreateAndGetUser();
            Exception? exception = null;
            Request? result = null;

            //Act
            try {
                result = await _repository.VerifyRequest(1, "token", Status.DENIED);
            } catch (Exception ex) {
                exception = ex;
            }

            //Assert
            Assert.Null(exception);
            Assert.NotNull(result);
            Assert.Equal(Status.DENIED, result.status);
            Assert.Null(result.token);
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

            Request[] request = {
                new Request {
                    id=1,
                    seats=1,
                    status=Status.PENDING,
                    token = "token",
                    creationDate=DateTime.Now.AddMinutes(1),
                    reservation=reservations[0],
                    user=users[0]
                },
                new Request {
                    id=2,
                    seats=1,
                    token = null,
                    status=Status.ACCEPTED,
                    creationDate=DateTime.Now.AddMinutes(1),
                    reservation=reservations[0],
                    user=users[0]
                },
                new Request {
                    id=3,
                    seats=1,
                    status=Status.DENIED,
                    token = null,
                    creationDate=DateTime.Now.AddMinutes(1),
                    reservation=reservations[0],
                    user=users[0]
                },
                new Request {
                    id=4,
                    seats=2,
                    token = "token",
                    status=Status.PENDING,
                    creationDate=DateTime.Now.AddMinutes(1),
                    reservation=reservations[1],
                    user=users[1]
                }
            };


            _context.Cars.AddRange(cars);
            _context.Users.AddRange(users);
            _context.Reservations.AddRange(reservations);
            _context.Requests.AddRange(request); 

            await _context.SaveChangesAsync();

            return users[0];
        }
    }
}
