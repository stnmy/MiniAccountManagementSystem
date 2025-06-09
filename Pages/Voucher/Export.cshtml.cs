using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MiniAccountManagementSystem.Interfaces;
using MiniAccountManagementSystem.Repository;
using System.Text;

namespace MiniAccountManagementSystem.Pages.Voucher
{
    public class ExportModel : PageModel
    {
        private readonly IVoucherRepository _voucherRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IWebHostEnvironment _env;

        public ExportModel(IVoucherRepository voucherRepository, IAccountRepository accountRepository, IWebHostEnvironment env)
        {
            _voucherRepository = voucherRepository;
            _accountRepository = accountRepository;
            _env = env;
        }

        [BindProperty]
        public int SelectedAccountId { get; set; }

        public List<SelectListItem> Accounts { get; set; } = new();
        public string? CsvFileName { get; set; }

        public async Task OnGetAsync()
        {
            var leafAccounts = await _accountRepository.GetAllAsync();
            Accounts = leafAccounts
                .Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.Name })
                .ToList();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await OnGetAsync();

            if (SelectedAccountId <= 0)
            {
                ModelState.AddModelError(string.Empty, "Please select an account.");
                return Page();
            }

            var entries = await _voucherRepository.GetVoucherEntriesByAccountAsync(SelectedAccountId);

            var fileName = $"VoucherEntries_{SelectedAccountId}_{DateTime.Now:yyyyMMddHHmmss}.csv";
            var filePath = Path.Combine("wwwroot", "exports");
            Directory.CreateDirectory(filePath);
            var fullFilePath = Path.Combine(filePath, fileName);

            var csv = new StringBuilder();
            csv.AppendLine("VoucherDate,ReferenceNo,VoucherType,AccountName,Debit,Credit");

            foreach (var entry in entries)
            {
                csv.AppendLine($"{entry.VoucherDate:yyyy-MM-dd},{entry.ReferenceNo},{entry.VoucherType},{entry.AccountName},{entry.Debit},{entry.Credit}");
            }

            await System.IO.File.WriteAllTextAsync(fullFilePath, csv.ToString());
            CsvFileName = $"/exports/{fileName}";

            return Page();
        }
    }
}
