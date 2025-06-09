namespace MiniAccountManagementSystem.Dtos
{
    public class VoucherEntryExportDto
    {
        public DateTime VoucherDate { get; set; }
        public string ReferenceNo { get; set; } = string.Empty;
        public string VoucherType { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
    }
}
