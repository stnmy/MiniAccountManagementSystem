using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MiniAccountManagementSystem.Dtos;
using MiniAccountManagementSystem.Interfaces;

namespace MiniAccountManagementSystem.Pages.Voucher
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly IVoucherRepository _voucherRepository;

        public CreateModel(IVoucherRepository voucherRepository)
        {
            _voucherRepository = voucherRepository;
        }

        [BindProperty]
        public VoucherDto Voucher { get; set; } = new();

        public List<SelectListItem> LeafAccounts { get; set; } = new();

        public async Task OnGetAsync()
        {
            var accounts = await _voucherRepository.GetLeafAccountsAsync();
            LeafAccounts = new List<SelectListItem>();

            foreach (var account in accounts)
            {
                LeafAccounts.Add(new SelectListItem
                {
                    Value = account.Id.ToString(),
                    Text = account.Name
                });
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {

            if (!ModelState.IsValid)
            {
                var accounts = await _voucherRepository.GetLeafAccountsAsync();
                LeafAccounts = new List<SelectListItem>();

                foreach (var account in accounts)
                {
                    LeafAccounts.Add(new SelectListItem
                    {
                        Value = account.Id.ToString(),
                        Text = account.Name
                    });
                }

                return Page();
            }

            try
            {
                await _voucherRepository.SaveVoucherAsync(Voucher);
                return RedirectToPage("/Account/List");
            }
            catch (UnauthorizedAccessException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

                var accounts = await _voucherRepository.GetLeafAccountsAsync();
                LeafAccounts = new List<SelectListItem>();

                foreach (var account in accounts)
                {
                    LeafAccounts.Add(new SelectListItem
                    {
                        Value = account.Id.ToString(),
                        Text = account.Name
                    });
                }

                return Page();
            }
        }
    }
}
