using DonainModel;

namespace DomainModel.tests {
    public class CarTest {
        [Fact]
        public void seats_with_negative_value_gives_exception() {
            //Arrange
            Exception? exception = null;

            //Act
            try {
                Car car = new Car() {
                    brand = "",
                    id = 0,
                    imageUrl = "",
                    isAvailable = true,
                    licensePlate = "",
                    location = "",
                    model = "",
                    seats = -2
                };
            } catch (Exception ex) {
                exception = ex;
            }


            //Assert
            Assert.NotNull(exception);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(5)]
        [InlineData(8)]
        public void seats_with_valid_value_does_not_give_exception(int seats) {
            //Arrange
            Exception? exception = null;

            //Act
            try {
                Car car = new Car() {
                    brand = "",
                    id = 0,
                    imageUrl = "",
                    isAvailable = true,
                    licensePlate = "999-XX-9",
                    location = "",
                    model = "",
                    seats = seats
                };
            } catch (Exception ex) {
                exception = ex;
            }


            //Assert
            Assert.Null(exception);
        }

        [Fact]
        public void seats_with_value_of_null_gives_exception() {
            //Arrange
            Exception? exception = null;

            //Act
            try {
                Car car = new Car() {
                    brand = "",
                    id = 0,
                    imageUrl = "",
                    isAvailable = true,
                    licensePlate = "999-XX-9",
                    location = "",
                    model = "",
                    seats = 0
                };
            } catch (Exception ex) {
                exception = ex;
            }


            //Assert
            Assert.Null(exception);
        }

        [Fact]
        public void licensePlate_with_invalid_value_thorws_error() {
            //Arrange
            Exception? exception = null;

            //Act
            try {
                Car car = new Car() {
                    brand = "",
                    id = 0,
                    imageUrl = "",
                    isAvailable = true,
                    licensePlate = "99A9-XX-9",
                    location = "",
                    model = "",
                    seats = 0
                };
            } catch (Exception ex) {
                exception = ex;
            }


            //Assert
            Assert.NotNull(exception);
        }

        [Fact]
        public void licensePlate_with_valid_value_thorws__no_error() {
            //Arrange
            Exception? exception = null;

            //Act
            try {
                Car car = new Car() {
                    brand = "",
                    id = 0,
                    imageUrl = "",
                    isAvailable = true,
                    licensePlate = "999-XX-9",
                    location = "",
                    model = "",
                    seats = 0
                };
            } catch (Exception ex) {
                exception = ex;
            }


            //Assert
            Assert.Null(exception);
        }

        [Theory]
        [InlineData('A')]
        [InlineData('E')]
        [InlineData('I')]
        [InlineData('O')]
        [InlineData('U')]
        public void checkLicensePlate_with_forbidden_caracter_thorws_error(char ch) {
            //Arrange
            Exception? exception = null;

            //Act
            try {
                Car car = new Car() {
                    brand = "",
                    id = 0,
                    imageUrl = "",
                    isAvailable = true,
                    licensePlate = $"99-{ch}X-9",
                    location = "",
                    model = "",
                    seats = 0
                };
            } catch (Exception ex) {
                exception = ex;
            }


            //Assert
            Assert.NotNull(exception);
        }

        [Theory]
        [InlineData("GVD")]
        [InlineData("KKK")]
        [InlineData("KVT")]
        [InlineData("LPF")]
        [InlineData("NSB")]
        [InlineData("PKK")]
        [InlineData("PSV")]
        [InlineData("SS")]
        [InlineData("PVV")]
        [InlineData("SGP")]
        [InlineData("VVD")]
        public void checkLicensePlate_with_forbidden_caracter_combinations_thorws_error(string str) {
            //Arrange
            Exception? exception = null;

            //Act
            try {
                Car car = new Car() {
                    brand = "",
                    id = 0,
                    imageUrl = "",
                    isAvailable = true,
                    licensePlate = $"99-{str}-9",
                    location = "",
                    model = "",
                    seats = 0
                };
            } catch (Exception ex) {
                exception = ex;
            }


            //Assert
            Assert.NotNull(exception);
        }

        [Theory]
        [InlineData("99-9")]
        [InlineData("99-99-XX-XX")]
        [InlineData("")]
        public void checkLicensePlate_with_not_enough_lines_thorws_error(string str) {
            //Arrange
            Exception? exception = null;

            //Act
            try {
                Car car = new Car() {
                    brand = "",
                    id = 0,
                    imageUrl = "",
                    isAvailable = true,
                    licensePlate = str,
                    location = "",
                    model = "",
                    seats = 0
                };
            } catch (Exception ex) {
                exception = ex;
            }


            //Assert
            Assert.NotNull(exception);
        }

        [Fact]
        public void checkLicensePlate_with_lower_case_sets_upper_case_characters() {
            //Arrange
            Exception? exception = null;

            //Act
            try {
                Car car = new Car() {
                    brand = "",
                    id = 0,
                    imageUrl = "",
                    isAvailable = true,
                    licensePlate = "99-xx-99",
                    location = "",
                    model = "",
                    seats = 0
                };
            } catch (Exception ex) {
                exception = ex;
            }


            //Assert
            Assert.Null(exception);
        }
    }
}
