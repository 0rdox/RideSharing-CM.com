using DonainModel;
using DonainModel.dto;

namespace DomainServices {
    public interface IReservationRepository {
        public IEnumerable<Reservation> GetAll(int? userId = null, int? carId = null, DateTime? beginDate = null, DateTime? endDate = null);

        public Reservation? GetById(int id);

        public Task<Reservation> Create(ReservationDto reservation, int userId);

        public Reservation Update(Reservation reservation);

        public Task Delete(int id);
    }
}
