using MiniAccountManagementSystem.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace MiniAccountManagementSystem.Dtos
{
    public class AccountEditDto
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public int? ParentId { get; set; }
        public AccountType Type { get; set; }
    }
}
