using API.Controllers;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using NSubstitute.ExceptionExtensions;

namespace Api.tests.RequestControllerTests {
    public class DeleteTests : IDisposable {
        private IRequestRepository repository;
        private UserManager<IdentityUser> userManager;
        private IUserRepository userRepository;
        private IReservationRepository reservationRepository;
        private IEmailService emailService;
        private IConfiguration configuration;
        private SecurityDbContext _securityContext { get; set; }
        private RequestController controller;

        public DeleteTests() {
            repository = Substitute.For<IRequestRepository>();
            userRepository = Substitute.For<IUserRepository>();
            reservationRepository = Substitute.For<IReservationRepository>();
            configuration = Substitute.For<IConfiguration>();
            emailService = Substitute.For<IEmailService>();

            //setup security db
            var securityOptionsBuilder = new DbContextOptionsBuilder<SecurityDbContext>();
            securityOptionsBuilder.UseInMemoryDatabase(databaseName: "SecurityDb9");

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
        public async void delete_thorws_exceptions_returns_BADREQUEST() {
            //Arrange
            repository.Delete(Arg.Any<int>()).Throws(new Exception());

            //Act
            var result = await controller.Delete(1);

            //Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badResult);
            Assert.Equal(400, badResult.StatusCode);
        }


        [Fact]
        public async void delete_with_no_exceptions_returns_OK() {
            //Arrange
 
            //Act
            var result = await controller.Delete(1);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);

        }
    }
}
