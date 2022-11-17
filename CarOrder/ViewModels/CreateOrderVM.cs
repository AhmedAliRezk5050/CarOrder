using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CarOrder.ViewModels;

public class CreateOrderVM
{
    [Required]
    public string Comment { get; set; } = null!;
    
    [Required]
    [DisplayName("Car")]
    public int? CarId { get; set; }  
}