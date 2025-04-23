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
