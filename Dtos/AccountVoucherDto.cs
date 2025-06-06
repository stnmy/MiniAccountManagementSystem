using MiniAccountManagementSystem.Data.Enums;

namespace MiniAccountManagementSystem.Dtos
{
    public class AccountVoucherDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? ParentId { get; set; }
        public AccountType Type { get; set; }
    }
}
