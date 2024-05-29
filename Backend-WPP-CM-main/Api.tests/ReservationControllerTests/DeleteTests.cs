using API.Controllers;
using API.Services;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using NSubstitute.ReturnsExtensions;
using NSubstitute.ExceptionExtensions;
using System.Globalization;
using DonainModel;

namespace Api.tests.ReservationControllerTests {
    public class DeleteTests : IDisposable {
        private IReservationRepository repository;
        private UserManager<IdentityUser> userManager;
        private IUserRepository userRepository;
        private IEmailService emailService;
        private SecurityDbContext _securityContext { get; set; }
        private ReservationController controller;

        public DeleteTests() {
            repository = Substitute.For<IReservationRepository>();
            userRepository = Substitute.For<IUserRepository>();
            emailService = Substitute.For<IEmailService>();
          
            //setup security db
            var securityOptionsBuilder = new DbContextOptionsBuilder<SecurityDbContext>();
            securityOptionsBuilder.UseInMemoryDatabase(databaseName: "SecurityDb4");

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

        private static Object? GetDataFromReturnObject(Object obj, string property) {
            return obj.GetType().GetProperties().First(i => i.Name == property).GetValue(obj, null);
        }

        [Fact]
        public async void delete_with_id_of_not_exising_reservation_returns_not_found() {
            //Arrange
            repository.GetById(1).ReturnsNull();
           
            //Act
            var result = await controller.Delete(1);

            //Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.NotNull(notFoundResult);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public async void delete_where_func_throws_exception_returns_bad_request() {
            //Arrange
            repository.GetById(1).Returns(new Reservation {
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
            emailService.SendDeletionEmailAsync(Arg.Any<string[]>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).ReturnsForAnyArgs(Task.FromResult(true));

            repository.Delete(Arg.Any<int>()).Throws(new Exception());

            //Act
            var result = await controller.Delete(1);

            //Assert
            var baResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(baResult);
            Assert.Equal(400, baResult.StatusCode);
        }

        [Fact]
        public async void delete_succes_sent_ok() {
            //Arrange
            repository.GetById(1).Returns(new Reservation {
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
            emailService.SendDeletionEmailAsync(Arg.Any<string[]>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).ReturnsForAnyArgs(Task.FromResult(true));

            //Act
            var result = await controller.Delete(1);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
        }
    }
}
