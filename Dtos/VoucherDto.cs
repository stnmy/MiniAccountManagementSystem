using MiniAccountManagementSystem.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace MiniAccountManagementSystem.Dtos
{
    public class VoucherDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Date is required.")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Reference is required.")]
        public string ReferenceNo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Voucher Type is required.")]
        public VoucherType Type { get; set; }

        [Required]
        public List<VoucherEntryDto> Entries { get; set; } = new List<VoucherEntryDto>();
    }
}
