using DomainServices;
using DonainModel;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Repositories; 

public class UserRepository : IUserRepository {
    private readonly UserManager<IdentityUser> _userManager;
    private readonly AppDbContext _context;
    
    public UserRepository(AppDbContext context, UserManager<IdentityUser> userManager) {
        _userManager = userManager;
        _context = context;
    }
    
    public IEnumerable<User> GetAll() {
        return _context.Users;
    }

    public User? GetById(int id) {
        return _context.Users.Find(id);
    }

    public async Task<User?> Create(User user, string password) {
        if (_userManager.FindByEmailAsync(user.emailAddress).Result != null && _context.Users.Any(u => u.emailAddress == user.emailAddress))
            throw new Exception("Email already exists");
        
            
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
            
        var identityUser = new IdentityUser {
            UserName = user.emailAddress,
            Email = user.emailAddress,
        };

        var identityResult = await _userManager.CreateAsync(identityUser, password);
        if (identityResult.Succeeded)
        {
            await _userManager.AddToRoleAsync(identityUser, user.role.ToString());
        }
            
        
        user.securityId = identityUser.Id;
        _context.SaveChanges();
            
        return await Task.FromResult(user)!;

    }

    public async Task<User> Update(int id, User user) {
        user.id = id;
        
        //find user an identityuser
        User? userToUpdate = _context.Users.Find(id);
        if (userToUpdate == null) {
            return null;
        }
        
        IdentityUser? identityUserToUpdate = _userManager.FindByIdAsync(userToUpdate.securityId).Result;
        if (identityUserToUpdate == null) {
            return null;
        }
        
        //check if email is duplicate
        if (_context.Users.Any(i => i.emailAddress == user.emailAddress && i.id != id)) {
            throw new Exception("Duplicate email");
        }
        
        //set values
        userToUpdate.name = user.name;
        userToUpdate.emailAddress = user.emailAddress;
        userToUpdate.employeeNr = user.employeeNr;
        userToUpdate.hasLicense = user.hasLicense;
        userToUpdate.role = user.role;
        
        identityUserToUpdate.UserName = user.emailAddress;
        identityUserToUpdate.Email = user.emailAddress;
        
        //update
        _context.Users.Update(userToUpdate);
        await _context.SaveChangesAsync();
        
        await _userManager.UpdateAsync(identityUserToUpdate);
        await _userManager.RemoveFromRolesAsync(identityUserToUpdate, await _userManager.GetRolesAsync(identityUserToUpdate));
        await _userManager.AddToRoleAsync(identityUserToUpdate, user.role.ToString());
        
        return userToUpdate;
    }

    public async Task Delete(int id) {
        User? user = _context.Users.Find(id);
        if (user == null) {
            throw new Exception("User not found");
        }
        var identityUser = _userManager.FindByIdAsync(user.securityId).Result;
        if (identityUser == null) {
            throw new Exception("User not found");
        }
        _context.Users.Remove(user);
        await _userManager.DeleteAsync(identityUser);
        await _context.SaveChangesAsync();
    }
    
    public User? GetBySecurityId(string securityId) {
        return _context.Users.FirstOrDefault(u => u.securityId == securityId);
    }
}