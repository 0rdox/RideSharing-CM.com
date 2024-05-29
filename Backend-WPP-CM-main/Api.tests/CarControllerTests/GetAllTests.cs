using API.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Newtonsoft.Json;

namespace Api.tests.CarControllerTests {
    public class GetAllTests {
        private ICarRepository repository;
        private CarController controller;

        public GetAllTests() {
            repository = Substitute.For<ICarRepository>();
            controller = new CarController(repository);
        }

        private static Object? GetDataFromReturnObject(Object obj) {
            return obj.GetType().GetProperties().First(i => i.Name == "data").GetValue(obj, null);
        }

        [Fact]
        public void method_returns_OK_with_no_filter_not_filled() {
            //Arrange
            repository.GetAll().Returns(new List<Car>().AsEnumerable());

            //Act
            var result = controller.GetAll(null, null, null);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult);
            Assert.NotNull(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);

            var model = (Object)okResult.Value;
            var data = (List<Object>)GetDataFromReturnObject(model); 
            Assert.NotNull(data);
            Assert.Empty(data);
        }

        [Fact]
        public void method_returns_OK_with_no_filter_filled() {
            //Arrange
            repository.GetAll().Returns(new List<Car> {
                new Car{
                    id= 1,
                    brand ="",
                    imageUrl="",
                    isAvailable=true,
                    licensePlate="KD-D2-LL",
                    location="",
                    model="",
                    seats=5
                }}.AsEnumerable());

            //Act
            var result = controller.GetAll(null, null, null);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult);
            Assert.NotNull(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);

            var model = (Object)okResult.Value;
            var data = (List<Object>)GetDataFromReturnObject(model);
            Assert.NotNull(data);
            Assert.NotEmpty(data);
        }

        [Fact]
        public void method_returns_OK_with_Date_filter_filled() {
            //Arrange
            repository.GetAll(departureDate: Arg.Any<DateTime>()).Returns(new List<Car> {
                new Car{
                    id= 1,
                    brand ="",
                    imageUrl="",
                    isAvailable=true,
                    licensePlate="KD-D2-LL",
                    location="",
                    model="",
                    seats=5
                }}.AsEnumerable());

            //Act
            var result = controller.GetAll(new DateTime().ToString(), null, null);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult);
            Assert.NotNull(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);

            var model = (Object)okResult.Value;
            var data = (List<Object>)GetDataFromReturnObject(model);
            Assert.NotNull(data);
            Assert.NotEmpty(data);
        }

        [Fact]
        public void method_returns_BADREQUEST_with_Date_filter_filled() {
            //Arrange
            repository.GetAll(departureDate: Arg.Any<DateTime>()).Returns(new List<Car> {
                new Car{
                    id= 1,
                    brand ="",
                    imageUrl="",
                    isAvailable=true,
                    licensePlate="KD-D2-LL",
                    location="",
                    model="",
                    seats=5
                }}.AsEnumerable());

            //Act
            var result = controller.GetAll("ssf", null, null);

            //Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badResult);
            Assert.NotNull(badResult.Value);
            Assert.Equal(400, badResult.StatusCode);

            var model = (Object)badResult.Value;
            var data = GetDataFromReturnObject(model);
            Assert.NotNull(data);
            Assert.Empty(data.GetType().GetProperties());
        }

        [Fact]
        public void method_returns_OK_with_seats_filter_filled() {
            //Arrange
            repository.GetAll(numberOfSeats: Arg.Any<int>()).Returns(new List<Car> {
                new Car{
                    id= 1,
                    brand ="",
                    imageUrl="",
                    isAvailable=true,
                    licensePlate="KD-D2-LL",
                    location="",
                    model="",
                    seats=5
                }}.AsEnumerable());

            //Act
            var result = controller.GetAll(null, null, "5");

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult);
            Assert.NotNull(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);

            var model = (Object)okResult.Value;
            var data = (List<Object>)GetDataFromReturnObject(model);
            Assert.NotNull(data);
            Assert.NotEmpty(data);
        }

        [Fact]
        public void method_returns_BADREQUEST_with_seats_filter_filled() {
            //Arrange
            repository.GetAll(numberOfSeats: Arg.Any<int>()).Returns(new List<Car> {
                new Car{
                    id= 1,
                    brand ="",
                    imageUrl="",
                    isAvailable=true,
                    licensePlate="KD-D2-LL",
                    location="",
                    model="",
                    seats=5
                }}.AsEnumerable());

            //Act
            var result = controller.GetAll(null, null, "sdf");

            //Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badResult);
            Assert.NotNull(badResult.Value);
            Assert.Equal(400, badResult.StatusCode);

            var model = (Object)badResult.Value;
            var data = GetDataFromReturnObject(model);
            Assert.NotNull(data);
            Assert.Empty(data.GetType().GetProperties());
        }
    }
}
