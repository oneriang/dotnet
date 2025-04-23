using DataManagementApp.Services;
using DataManagementApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;  // 添加这行以使用 EF Core 的异步扩展方法

namespace DataManagementApp.Controllers
{
    public abstract class BaseController<T, TViewModel> : Controller 
        where T : class 
        where TViewModel : BaseViewModel<T>, new()
    {
        protected readonly IBaseService<T> _service;
        protected abstract string EntityName { get; }
        protected virtual int PageSize => 10;

        public BaseController(IBaseService<T> service)
        {
            _service = service;
        }

        public virtual async Task<IActionResult> Index(
            string sortOrder,
            string currentFilter,
            string searchString,
            int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var entities = await _service.GetAllAsync();
            var query = entities.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = ApplySearchFilter(query, searchString);
            }

            query = ApplySorting(query, sortOrder);

            var paginatedList = await PaginatedList<T>.CreateAsync(query, pageNumber ?? 1, PageSize);
            
            return View(paginatedList);
        }

        protected virtual IQueryable<T> ApplySearchFilter(IQueryable<T> query, string searchString)
        {
            return query;
        }

        protected virtual IQueryable<T> ApplySorting(IQueryable<T> query, string sortOrder)
        {
            return query;
        }

        public virtual async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entity = await _service.GetByIdAsync(id.Value);
            if (entity == null)
            {
                return NotFound();
            }

            var viewModel = new TViewModel
            {
                Entity = entity,
                PageTitle = $"{EntityName} 详情"
            };

            return View(viewModel);
        }

        public virtual IActionResult Create()
        {
            var viewModel = new TViewModel
            {
                PageTitle = $"创建{EntityName}",
                FormAction = "Create",
                FormController = this.GetType().Name.Replace("Controller", "")
            };
            return View("Form", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Create(T entity)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _service.CreateAsync(entity);
                    TempData["SuccessMessage"] = $"{EntityName}创建成功";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"创建{EntityName}时出错: {ex.Message}");
                }
            }
            
            var viewModel = new TViewModel
            {
                Entity = entity,
                PageTitle = $"创建{EntityName}",
                FormAction = "Create",
                FormController = this.GetType().Name.Replace("Controller", "")
            };
            
            return View("Form", viewModel);
        }

        public virtual async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entity = await _service.GetByIdAsync(id.Value);
            if (entity == null)
            {
                return NotFound();
            }

            var viewModel = new TViewModel
            {
                Entity = entity,
                PageTitle = $"编辑{EntityName}",
                FormAction = "Edit",
                FormController = this.GetType().Name.Replace("Controller", "")
            };

            return View("Form", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Edit(int id, T entity)
        {
            if (id != (entity as dynamic).Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _service.UpdateAsync(entity);
                    TempData["SuccessMessage"] = $"{EntityName}更新成功";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"更新{EntityName}时出错: {ex.Message}");
                }
            }
            
            var viewModel = new TViewModel
            {
                Entity = entity,
                PageTitle = $"编辑{EntityName}",
                FormAction = "Edit",
                FormController = this.GetType().Name.Replace("Controller", "")
            };
            
            return View("Form", viewModel);
        }

        public virtual async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entity = await _service.GetByIdAsync(id.Value);
            if (entity == null)
            {
                return NotFound();
            }

            var viewModel = new TViewModel
            {
                Entity = entity,
                PageTitle = $"删除{EntityName}"
            };

            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _service.DeleteAsync(id);
                TempData["SuccessMessage"] = $"{EntityName}删除成功";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"删除{EntityName}时出错: {ex.Message}";
            }
            
            return RedirectToAction(nameof(Index));
        }
    }

    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            this.AddRange(items);
        }

        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;

        public static async Task<PaginatedList<T>> CreateAsync(
            IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip(
                (pageIndex - 1) * pageSize)
                .Take(pageSize).ToListAsync();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }
}
