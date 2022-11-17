using CarOrder.Models;
using CarOrder.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CarOrder.Data;

public class DbInitializer : IDbInitializer
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _context;
    
    public DbInitializer(UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
    }
    
    public async Task Initialize()
    {
        try
        {
            await _context.Database.MigrateAsync();
            var editRoleExist = await _roleManager.RoleExistsAsync(Roles.Edit);
            if (!editRoleExist)
            {
                await _roleManager.CreateAsync(new IdentityRole(Roles.Edit));
                await _roleManager.CreateAsync(new IdentityRole(Roles.Remove));

                await _userManager.CreateAsync(new IdentityUser()
                {
                    Email = "bob@gmail.com",
                    UserName = "bob@gmail.com",
                }, "Ahmed123456789*");

                await _userManager.CreateAsync(new IdentityUser()
                {
                    Email = "tom@gmail.com",
                    UserName = "tom@gmail.com",
                }, "Ahmed123456789*");


                var bob =
                    await _userManager.Users.FirstOrDefaultAsync(u => u.Email == "bob@gmail.com");
                
                var tom =
                    await _userManager.Users.FirstOrDefaultAsync(u => u.Email == "tom@gmail.com");
                
                if (bob == null || tom == null) throw new Exception("User not found");
                
                await  _userManager.AddToRoleAsync(bob, Roles.Edit);
                await  _userManager.AddToRoleAsync(tom, Roles.Remove);

                List<Car> cars = new List<Car>()
                {
                    new Car()
                    {
                        Model = "Ferrari",
                        LicensePlate = "54f5r1s"
                    },
                    new Car()
                    {
                        Model = "Mercedes",
                        LicensePlate = "ed6914"
                    },
                    new Car()
                    {
                        Model = "Bego",
                        LicensePlate = "q9f3d6"
                    },
                };
                
                _context.Cars.AddRange(cars);

                await _context.SaveChangesAsync();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}