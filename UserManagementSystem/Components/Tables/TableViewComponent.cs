using Microsoft.AspNetCore.Mvc;
using UserManagementSystem.Models.ComponentModels;

namespace UserManagementSystem.Components.Tables
{
    public class TableViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(TableComponent config)
        {
            return View(config);
        }
    }
}
