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
