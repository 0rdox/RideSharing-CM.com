using API.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Api.tests.UserControllerTests {
    public class GetAllTests {
        private IUserRepository repository;
        private UserController controller;

        public GetAllTests() {
            repository = Substitute.For<IUserRepository>();
            controller = new UserController(repository);
        }

        private static Object? GetDataFromReturnObject(Object obj, string property) {
            return obj.GetType().GetProperties().First(i => i.Name == property).GetValue(obj, null);
        }

        [Fact]
        public void returns_all() {
            //Arrange
            repository.GetAll().Returns(new List<User> { });

            //Act
            var result = controller.GetAll();

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
