using API.Controllers;
using DonainModel.dto;
using Microsoft.AspNetCore.Mvc;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;

namespace Api.tests.CarControllerTests {
    public class CreateTests {
        private ICarRepository repository;
        private CarController controller;

        public CreateTests() {
            repository = Substitute.For<ICarRepository>();
            controller = new CarController(repository);
        }

        private static Object? GetDataFromReturnObject(Object obj, string property) {
            return obj.GetType().GetProperties().First(i => i.Name == property).GetValue(obj, null);
        }

        [Fact]
        public async void create_with_bad_dto_returns_bad_request() {
            //Arrange
            repository.Create(Arg.Any<Car>()).Returns(new Car {
                id = 1,
                brand = "",
                imageUrl = "",
                isAvailable = true,
                licensePlate = "KD-D2-LL",
                location = "",
                model = "",
                seats = 5
            });
            CarDto dto = new CarDto() {
                brand = ""
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
        public async void create_with_good_dto_but_bad_model_validations_returns_bad_request() {
            //Arrange
            repository.Create(Arg.Any<Car>()).Returns(new Car {
                id = 1,
                brand = "",
                imageUrl = "",
                isAvailable = true,
                licensePlate = "KD-D2-LL",
                location = "",
                model = "",
                seats = 5
            });
            CarDto dto = new CarDto() {
                brand = "",
                imageUrl = "",
                isAvailable = true,
                licensePlate = "AA",
                location = "",
                model = "",
                seats = 2
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
        public async void create_with_exception_thrown_at_repo_returns_conflict() {
            //Arrange
            repository.Create(Arg.Any<Car>()).Throws(new Exception());

            CarDto dto = new CarDto() {
                brand = "",
                imageUrl = "",
                isAvailable = true,
                licensePlate = "KD-D2-LL",
                location = "",
                model = "",
                seats = 2
            };

            //Act
            var result = await controller.Create(dto);
            
            //Assert
            var conflictResult = Assert.IsType<ConflictObjectResult>(result);
            Assert.NotNull(conflictResult);
            Assert.NotNull(conflictResult.Value);
            Assert.Equal(409, conflictResult.StatusCode);
        }

        [Fact]
        public async void create_with_repo_returns_null_returns_internal_server_error() {
            //Arrange
            repository.Create(Arg.Any<Car>()).ReturnsNull();

            CarDto dto = new CarDto() {
                brand = "",
                imageUrl = "",
                isAvailable = true,
                licensePlate = "KD-D2-LL",
                location = "",
                model = "",
                seats = 2
            };

            //Act
            var result = await controller.Create(dto);

            //Assert
            var serverErrorResult = Assert.IsType<ObjectResult>(result);
            Assert.NotNull(serverErrorResult);
            Assert.Equal(500, serverErrorResult.StatusCode);
        }

        [Fact]
        public async void create_with_good_dto_and_good_model_validations_returns_OK() {
            //Arrange
            repository.Create(Arg.Any<Car>()).Returns(new Car {
                id = 1,
                brand = "",
                imageUrl = "",
                isAvailable = true,
                licensePlate = "KD-D2-LL",
                location = "",
                model = "",
                seats = 5
            });
            CarDto dto = new CarDto() {
                brand = "",
                imageUrl = "",
                isAvailable = true,
                licensePlate = "KD-D2-LL",
                location = "",
                model = "",
                seats = 2
            };

            //Act
            var result = await controller.Create(dto);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult);
            Assert.NotNull(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);

            var model = (Object)okResult.Value;
            var data = GetDataFromReturnObject(model, "data");
            Assert.NotNull(data);
            var id = (int) GetDataFromReturnObject(data, "id");
            Assert.Equal(1 ,id);
        }
    }
}
