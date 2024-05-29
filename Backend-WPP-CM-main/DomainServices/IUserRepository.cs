using DonainModel;

namespace DomainServices {
    public interface IUserRepository {
        public IEnumerable<User> GetAll();

        public User? GetById(int id);

        public Task<User?> Create(User user, string password);

        public Task<User> Update(int id, User user); 

        public Task Delete(int id);
        
        public User? GetBySecurityId(string securityId);
    }
}
