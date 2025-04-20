// Models/User.cs
using System.ComponentModel.DataAnnotations;

namespace UserManagementSystem.Models
{
    public class User
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Username { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        public string Password { get; set; }
        
        [StringLength(20)]  // 移除了 [Required] 特性
        public string? Phone { get; set; }  // 添加 ? 表示可为null
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;
    }
}