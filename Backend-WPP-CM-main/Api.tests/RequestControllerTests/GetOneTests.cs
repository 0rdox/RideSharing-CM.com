using API.Controllers;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using NSubstitute.ReturnsExtensions;

namespace Api.tests.RequestControllerTests {
    public class GetOneTests : IDisposable {
        private IRequestRepository repository;
        private UserManager<IdentityUser> userManager;
        private IUserRepository userRepository;
        private IReservationRepository reservationRepository;
        private IEmailService emailService;
        private IConfiguration configuration;
        private SecurityDbContext _securityContext { get; set; }
        private RequestController controller;

        public GetOneTests() {
            repository = Substitute.For<IRequestRepository>();
            userRepository = Substitute.For<IUserRepository>();
            reservationRepository = Substitute.For<IReservationRepository>();
            configuration = Substitute.For<IConfiguration>();
            emailService = Substitute.For<IEmailService>();

            //setup security db
            var securityOptionsBuilder = new DbContextOptionsBuilder<SecurityDbContext>();
            securityOptionsBuilder.UseInMemoryDatabase(databaseName: "SecurityDb6");

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
        public void get_witch_returns_null_returns_NOTFOUND() {
            //Arrange
            repository.GetById(Arg.Any<int>()).ReturnsNull();

            //Act
            var result = controller.GetOne(1);

            //Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.NotNull(notFoundResult);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public void get_witch_returns_object_returns_OK() {
            //Arrange
            repository.GetById(Arg.Any<int>()).Returns(new Request {
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

            //Act
            var result = controller.GetOne(1);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);

            var model = (Object)okResult.Value;
            var data = GetDataFromReturnObject(model, "data");
            Assert.NotNull(data!);
        }
    }
}
