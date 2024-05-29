using DomainServices;
using DonainModel;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.tests;

public class UserRepositoryTest : IDisposable {
    private AppDbContext _context { get; set; }
    private SecurityDbContext _securityContext { get; set; }
    private IUserRepository _repository { get; set; }
    private UserManager<IdentityUser> _userManager { get; set; }

    public UserRepositoryTest() {
        //setup db
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseInMemoryDatabase(databaseName: "UserDb");

        //set up context
        _context = new AppDbContext(optionsBuilder.Options);

        //setup security db
        var securityOptionsBuilder = new DbContextOptionsBuilder<SecurityDbContext>();
        securityOptionsBuilder.UseInMemoryDatabase(databaseName: "SecurityDb");

        //set up security context
        _securityContext = new SecurityDbContext(securityOptionsBuilder.Options);

        //set up user manager
        var userStore = new UserStore<IdentityUser>(_securityContext);
        var userValidator = new UserValidator<IdentityUser>();
        var passwordValidator = new PasswordValidator<IdentityUser>();
        var passwordHasher = new PasswordHasher<IdentityUser>();
        var userValidators = new List<IUserValidator<IdentityUser>>{ userValidator };
        var passwordValidators = new List<IPasswordValidator<IdentityUser>>{ passwordValidator };
        var logger = new LoggerFactory().CreateLogger<UserManager<IdentityUser>>();
        // set up role manager
        var roleStore = new RoleStore<IdentityRole>(_securityContext);
        var roleValidator = new RoleValidator<IdentityRole>();
        var roleValidators = new List<IRoleValidator<IdentityRole>> { roleValidator };
        var loggerRoleManager = new LoggerFactory().CreateLogger<RoleManager<IdentityRole>>();
        var roleManager = new RoleManager<IdentityRole>(roleStore, roleValidators, null, null, loggerRoleManager);

        // seed roles
        var roles = new[] { "ADMIN", "USER" };
        foreach (var role in roles)
        {
            if (!roleManager.RoleExistsAsync(role).Result)
            {
                roleManager.CreateAsync(new IdentityRole(role)).Wait();
            }
        }
        
        _userManager = new UserManager<IdentityUser>(userStore, null, passwordHasher, userValidators,
            passwordValidators, null, null, null, logger);
        //set up repo
        _repository = new UserRepository(_context, _userManager);
    }

    public void Dispose() {
        _context.Database.EnsureDeleted();
        _context.Dispose();

        _securityContext.Database.EnsureDeleted();
        _securityContext.Dispose();
    }

    [Fact]
    public void get_all_with_empty_db_returns_empty_list() {
        //Arrange


        //Act
        var result = _repository.GetAll();

        //Assert
        Assert.Empty(result);
    }

    [Fact]
    public async void get_all_returns_list() {
        //Arrange
        User user = new User{
            id = 0,
            emailAddress = "test@test.com",
            name = "",
            role = Role.USER,
            employeeNr = "",
            hasLicense = false
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        //Act
        var result = _repository.GetAll();

        //Assert
        Assert.NotEmpty(result);
    }

    [Fact]
    public async void get_by_id_returns_user() {
        //Arrange
        User user = new User{
            id = 2,
            emailAddress = "test@test.com",
            name = "",
            role = Role.USER,
            employeeNr = "",
            hasLicense = false
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        //Act
        var result = _repository.GetById(2);

        //Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async void get_by_id_returns_null() {
        //Arrange
        User user = new User{
            name = "",
            emailAddress = "test@test.com",
            role = Role.USER,
            employeeNr = "",
            hasLicense = false
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        //Act
        var result = _repository.GetById(int.MaxValue);

        //Assert
        Assert.Null(result);
    }

    [Fact]
    public async void create_user_returns_user() {
        //Arrange
        User user = new User{
            name = "",
            emailAddress = "test@test.com",
            employeeNr = "",
            hasLicense = false,
            role = Role.USER
        };

        //Act
        var result = await _repository.Create(user, "test");

        //Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async void create_duplicate_user_returns_null() {
        //Arrange
        User user = new User{
            name = "",
            emailAddress = "test@test.com",
            employeeNr = "",
            hasLicense = false,
            role = Role.USER
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        _userManager.CreateAsync(new IdentityUser{
            UserName = "test@test.com",
            Email = "test@test.com"
        }, "Test123$").Wait();

        //Act
        

        //Assert
        await Assert.ThrowsAsync<Exception>(async () => await _repository.Create(user, "test"));
    }
  
        [Fact]
    public async void update_user_returns_user() {
        //Arrange
        User user = new User{
            id = 2,
            name = "",
            emailAddress = "test@test.test",
            employeeNr = "",
            hasLicense = false,
            role = Role.USER
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        _userManager.CreateAsync(new IdentityUser{
            UserName = "test@test.test",
            Email = "test@test.test"
                }, "Test123$").Wait();
        
        user.securityId = _userManager.FindByEmailAsync("test@test.test").Result!.Id;

        User updatedUser = new User{
            id = 2,
            name = "test",
            emailAddress = "test@test.test",
            employeeNr = "test",
            hasLicense = true,
            role = Role.ADMIN
        };

        //Act
        var result = await _repository.Update(updatedUser.id, updatedUser);

        //Assert
        Assert.NotNull(result);
        Assert.Equal(updatedUser.name, result.name);
        Assert.Equal(updatedUser.emailAddress, result.emailAddress);
        Assert.Equal(updatedUser.employeeNr, result.employeeNr);
        Assert.Equal(updatedUser.hasLicense, result.hasLicense);
        Assert.Equal(updatedUser.role, result.role);
    }

    [Fact]
    public async void update_user_returns_null() {
        //Arrange
        User user = new User{
            id = 1,
            name = "",
            emailAddress = "test@test.test",
            employeeNr = "",
            hasLicense = false,
            role = Role.USER
        };

        _userManager.CreateAsync(new IdentityUser{
            UserName = "test@test.test",
            Email = "test@test.test"
        }, "Test123$").Wait();

        user.securityId = _userManager.FindByEmailAsync("test@test.test").Result!.Id;

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        User updatedUser = new User{
            id = 2,
            name = "test",
            emailAddress = "test@test.test",
            employeeNr = "test",
            hasLicense = true,
            role = Role.ADMIN
        };

        //Act
        var result = await _repository.Update(updatedUser.id, updatedUser);

        //Assert
        Assert.Null(result);
    }

    [Fact]
    public async void delete_user_returns_null() {
        //Arrange
        User user = new User{
            name = "",
            emailAddress = "test@test.test",
            employeeNr = "",
            hasLicense = false,
            role = Role.USER
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        _userManager.CreateAsync(new IdentityUser{
            UserName = "test@test.test",
            Email = "test@test.test"
        }, "Test123$").Wait();
        
        user.securityId = _userManager.FindByEmailAsync("test@test.test").Result!.Id;
        await _context.SaveChangesAsync();

        //Act
        _repository.Delete(user.id);

        //Assert
        Assert.Null(_repository.GetById(user.id));
    }

    [Fact]
    public async void delete_user_returns_exception() {
        //Arrange

        //Act

        //Assert
        await Assert.ThrowsAsync<Exception>(async () => await _repository.Delete(int.MaxValue - 1));
    }
}