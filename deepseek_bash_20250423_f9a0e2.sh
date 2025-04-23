#!/bin/bash

# 创建项目目录结构
create_project_structure() {
    echo "创建项目目录结构..."
    mkdir -p DataManagementApp/{Controllers,Models,ViewModels,Data,Services,Views/Shared}
}

# 创建基础模型文件
create_base_entity() {
    echo "创建 BaseEntity.cs..."
    cat > DataManagementApp/Models/BaseEntity.cs << 'EOL'
using System;
using System.ComponentModel.DataAnnotations;

namespace DataManagementApp.Models
{
    public abstract class BaseEntity
    {
        [Key]
        public int Id { get; set; }
        
        [Display(Name = "创建时间")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        [Display(Name = "更新时间")]
        public DateTime? UpdatedAt { get; set; }
        
        [Display(Name = "是否删除")]
        public bool IsDeleted { get; set; } = false;
    }
}
EOL
}

# 创建基础视图模型
create_base_viewmodel() {
    echo "创建 BaseViewModel.cs..."
    cat > DataManagementApp/ViewModels/BaseViewModel.cs << 'EOL'
namespace DataManagementApp.ViewModels
{
    public abstract class BaseViewModel<T> where T : class
    {
        public T Entity { get; set; }
        public string PageTitle { get; set; }
        public string FormAction { get; set; }
        public string FormController { get; set; }
    }
}
EOL
}

# 创建基础服务接口
create_ibase_service() {
    echo "创建 IBaseService.cs..."
    cat > DataManagementApp/Services/IBaseService.cs << 'EOL'
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataManagementApp.Services
{
    public interface IBaseService<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task CreateAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
EOL
}

# 创建基础服务实现
create_base_service() {
    echo "创建 BaseService.cs..."
    cat > DataManagementApp/Services/BaseService.cs << 'EOL'
using DataManagementApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataManagementApp.Services
{
    public abstract class BaseService<T, TContext> : IBaseService<T> 
        where T : BaseEntity
        where TContext : DbContext
    {
        protected readonly TContext _context;

        public BaseService(TContext context)
        {
            _context = context;
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().Where(e => !e.IsDeleted).ToListAsync();
        }

        public virtual async Task<T> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
        }

        public virtual async Task CreateAsync(T entity)
        {
            entity.CreatedAt = DateTime.Now;
            _context.Set<T>().Add(entity);
            await _context.SaveChangesAsync();
        }

        public virtual async Task UpdateAsync(T entity)
        {
            entity.UpdatedAt = DateTime.Now;
            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();
        }

        public virtual async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                entity.IsDeleted = true;
                entity.UpdatedAt = DateTime.Now;
                await UpdateAsync(entity);
            }
        }

        public virtual async Task<bool> ExistsAsync(int id)
        {
            return await _context.Set<T>().AnyAsync(e => e.Id == id && !e.IsDeleted);
        }
    }
}
EOL
}

# 创建基础控制器
create_base_controller() {
    echo "创建 BaseController.cs..."
    cat > DataManagementApp/Controllers/BaseController.cs << 'EOL'
using DataManagementApp.Services;
using DataManagementApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

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
EOL
}

# 创建基础视图模板
create_base_views() {
    echo "创建基础视图模板..."
    
    # 基础布局
    cat > DataManagementApp/Views/Shared/_BaseLayout.cshtml << 'EOL'
<!DOCTYPE html>
<html lang="zh-CN">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>@ViewData["Title"] - 数据管理系统</title>
    <link rel="stylesheet" href="~/lib/bootstrap/dist/css/bootstrap.min.css" />
    <link rel="stylesheet" href="~/css/site.css" />
</head>
<body>
    <header>
        <nav class="navbar navbar-expand-sm navbar-toggleable-sm navbar-light bg-white border-bottom box-shadow mb-3">
            <div class="container">
                <a class="navbar-brand" asp-area="" asp-controller="Home" asp-action="Index">数据管理系统</a>
                <button class="navbar-toggler" type="button" data-toggle="collapse" data-target=".navbar-collapse" aria-controls="navbarSupportedContent"
                        aria-expanded="false" aria-label="Toggle navigation">
                    <span class="navbar-toggler-icon"></span>
                </button>
                <div class="navbar-collapse collapse d-sm-inline-flex flex-sm-row-reverse">
                    <partial name="_LoginPartial" />
                    <ul class="navbar-nav flex-grow-1">
                        <li class="nav-item">
                            <a class="nav-link text-dark" asp-area="" asp-controller="Home" asp-action="Index">首页</a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link text-dark" asp-area="" asp-controller="Products" asp-action="Index">产品管理</a>
                        </li>
                    </ul>
                </div>
            </div>
        </nav>
    </header>
    <div class="container">
        <main role="main" class="pb-3">
            @RenderBody()
        </main>
    </div>

    <footer class="border-top footer text-muted">
        <div class="container">
            &copy; @DateTime.Now.Year - 数据管理系统
        </div>
    </footer>

    <script src="~/lib/jquery/dist/jquery.min.js"></script>
    <script src="~/lib/bootstrap/dist/js/bootstrap.bundle.min.js"></script>
    <script src="~/js/site.js" asp-append-version="true"></script>
    @RenderSection("Scripts", required: false)
</body>
</html>
EOL

    # 基础列表视图
    cat > DataManagementApp/Views/Shared/_BaseList.cshtml << 'EOL'
@model DataManagementApp.Controllers.PaginatedList<dynamic>

@{
    ViewData["Title"] = "列表";
    Layout = "_BaseLayout";
}

<div class="card">
    <div class="card-header">
        <h3 class="card-title">@ViewData["Title"]</h3>
        <div class="card-tools">
            <a asp-action="Create" class="btn btn-primary">新建</a>
        </div>
    </div>
    <div class="card-body">
        @if (TempData["SuccessMessage"] != null)
        {
            <div class="alert alert-success">
                @TempData["SuccessMessage"]
            </div>
        }
        @if (TempData["ErrorMessage"] != null)
        {
            <div class="alert alert-danger">
                @TempData["ErrorMessage"]
            </div>
        }
        
        <form asp-action="Index" method="get">
            <div class="form-row">
                <div class="col-md-8 mb-3">
                    <input type="text" name="searchString" value="@ViewData["CurrentFilter"]" 
                           class="form-control" placeholder="搜索..." />
                </div>
                <div class="col-md-4 mb-3">
                    <button type="submit" class="btn btn-secondary">搜索</button>
                    <a asp-action="Index" class="btn btn-outline-secondary">重置</a>
                </div>
            </div>
        </form>
        
        <table class="table table-bordered table-striped">
            <thead>
                <tr>
                    @foreach (var prop in Model.FirstOrDefault()?.GetType().GetProperties())
                    {
                        <th>@prop.Name</th>
                    }
                    <th>操作</th>
                </tr>
            </thead>
            <tbody>
                @foreach (var item in Model)
                {
                    <tr>
                        @foreach (var prop in item.GetType().GetProperties())
                        {
                            <td>@prop.GetValue(item)</td>
                        }
                        <td>
                            <a asp-action="Edit" asp-route-id="@item.Id" class="btn btn-sm btn-warning">编辑</a>
                            <a asp-action="Details" asp-route-id="@item.Id" class="btn btn-sm btn-info">详情</a>
                            <a asp-action="Delete" asp-route-id="@item.Id" class="btn btn-sm btn-danger">删除</a>
                        </td>
                    </tr>
                }
            </tbody>
        </table>
        
        @{
            var prevDisabled = !Model.HasPreviousPage ? "disabled" : "";
            var nextDisabled = !Model.HasNextPage ? "disabled" : "";
        }
        
        <nav aria-label="Page navigation">
            <ul class="pagination">
                <li class="page-item @prevDisabled">
                    <a asp-action="Index"
                       asp-route-sortOrder="@ViewData["CurrentSort"]"
                       asp-route-pageNumber="@(Model.PageIndex - 1)"
                       asp-route-currentFilter="@ViewData["CurrentFilter"]"
                       class="page-link">上一页</a>
                </li>
                @for (var i = 1; i <= Model.TotalPages; i++)
                {
                    <li class="page-item @(i == Model.PageIndex ? "active" : "")">
                        <a asp-action="Index"
                           asp-route-sortOrder="@ViewData["CurrentSort"]"
                           asp-route-pageNumber="@i"
                           asp-route-currentFilter="@ViewData["CurrentFilter"]"
                           class="page-link">@i</a>
                    </li>
                }
                <li class="page-item @nextDisabled">
                    <a asp-action="Index"
                       asp-route-sortOrder="@ViewData["CurrentSort"]"
                       asp-route-pageNumber="@(Model.PageIndex + 1)"
                       asp-route-currentFilter="@ViewData["CurrentFilter"]"
                       class="page-link">下一页</a>
                </li>
            </ul>
        </nav>
    </div>
</div>
EOL

    # 基础表单视图
    cat > DataManagementApp/Views/Shared/_BaseForm.cshtml << 'EOL'
@model DataManagementApp.ViewModels.BaseViewModel<dynamic>

@{
    ViewData["Title"] = Model.PageTitle;
    Layout = "_BaseLayout";
}

<div class="card">
    <div class="card-header">
        <h3 class="card-title">@Model.PageTitle</h3>
        <div class="card-tools">
            <a asp-action="Index" class="btn btn-secondary">返回列表</a>
        </div>
    </div>
    <div class="card-body">
        @if (TempData["SuccessMessage"] != null)
        {
            <div class="alert alert-success">
                @TempData["SuccessMessage"]
            </div>
        }
        @if (TempData["ErrorMessage"] != null)
        {
            <div class="alert alert-danger">
                @TempData["ErrorMessage"]
            </div>
        }
        
        <form asp-action="@Model.FormAction" asp-controller="@Model.FormController" method="post">
            <div asp-validation-summary="ModelOnly" class="text-danger"></div>
            
            @if (Model.Entity.GetType().GetProperty("Id") != null && 
                 (int)Model.Entity.GetType().GetProperty("Id").GetValue(Model.Entity) != 0)
            {
                <input type="hidden" asp-for="@Model.Entity.GetType().GetProperty("Id").GetValue(Model.Entity)" name="Id" />
            }
            
            @foreach (var prop in Model.Entity.GetType().GetProperties())
            {
                if (prop.Name == "Id" || prop.Name == "CreatedAt" || prop.Name == "UpdatedAt" || prop.Name == "IsDeleted")
                {
                    continue;
                }
                
                var displayAttr = prop.GetCustomAttributes(typeof(DisplayAttribute), false)
                    .FirstOrDefault() as DisplayAttribute;
                var displayName = displayAttr?.Name ?? prop.Name;
                
                <div class="form-group">
                    <label class="control-label">@displayName</label>
                    
                    @if (prop.PropertyType == typeof(bool))
                    {
                        <div class="form-check">
                            <input type="checkbox" asp-for="@Model.Entity.GetType().GetProperty(prop.Name).GetValue(Model.Entity)" 
                                   class="form-check-input" name="@prop.Name" />
                        </div>
                    }
                    else if (prop.PropertyType == typeof(DateTime) || prop.PropertyType == typeof(DateTime?))
                    {
                        <input type="datetime-local" asp-for="@Model.Entity.GetType().GetProperty(prop.Name).GetValue(Model.Entity)" 
                               class="form-control" name="@prop.Name" />
                    }
                    else if (prop.PropertyType.IsEnum)
                    {
                        <select asp-for="@Model.Entity.GetType().GetProperty(prop.Name).GetValue(Model.Entity)" 
                                class="form-control" name="@prop.Name">
                            @foreach (var value in Enum.GetValues(prop.PropertyType))
                            {
                                <option value="@value">@value</option>
                            }
                        </select>
                    }
                    else
                    {
                        <input asp-for="@Model.Entity.GetType().GetProperty(prop.Name).GetValue(Model.Entity)" 
                               class="form-control" name="@prop.Name" />
                    }
                    
                    <span asp-validation-for="@Model.Entity.GetType().GetProperty(prop.Name).GetValue(Model.Entity)" 
                          class="text-danger"></span>
                </div>
            }
            
            <div class="form-group">
                <input type="submit" value="保存" class="btn btn-primary" />
            </div>
        </form>
    </div>
</div>

@section Scripts {
    @{await Html.RenderPartialAsync("_ValidationScriptsPartial");}
}
EOL
}

# 创建产品相关文件
create_product_files() {
    echo "创建产品相关文件..."
    
    # 产品模型
    cat > DataManagementApp/Models/Product.cs << 'EOL'
using System.ComponentModel.DataAnnotations;

namespace DataManagementApp.Models
{
    public class Product : BaseEntity
    {
        [Required]
        [Display(Name = "产品名称")]
        public string Name { get; set; }
        
        [Display(Name = "描述")]
        public string Description { get; set; }
        
        [Required]
        [Display(Name = "价格")]
        public decimal Price { get; set; }
        
        [Display(Name = "库存数量")]
        public int StockQuantity { get; set; }
    }
}
EOL

    # 产品视图模型
    cat > DataManagementApp/ViewModels/ProductViewModel.cs << 'EOL'
using DataManagementApp.Models;

namespace DataManagementApp.ViewModels
{
    public class ProductViewModel : BaseViewModel<Product>
    {
    }
}
EOL

    # 产品服务接口
    cat > DataManagementApp/Services/IProductService.cs << 'EOL'
using DataManagementApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataManagementApp.Services
{
    public interface IProductService : IBaseService<Product>
    {
        Task<IEnumerable<Product>> GetOutOfStockProducts();
        Task<IEnumerable<Product>> GetExpensiveProducts(decimal minPrice);
    }
}
EOL

    # 产品服务实现
    cat > DataManagementApp/Services/ProductService.cs << 'EOL'
using DataManagementApp.Data;
using DataManagementApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataManagementApp.Services
{
    public class ProductService : BaseService<Product, ApplicationDbContext>, IProductService
    {
        public ProductService(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Product>> GetOutOfStockProducts()
        {
            return await _context.Products
                .Where(p => p.StockQuantity <= 0 && !p.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetExpensiveProducts(decimal minPrice)
        {
            return await _context.Products
                .Where(p => p.Price >= minPrice && !p.IsDeleted)
                .ToListAsync();
        }
    }
}
EOL

    # 产品控制器
    cat > DataManagementApp/Controllers/ProductsController.cs << 'EOL'
using DataManagementApp.Models;
using DataManagementApp.Services;
using DataManagementApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace DataManagementApp.Controllers
{
    public class ProductsController : BaseController<Product, ProductViewModel>
    {
        public ProductsController(IProductService service) : base(service)
        {
        }

        protected override string EntityName => "产品";

        protected override IQueryable<Product> ApplySearchFilter(IQueryable<Product> query, string searchString)
        {
            return query.Where(p => 
                p.Name.Contains(searchString) || 
                p.Description.Contains(searchString));
        }

        protected override IQueryable<Product> ApplySorting(IQueryable<Product> query, string sortOrder)
        {
            switch (sortOrder)
            {
                case "name_desc":
                    return query.OrderByDescending(p => p.Name);
                case "price":
                    return query.OrderBy(p => p.Price);
                case "price_desc":
                    return query.OrderByDescending(p => p.Price);
                default:
                    return query.OrderBy(p => p.Name);
            }
        }
    }
}
EOL

    # 创建数据库上下文
    cat > DataManagementApp/Data/ApplicationDbContext.cs << 'EOL'
using DataManagementApp.Models;
using Microsoft.EntityFrameworkCore;

namespace DataManagementApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<Product>().HasQueryFilter(p => !p.IsDeleted);
        }
    }
}
EOL
}

# 创建项目文件
create_project_file() {
    echo "创建项目文件..."
    cat > DataManagementApp/DataManagementApp.csproj << 'EOL'
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net6.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="6.0.0" />
    <PackageReference Include="Microsoft.AspNetCore.Identity.UI" Version="6.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="6.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="6.0.0">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="Microsoft.VisualStudio.Web.CodeGeneration.Design" Version="6.0.0" />
  </ItemGroup>

</Project>
EOL
}

# 创建启动类
create_startup_files() {
    echo "创建启动类..."
    
    # Program.cs
    cat > DataManagementApp/Program.cs << 'EOL'
using DataManagementApp.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 添加服务到容器
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 注册基础服务
builder.Services.AddScoped(typeof(IBaseService<>), typeof(BaseService<,>));

// 注册特定服务
builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation();

builder.Services.AddRazorPages()
    .AddViewOptions(options =>
    {
        options.HtmlHelperOptions.ClientValidationEnabled = true;
    });

var app = builder.Build();

// 配置HTTP请求管道
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
EOL

    # appsettings.json
    cat > DataManagementApp/appsettings.json << 'EOL'
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=DataManagementApp;Trusted_Connection=True;MultipleActiveResultSets=true"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*"
}
EOL
}

# 主函数
main() {
    create_project_structure
    create_base_entity
    create_base_viewmodel
    create_ibase_service
    create_base_service
    create_base_controller
    create_base_views
    create_product_files
    create_project_file
    create_startup_files
    
    echo "项目结构创建完成！"
    echo "请执行以下命令完成设置："
    echo "1. cd DataManagementApp"
    echo "2. dotnet restore"
    echo "3. dotnet ef migrations add InitialCreate"
    echo "4. dotnet ef database update"
    echo "5. dotnet run"
}

# 执行主函数
main