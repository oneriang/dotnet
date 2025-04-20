using Microsoft.AspNetCore.Mvc;
using UserManagementSystem.Models.ComponentModels;

namespace UserManagementSystem.Components.Forms
{
    public class FormFieldViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(FormField field, object value = null)
        {
            return View(field.Type.ToLower(), field);
        }
    }
}
