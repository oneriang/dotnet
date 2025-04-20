using Microsoft.AspNetCore.Mvc;
using UserManagementSystem.Models;
using UserManagementSystem.Services;

namespace UserManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ComponentService _componentService;
        
        public HomeController(ComponentService componentService)
        {
            _componentService = componentService;
        }
        
        public async Task<IActionResult> Index()
        {
            var layout = await _componentService.LoadLayoutAsync();
            return View(layout);
        }
        
        [HttpGet("GetForm")]
        public async Task<IActionResult> GetForm(string formId, [FromQuery] User data)
        {
            var forms = await _componentService.LoadFormsAsync();
            if (forms.TryGetValue(formId, out var formConfig))
            {
                if (data.Id > 0)
                {
                    foreach (var field in formConfig.Fields)
                    {
                        if (!string.IsNullOrEmpty(field.Condition) && field.Condition == "!Id")
                        {
                            field.Hidden = true;
                        }
                    }
                }
                
                ViewBag.FormData = data;
                return PartialView("_FormModal", formConfig);
            }
            return NotFound();
        }
    }
}
