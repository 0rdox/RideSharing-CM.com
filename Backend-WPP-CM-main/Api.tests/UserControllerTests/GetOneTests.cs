using API.Controllers;
using Microsoft.AspNetCore.Mvc;
using NSubstitute.ReturnsExtensions;
using NSubstitute;

namespace Api.tests.UserControllerTests {
    public class GetOneTests {
        private IUserRepository repository;
        private UserController controller;

        public GetOneTests() {
            repository = Substitute.For<IUserRepository>();
            controller = new UserController(repository);
        }

        private static Object? GetDataFromReturnObject(Object obj, string property) {
            return obj.GetType().GetProperties().First(i => i.Name == property).GetValue(obj, null);
        }

        [Fact]
        public void get_with_id_of_not_exising_car_returns_not_found() {
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
        public void get_with_id_of_exising_car_returns_car_with_OK() {
            //Arrange
            repository.GetById(Arg.Any<int>()).Returns(new User { 
                emailAddress = "test@test.nl",
                id=1,
                securityId = "",
                employeeNr = "1",
                hasLicense = true,
                name = "",
                role = Role.USER
            });

            //Act
            var result = controller.GetOne(1);

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
