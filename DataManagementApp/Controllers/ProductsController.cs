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
