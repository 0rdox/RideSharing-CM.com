
using DonainModel;

namespace DomainServices {
    public interface IEmailService {
        public Task<bool> SendRequestAsync(string name, string email, string destination, string departureTime, string requestId, string requestToken);

        public Task<bool> SendDeletionEmailAsync(string[] emails, string name, string destination, string departureTime);

        public Task<bool> SendRequestStatusAsync(string email, string name, Status status, string destination, string departureTime);
        }
}
