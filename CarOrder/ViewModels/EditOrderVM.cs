using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarOrder.ViewModels;

public class EditOrderVM : CreateOrderVM
{
    public int Id { get; set; }

    [ValidateNever] public IEnumerable<SelectListItem> Cars { get; set; } = null!;
}