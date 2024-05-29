using API.Controllers;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using DonainModel;

namespace Api.tests.RequestControllerTests {
    public class GetAllTests : IDisposable {
        private IRequestRepository repository;
        private UserManager<IdentityUser> userManager;
        private IUserRepository userRepository;
        private IReservationRepository reservationRepository;
        private IEmailService emailService;
        private IConfiguration configuration;
        private SecurityDbContext _securityContext { get; set; }
        private RequestController controller;

        public GetAllTests() {
            repository = Substitute.For<IRequestRepository>();
            userRepository = Substitute.For<IUserRepository>();
            reservationRepository = Substitute.For<IReservationRepository>();
            configuration = Substitute.For<IConfiguration>();
            emailService = Substitute.For<IEmailService>();

            //setup security db
            var securityOptionsBuilder = new DbContextOptionsBuilder<SecurityDbContext>();
            securityOptionsBuilder.UseInMemoryDatabase(databaseName: "SecurityDb5");

            //set up security context
            _securityContext = new SecurityDbContext(securityOptionsBuilder.Options);

            //set up user manager
            var userStore = new UserStore<IdentityUser>(_securityContext);
            var userValidator = new UserValidator<IdentityUser>();
            var passwordValidator = new PasswordValidator<IdentityUser>();
            var passwordHasher = new PasswordHasher<IdentityUser>();
            var userValidators = new List<IUserValidator<IdentityUser>> { userValidator };
            var passwordValidators = new List<IPasswordValidator<IdentityUser>> { passwordValidator };
            var logger = new LoggerFactory().CreateLogger<UserManager<IdentityUser>>();
            // set up role manager
            var roleStore = new RoleStore<IdentityRole>(_securityContext);
            var roleValidator = new RoleValidator<IdentityRole>();
            var roleValidators = new List<IRoleValidator<IdentityRole>> { roleValidator };
            var loggerRoleManager = new LoggerFactory().CreateLogger<RoleManager<IdentityRole>>();
            var roleManager = new RoleManager<IdentityRole>(roleStore, roleValidators, null, null, loggerRoleManager);

            // seed roles
            var roles = new[] { "ADMIN", "USER" };
            foreach (var role in roles) {
                if (!roleManager.RoleExistsAsync(role).Result) {
                    roleManager.CreateAsync(new IdentityRole(role)).Wait();
                }
            }

            userManager = new UserManager<IdentityUser>(userStore, null, passwordHasher, userValidators,
                passwordValidators, null, null, null, logger);

            //set up controller
            controller = new RequestController(repository, userManager, userRepository, configuration, reservationRepository, emailService);
        }

        public void Dispose() {
            _securityContext.ChangeTracker.Clear();
            _securityContext.Database.EnsureDeleted();
            _securityContext.Dispose();
        }

        private static Object? GetDataFromReturnObject(Object obj, string property) {
            return obj.GetType().GetProperties().First(i => i.Name == property).GetValue(obj, null);
        }

        [Fact]
        public void get_all_with_reservationId_filter_parse_bad_retusn_bad_request() {
            //Arrange

            //Act
            var result = controller.GetAll("id", null, null, null, null);

            //Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badResult);
            Assert.Equal(400, badResult.StatusCode);
        }

        [Fact]
        public void get_all_with_requestStatus_filter_parse_bad_retusn_bad_request() {
            //Arrange

            //Act
            var result = controller.GetAll(null, "Status", null, null, null);

            //Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badResult);
            Assert.Equal(400, badResult.StatusCode);
        }

        [Fact]
        public void get_all_with_creationDateStart_filter_parse_bad_retusn_bad_request() {
            //Arrange

            //Act
            var result = controller.GetAll(null, null, "Date", null, null);

            //Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badResult);
            Assert.Equal(400, badResult.StatusCode);
        }

        [Fact]
        public void get_all_with_creationDateEnd_filter_parse_bad_retusn_bad_request() {
            //Arrange

            //Act
            var result = controller.GetAll(null, null, null, "Date", null);

            //Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badResult);
            Assert.Equal(400, badResult.StatusCode);
        }

        [Fact]
        public void get_all_with_userId_filter_parse_bad_retusn_bad_request() {
            //Arrange

            //Act
            var result = controller.GetAll(null, null, null, null, "Id");

            //Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badResult);
            Assert.Equal(400, badResult.StatusCode);
        }

        [Fact]
        public void get_all_without_filters_returns_OK() {
            //Arrange
            repository.GetAll(null, null, null, null, null)
                .Returns(new List<Request>() {
                    new Request {
                        seats = 1,
                        status = Status.ACCEPTED,
                        id = 1,
                        creationDate = DateTime.Now.AddMinutes(1),
                        token = null,
                        reservation = new Reservation {
                            id = 1,
                            departureDate = DateTime.Now.AddMinutes(1),
                            arrivalDate = DateTime.Now.AddDays(1),
                            destination = "",
                            willReturn = true,
                            seats = 2,
                            car = new Car {
                                id = 1,
                                licensePlate = "LL-LL-LL",
                                brand = "",
                                model = "",
                                seats = 4,
                                imageUrl = "",
                                isAvailable = true,
                                location = ""
                            },
                            user = new User {
                                id = 1,
                                emailAddress = "email@email.nl",
                                name = "",
                                employeeNr = "",
                                hasLicense = true,
                                role = Role.USER,
                            },
                        },
                        user = new User {
                            id = 1,
                            emailAddress = "email@email.nl",
                            name = "",
                            employeeNr = "",
                            hasLicense = true,
                            role = Role.USER,
                        },
                    }
                }.AsEnumerable());

            //Act
            var result = controller.GetAll(null, null, null, null, null);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);

            var model = (Object)okResult.Value;
            var data = (List<Object>) GetDataFromReturnObject(model, "data");
            Assert.NotEmpty(data!);
        }

        [Fact]
        public void get_all_with_all_correct_filters_returns_OK() {
            //Arrange
            repository.GetAll(Arg.Any<int>(), Arg.Any<Status>(), Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<int>())
                .Returns(new List<Request>() {
                    new Request {
                        seats = 1,
                        status = Status.ACCEPTED,
                        id = 1,
                        creationDate = DateTime.Now.AddMinutes(1),
                        token = null,
                        reservation = new Reservation {
                            id = 1,
                            departureDate = DateTime.Now.AddMinutes(1),
                            arrivalDate = DateTime.Now.AddDays(1),
                            destination = "",
                            willReturn = true,
                            seats = 2,
                            car = new Car {
                                id = 1,
                                licensePlate = "LL-LL-LL",
                                brand = "",
                                model = "",
                                seats = 4,
                                imageUrl = "",
                                isAvailable = true,
                                location = ""
                            },
                            user = new User {
                                id = 1,
                                emailAddress = "email@email.nl",
                                name = "",
                                employeeNr = "",
                                hasLicense = true,
                                role = Role.USER,
                            },
                        },
                        user = new User {
                            id = 1,
                            emailAddress = "email@email.nl",
                            name = "",
                            employeeNr = "",
                            hasLicense = true,
                            role = Role.USER,
                        },
                    }
                }.AsEnumerable());

            //Act
            var result = controller.GetAll("1", "ACCEPTED", DateTime.Now.ToString(), DateTime.Now.ToString(), "1");

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);

            var model = (Object)okResult.Value;
            var data = (List<Object>)GetDataFromReturnObject(model, "data");
            Assert.NotEmpty(data!);
        }

        [Fact]
        public void get_all_without_filters_and_empty_list_returns_OK() {
            //Arrange
            repository.GetAll(Arg.Any<int>(), Arg.Any<Status>(), Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<int>())
                .Returns(new List<Request>().AsEnumerable());

            //Act
            var result = controller.GetAll(null, null, null, null, null);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);

            var model = (Object)okResult.Value;
            var data = (List<Object>)GetDataFromReturnObject(model, "data");
            Assert.Empty(data!);
        }

        //get all

    }
}
