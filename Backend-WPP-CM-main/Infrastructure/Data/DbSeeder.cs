using DonainModel;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class DbSeeder
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly AppDbContext _context;

    public DbSeeder(UserManager<IdentityUser> userManager, AppDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public async Task Seed()
    {
        // Check if any users exist
        if (!_context.Users.Any())
        {
            var users = new List<User>
            {
                new User() {name = "admin", emailAddress = "admin@cm.com", employeeNr = "nr1", hasLicense = true, role = Role.ADMIN},
                new User() {name = "user", emailAddress = "user@cm.com", employeeNr = "nr2", hasLicense = true, role = Role.USER}
            };
            
            foreach (var user in users)
            {
                var identityUser = new IdentityUser
                {
                    UserName = user.emailAddress,
                    Email = user.emailAddress,
                };

                var identityResult = await _userManager.CreateAsync(identityUser, "Password123!");
                if (identityResult.Succeeded)
                {
                    await _userManager.AddToRoleAsync(identityUser, user.role.ToString());
                }

                user.securityId = identityUser.Id;
                _context.Users.Add(user);
            }

            await _context.SaveChangesAsync();
            
        }
        
        // Check if any cars exist
        if (!_context.Cars.Any())
        {
            var cars = new List<Car>
            {
                new Car() {brand = "brand", imageUrl = "https://www.rtlnieuws.nl/sites/default/files/content/images/2023/02/21/nio.jpg?itok=hZID-uqv&width=2048&height=1152&impolicy=semi_dynamic", isAvailable = true, licensePlate = "R-472-NB", location = "Breda", model = "model", seats = 5}
            };
            
            _context.Cars.AddRange(cars);
            await _context.SaveChangesAsync();
            
        }
    }}