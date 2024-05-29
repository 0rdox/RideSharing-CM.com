using API.Controllers;
using Microsoft.AspNetCore.Mvc;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;

namespace Api.tests.CarControllerTests {
    public class DeleteTests {
        private ICarRepository repository;
        private CarController controller;

        public DeleteTests() {
            repository = Substitute.For<ICarRepository>();
            controller = new CarController(repository);
        }

        private static Object? GetDataFromReturnObject(Object obj, string property) {
            return obj.GetType().GetProperties().First(i => i.Name == property).GetValue(obj, null);
        }

        [Fact]
        public async void delete_with_id_of_not_existing_car_returns_not_found() {
            //Arrange
            repository.GetById(Arg.Any<int>()).ReturnsNull();

            //Act
            var result = await controller.Delete(1);

            //Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.NotNull(notFoundResult);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public async void delete_with_id_of_existing_car_but_not_deleted_returns_INTERNALSERVERERROR() {
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
            var result = await controller.Delete(1);

            //Assert
            var serverErrorResult = Assert.IsType<ObjectResult>(result);
            Assert.NotNull(serverErrorResult);
            Assert.Equal(500, serverErrorResult.StatusCode);
        }

        [Fact]
        public async void delete_with_id_of_existing_car_returns_OK() {
            //Arrange
            repository.GetById(Arg.Any<int>()).Returns(x =>new Car {
                id = 1,
                brand = "",
                imageUrl = "",
                isAvailable = true,
                licensePlate = "KD-D2-LL",
                location = "",
                model = "",
                seats = 5
            },
            x => null);

            //Act
            var result = await controller.Delete(1);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
        }
    }
}
