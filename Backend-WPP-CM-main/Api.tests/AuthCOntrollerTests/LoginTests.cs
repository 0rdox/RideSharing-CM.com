using API.Controllers;
using DonainModel.dto;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Api.tests.AuthControllerTests {
    public class LoginTests : IDisposable {
        private IUserRepository repository;
        private UserManager<IdentityUser> userManager;        
        private IConfiguration configuration;
        private AuthController controller;
        private SecurityDbContext _securityContext { get; set; }

        public LoginTests() {
            //Nsubstitute interfaces
            repository = Substitute.For<IUserRepository>();
            configuration = Substitute.For<IConfiguration>();

            //setup security db
            var securityOptionsBuilder = new DbContextOptionsBuilder<SecurityDbContext>();
            securityOptionsBuilder.UseInMemoryDatabase(databaseName: "SecurityDb");

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

            //Create Controller
            controller = new AuthController(userManager, repository, configuration);
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
        public async void login_with_unknown_user_returns_unauthorized() {
            //Arrange
            LoginDto dto = new LoginDto() {
                email = "test@test.nl",
                password = "Password123!",
            };

            //Act
            var result = await controller.Login(dto);

            //Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.NotNull(unauthorizedResult);
            Assert.NotNull(unauthorizedResult.Value);
            Assert.Equal(401, unauthorizedResult.StatusCode);
        }

        [Fact]
        public async void login_with_wrong_password_returns_unauthorized() {
            //Arrange
            LoginDto dto = new LoginDto() {
                email = "test@test.nl",
                password = "PasswordWrong123!",
            };

            userManager.CreateAsync(new IdentityUser {
                UserName = "test@test.nl",
                Email = "test@test.nl"
            }, "Password123!").Wait();

            //Act
            var result = await controller.Login(dto);

            //Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.NotNull(unauthorizedResult);
            Assert.NotNull(unauthorizedResult.Value);
            Assert.Equal(401, unauthorizedResult.StatusCode);
        }

        [Fact]
        public async void login_with_correct_info_returns_token() {
            //Arrange
            LoginDto dto = new LoginDto() {
                email = "test2@test.nl",
                password = "Password123!",
            };

            userManager.CreateAsync(new IdentityUser {
                UserName = "test2@test.nl",
                Email = "test2@test.nl"
            }, "Password123!").Wait();

            configuration.GetSection("Jwt:Key").Value.Returns("Never Gonna Give You Up");
            configuration.GetSection("Jwt:Issuer").Value.Returns("Never Gonna Give You Up");
            configuration.GetSection("Jwt:Audience").Value.Returns("Never Gonna Give You Up");

            //Act
            var result = await controller.Login(dto);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult);
            Assert.NotNull(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);

            var model = (Object)okResult.Value;
            var data = GetDataFromReturnObject(model, "data");
            Assert.NotNull(data);
            var token =(string) GetDataFromReturnObject(data, "bearerToken");
            Assert.NotNull(token);
        }
    }
}
