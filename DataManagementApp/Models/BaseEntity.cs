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
