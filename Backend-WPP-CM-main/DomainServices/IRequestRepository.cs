using DonainModel;
using DonainModel.dto;

namespace DomainServices {
    public interface IRequestRepository {
        public IEnumerable<Request> GetAll(int? reservationId = null, Status? requestStatus = null, DateTime? creationDateStart = null, DateTime? creationDateEnd = null, int? userId = null);

        public Request? GetById(int id);

        public Task<Request> Create(RequestDto request, int userId);

        public Task<Request> VerifyRequest(int requestId, string verifyToken, Status newStatus);

        public Task Delete(int id);
    }
}
