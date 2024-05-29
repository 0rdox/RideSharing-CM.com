using API.Controllers;
using DonainModel.dto;
using Microsoft.AspNetCore.Mvc;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;

namespace Api.tests.UserControllerTests {
    public class UpdateTests {
        private IUserRepository repository;
        private UserController controller;

        public UpdateTests() {
            repository = Substitute.For<IUserRepository>();
            controller = new UserController(repository);
        }

        private static Object? GetDataFromReturnObject(Object obj, string property) {
            return obj.GetType().GetProperties().First(i => i.Name == property).GetValue(obj, null);
        }

        [Fact]
        public async void create_with_bad_dto_returns_bad_request() {
            //Arrange
            repository.Create(Arg.Any<User>(), Arg.Any<String>()).Returns(new User {
                emailAddress = "test@test.nl",
                id = 1,
                securityId = "",
                employeeNr = "1",
                hasLicense = true,
                name = "",
                role = Role.USER,
            });
            UserDto dto = new UserDto() {
                employeeNr = ""
            };

            //Act
            var result = await controller.Create(dto);

            //Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badResult);
            Assert.NotNull(badResult.Value);
            Assert.Equal(400, badResult.StatusCode);
        }

        [Fact]
        public async void create_with_wrong_role_returns_bad_request() {
            //Arrange
            repository.Update(Arg.Any<int>(),Arg.Any<User>()).Returns(new User {
                emailAddress = "test@test.nl",
                id = 1,
                securityId = "",
                employeeNr = "1",
                hasLicense = true,
                name = "",
                role = Role.USER
            });
            UserUpdateDto dto = new UserUpdateDto() {
                employeeNr = "",
                hasLicense = true,
                emailAddress = "test@test.nl",
                name = "",
                role = "fasf"
            };

            //Act
            var result = await controller.Update(1, dto);

            //Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badResult);
            Assert.NotNull(badResult.Value);
            Assert.Equal(400, badResult.StatusCode);
        }

        [Fact]
        public async void create_with_exception_thrown_at_repo_returns_internal_server_error() {
            //Arrange
            repository.Update(Arg.Any<int>(), Arg.Any<User>()).Throws(new Exception());
            UserUpdateDto dto = new UserUpdateDto() {
                employeeNr = "",
                hasLicense = true,
                emailAddress = "test@test.nl",
                name = "",
                role = "USER"
            };

            //Act
            var result = await controller.Update(1, dto);

            //Assert
            var serverErrorResult = Assert.IsType<ObjectResult>(result);
            Assert.NotNull(serverErrorResult);
            Assert.NotNull(serverErrorResult.Value);
            Assert.Equal(500, serverErrorResult.StatusCode);
        }

        [Fact]
        public async void create_with_repo_returns_null_returns_not_found() {
            //Arrange
            repository.Update(Arg.Any<int>(), Arg.Any<User>()).ReturnsNull();
            UserUpdateDto dto = new UserUpdateDto() {
                employeeNr = "",
                hasLicense = true,
                emailAddress = "test@test.nl",
                name = "",
                role = "USER"
            };

            //Act
            var result = await controller.Update(1, dto);

            //Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.NotNull(notFoundResult);
            Assert.NotNull(notFoundResult.Value);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public async void create_correct_returns_OK() {
            //Arrange
            repository.Update(Arg.Any<int>(), Arg.Any<User>()).Returns(new User {
                emailAddress = "test@test.nl",
                id = 1,
                securityId = "",
                employeeNr = "1",
                hasLicense = true,
                name = "",
                role = Role.USER,
            });
            UserUpdateDto dto = new UserUpdateDto() {
                employeeNr = "",
                hasLicense = true,
                emailAddress = "test@test.nl",
                name = "",
                role = "USER"
            };

            //Act
            var result = await controller.Update(1, dto);

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
