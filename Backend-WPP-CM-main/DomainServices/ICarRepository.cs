using DonainModel;

namespace DomainServices {
    public interface ICarRepository {
        public IEnumerable<Car> GetAll(DateTime? departureDate = null, string? destination = null, int? numberOfSeats = null);

        public Car? GetById(int id);

        public Task<Car?> Create(Car car);

        public Task<Car> Update(int id, Car car);

        public Task Delete(int id);
    }
}
