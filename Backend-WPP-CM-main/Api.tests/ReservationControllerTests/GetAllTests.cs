using API.Controllers;
using API.Services;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace Api.tests.ReservationControllerTests {
    public class GetAllTests : IDisposable {
        private IReservationRepository repository;
        private UserManager<IdentityUser> userManager;
        private IUserRepository userRepository;
        private IEmailService emailService;
        private SecurityDbContext _securityContext { get; set; }
        private ReservationController controller;

        public GetAllTests() {
            repository = Substitute.For<IReservationRepository>();
            userRepository = Substitute.For<IUserRepository>();
            emailService = Substitute.For<IEmailService>();
            
            //setup security db
            var securityOptionsBuilder = new DbContextOptionsBuilder<SecurityDbContext>();
            securityOptionsBuilder.UseInMemoryDatabase(databaseName: "SecurityDb2");

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
            controller = new ReservationController(userManager, repository, userRepository, emailService);
        }

        public void Dispose() {
            _securityContext.ChangeTracker.Clear();
            _securityContext.Database.EnsureDeleted();
            _securityContext.Dispose();
        }

        private static Object? GetDataFromReturnObject(Object obj, string propertie) {
            return obj.GetType().GetProperties().First(i => i.Name == propertie).GetValue(obj, null);
        }

        [Fact]
        public void get_with_wrong_beginDate_filter_returns_bad_request() {
            //Arrange

            //Act
            var result = controller.Get(null, null, "sfd", "sf");

            //Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badResult);
            Assert.NotNull(badResult.Value);
            Assert.Equal(400, badResult.StatusCode);

        }

        [Fact]
        public void get_with_wrong_endDate_filter_returns_bad_request() {
            //Arrange

            //Act
            var result = controller.Get(null, null,null, "sdf");

            //Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badResult);
            Assert.NotNull(badResult.Value);
            Assert.Equal(400, badResult.StatusCode);

        }

        [Fact]
        public void get_with_wrong_userId_filter_returns_bad_request() {
            //Arrange

            //Act
            var result = controller.Get("sdf", null, null, null);

            //Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badResult);
            Assert.NotNull(badResult.Value);
            Assert.Equal(400, badResult.StatusCode);

        }

        [Fact]
        public void get_with_wrong_carId_filter_returns_bad_request() {
            //Arrange

            //Act
            var result = controller.Get(null, "sf", null, null);

            //Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badResult);
            Assert.NotNull(badResult.Value);
            Assert.Equal(400, badResult.StatusCode);

        }

        [Fact]
        public void get_with_good_filter_returns_OK() {
            //Arrange
            repository.GetAll(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<DateTime>(), Arg.Any<DateTime>())
                .Returns(new List<Reservation>{
                    new Reservation {
                        id = 1,
                        departureDate = DateTime.Now.AddMinutes(1),
                        arrivalDate = DateTime.Now.AddDays(1),
                        destination = "",
                        willReturn = true,
                        seats = 2,
                        user = new User {
                            id= 1,
                            emailAddress = "email@email.nl",
                            name = "",
                            employeeNr = "",
                            hasLicense = true,
                            role = Role.USER,
                        },
                        car = new Car {
                            id= 1,
                            licensePlate = "LL-LL-LL",
                            brand = "",
                            model = "",
                            seats = 4,
                            imageUrl = "",
                            isAvailable = true,
                            location = ""
                        }
                    }
                }
            );

            //Act
            var result = controller.Get("1", "2", new DateTime().ToString(), new DateTime().ToString());

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult);
            Assert.NotNull(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);

            var model = (Object)okResult.Value;
            var data = GetDataFromReturnObject(model, "data");
            Assert.NotNull(data);
        }

        [Fact]
        public void get_with_no_filter_returns_OK() {
            //Arrange
            repository.GetAll(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<DateTime>(), Arg.Any<DateTime>())
                .Returns(new List<Reservation>{
                    new Reservation {
                        id = 1,
                        departureDate = DateTime.Now.AddMinutes(1),
                        arrivalDate = DateTime.Now.AddDays(1),
                        destination = "",
                        willReturn = true,
                        seats = 2,
                        user = new User {
                            id= 1,
                            emailAddress = "email@email.nl",
                            name = "",
                            employeeNr = "",
                            hasLicense = true,
                            role = Role.USER,
                        },
                        car = new Car {
                            id= 1,
                            licensePlate = "LL-LL-LL",
                            brand = "",
                            model = "",
                            seats = 4,
                            imageUrl = "",
                            isAvailable = true,
                            location = ""
                        }
                    }
                }
            );

            //Act
            var result = controller.Get(null, null, null, null);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult);
            Assert.NotNull(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);

            var model = (Object)okResult.Value;
            var data = GetDataFromReturnObject(model, "data");
            Assert.NotNull(data);
        }
    }
}
