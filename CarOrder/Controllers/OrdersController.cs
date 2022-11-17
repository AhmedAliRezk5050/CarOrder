using Microsoft.AspNetCore.Mvc;
using CarOrder.Data;
using CarOrder.Models;
using CarOrder.Utility;
using CarOrder.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CarOrder.Controllers;

[Authorize]
public class OrdersController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;


    public OrdersController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var orders = await _context
            .Orders
            .Include(o => o.Car)
            .Include(o => o.User)
            .ToListAsync();

        return View(orders);
    }

    [Authorize(Roles = $"{Roles.Edit},{Roles.Remove}")]
    public async Task<IActionResult> Create()
    {
        ViewBag.Cars = (await _context.Cars.ToListAsync()).Select(c => new SelectListItem()
        {
            Text = c.Model,
            Value = c.Id.ToString()
        });
        return View();
    }

    [Authorize(Roles = $"{Roles.Edit},{Roles.Remove}")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateOrderVM model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);

                var order = new Order()
                {
                    Comment = model.Comment,
                    User = user,
                    Car = (await _context.Cars.FirstOrDefaultAsync(c => c.Id == model.CarId))!
                };

                _context.Orders.Add(order);

                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }

        return View(model);
    }

    [Authorize(Roles = Roles.Edit)]
    public async Task<IActionResult> Edit(int? id)
    {
        if (!IsValidId(id))
        {
            return NotFound();
        }

        var order = await _context.Orders
            .Include(o => o.Car)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
        {
            return NotFound();
        }
        return View(new EditOrderVM()
        {
            Id = order.Id,
            Comment = order.Comment,
            CarId = order.Car.Id,
            Cars = (await _context.Cars.ToListAsync()).Select(c => new SelectListItem()
            {
                Text = c.Model,
                Value = c.Id.ToString()
            })
        });
    }
    
    [Authorize(Roles = Roles.Edit)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EditOrderVM model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);

                if (order is null)
                {
                    return NotFound();
                }

                var car = await _context.Cars.FirstOrDefaultAsync(c => c.Id == model.CarId);

                if (car is not null)
                {
                    order.Car = car;
                }

                order.Comment = model.Comment;
                
                await _context.SaveChangesAsync();
                
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Unable to save changes. " +
                                             "Try again, and if the problem persists, " +
                                             "see your system administrator.");
            }
        }

        return View(model);
    }
    
    
    [Authorize(Roles = Roles.Remove)]
    public async Task<IActionResult> Delete(int? id, bool? saveChangesError)
    {
        if (!IsValidId(id))
        {
            return NotFound();
        }

        var order = await _context
            .Orders
            .Include(o => o.Car)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
        {
            return NotFound();
        }
        
        if (saveChangesError.GetValueOrDefault())
        {
            AddDeletionFailureTempData();
        }

        return View(order!);
    }

    [Authorize(Roles = Roles.Remove)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int? id)
    {
        if (!IsValidId(id))
        {
            return NotFound();
        }

        try
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);

            if (order is null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
        
        return RedirectToAction(nameof(Delete), new { id, saveChangesError = true });
    }

    private static bool IsValidId(int? id) => id is not null or 0;
    
    private void AddDeletionFailureTempData()
    {
        TempData["error"] =
            "Delete failed. Try again, and if the problem persists " +
            "see your system administrator.";
    }
}