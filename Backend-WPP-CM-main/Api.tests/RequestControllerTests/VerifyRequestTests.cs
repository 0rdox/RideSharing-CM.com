using API.Controllers;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using NSubstitute.ExceptionExtensions;
using Infrastructure.Repositories;
using Infrastructure;

namespace Api.tests.RequestControllerTests {
    public class VerifyRequestTests : IDisposable {
        private IRequestRepository repository;
        private UserManager<IdentityUser> userManager;
        private IUserRepository userRepository;
        private IReservationRepository reservationRepository;
        private IEmailService emailService;
        private IConfiguration configuration;
        private SecurityDbContext _securityContext { get; set; }
        private RequestController controller;

        public VerifyRequestTests() {
            repository = Substitute.For<IRequestRepository>();
            userRepository = Substitute.For<IUserRepository>();
            reservationRepository = Substitute.For<IReservationRepository>();
            configuration = Substitute.For<IConfiguration>();
            emailService = Substitute.For<IEmailService>();

            //setup security db
            var securityOptionsBuilder = new DbContextOptionsBuilder<SecurityDbContext>();
            securityOptionsBuilder.UseInMemoryDatabase(databaseName: "SecurityDb8");

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
        public async void verify_with_wrong_status_query_returns_BADREQUEST() {
            //Arrange

            //Act
            var result = await controller.verifyRequest(1, "Token", "StatusWrong");

            //Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badResult);
            Assert.Equal(400, badResult.StatusCode);
        }

        [Fact]
        public async void verify_with_token_query_is_null_returns_UNAUTHORIZED() {
            //Arrange

            //Act
            var result = await controller.verifyRequest(1, null, "ACCEPTED");

            //Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.NotNull(unauthorizedResult);
            Assert.Equal(401, unauthorizedResult.StatusCode);
        }

        [Fact]
        public async void verify_repo_func_thows_exception_with_wrong_verify_token_returns_UNAUTHORIZED() {
            //Arrange
            repository.VerifyRequest(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<Status>())
                .Throws(new Exception(RequestRepository.wrongVerifyToken));

            //Act
            var result = await controller.verifyRequest(1, "token", "ACCEPTED");

            //Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.NotNull(unauthorizedResult);
            Assert.Equal(401, unauthorizedResult.StatusCode);
        }

        [Fact]
        public async void verify_repo_func_thows_other_exception_than_wrong_verify_token_returns_Bad_request() {
            //Arrange
            repository.VerifyRequest(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<Status>())
                .Throws(new Exception("Other message"));

            //Act
            var result = await controller.verifyRequest(1, "token", "ACCEPTED");

            //Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badResult);
            Assert.Equal(400, badResult.StatusCode);
        }

        [Fact]
        public async void verify_repo_thows_no_exception_returns_OK() {
            //Arrange
            repository.VerifyRequest(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<Status>())
                .Returns(new Request {
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
                });

            reservationRepository.GetById(Arg.Any<int>())
                .Returns(new Reservation {
                    id = 1,
                    departureDate = DateTime.Now.AddMinutes(1),
                    arrivalDate = DateTime.Now.AddDays(1),
                    destination = "",
                    willReturn = true,
                    seats = 2,
                    user = new User {
                        id = 1,
                        emailAddress = "email@email.nl",
                        name = "",
                        employeeNr = "",
                        hasLicense = true,
                        role = Role.USER,
                    },
                    car = new Car {
                        id = 1,
                        licensePlate = "LL-LL-LL",
                        brand = "",
                        model = "",
                        seats = 4,
                        imageUrl = "",
                        isAvailable = true,
                        location = ""
                    }
                });

            emailService.SendRequestStatusAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<Status>(), Arg.Any<string>(), Arg.Any<string>())
                .Returns(Task.FromResult(true));

            //Act
            var result = await controller.verifyRequest(1, "token", "ACCEPTED");

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
        }
    }
}
