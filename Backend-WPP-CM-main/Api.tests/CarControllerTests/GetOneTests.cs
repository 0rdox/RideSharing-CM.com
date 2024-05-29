using API.Controllers;
using Microsoft.AspNetCore.Mvc;
using NSubstitute.ReturnsExtensions;

namespace Api.tests.CarControllerTests {
    public class GetOneTests {
        private ICarRepository repository;
        private CarController controller;

        public GetOneTests() {
            repository = Substitute.For<ICarRepository>();
            controller = new CarController(repository);
        }

        private static Object? GetDataFromReturnObject(Object obj) {
            return obj.GetType().GetProperties().First(i => i.Name == "data").GetValue(obj, null);
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
            repository.GetById(Arg.Any<int>()).Returns(new Car {
                id = 1,
                brand = "",
                imageUrl = "",
                isAvailable = true,
                licensePlate = "KD-D2-LL",
                location = "",
                model = "",
                seats = 5
            });

            //Act
            var result = controller.GetOne(1);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult);
            Assert.NotNull(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);

            var model = (Object)okResult.Value;
            var data = GetDataFromReturnObject(model);
            Assert.NotNull(data);
        }
    }
}
