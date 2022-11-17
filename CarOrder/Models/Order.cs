using Microsoft.AspNetCore.Identity;

namespace CarOrder.Models;

public class Order
{
    public int Id { get; set; }

    public string Comment { get; set; } = null!;

    public IdentityUser User { get; set; } = null!;
    
    public Car Car { get; set; } = null!;
}