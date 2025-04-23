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
