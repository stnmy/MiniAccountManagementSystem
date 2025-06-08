using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MiniAccountManagementSystem.Data.Enums;
using MiniAccountManagementSystem.Dtos;
using MiniAccountManagementSystem.Interfaces;

namespace MiniAccountManagementSystem.Pages.Account
{
    public class EditModel : PageModel
    {
        private readonly IAccountRepository _accountRepository;

        public EditModel(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        [BindProperty]
        public AccountEditDto Account { get; set; }

        public SelectList ParentAccounts { get; set; }
        public SelectList AccountTypes { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var existingAccount = await _accountRepository.GetByIdAsync(id);

            if (existingAccount == null)
            {
                return NotFound();
            }

            Account = existingAccount;

            var allAccounts = await _accountRepository.GetAllAsync();
            var parentOptions = new List<SelectListItem>();

            foreach (var a in allAccounts)
            {
                if (a.Id != existingAccount.Id)
                {
                    parentOptions.Add(new SelectListItem
                    {
                        Value = a.Id.ToString(),
                        Text = a.Name
                    });
                }
            }

            parentOptions.Insert(0, new SelectListItem { Value = "", Text = "No Parent (Root Level)" });

            ParentAccounts = new SelectList(parentOptions, "Value", "Text", Account.ParentId?.ToString());

            AccountTypes = new SelectList(Enum.GetNames(typeof(AccountType)), Account.Type.ToString());

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                var allAccounts = await _accountRepository.GetAllAsync();
                var parentOptions = new List<SelectListItem>();

                foreach (var a in allAccounts)
                {
                    if (a.Id != Account.Id)
                    {
                        parentOptions.Add(new SelectListItem
                        {
                            Value = a.Id.ToString(),
                            Text = a.Name
                        });
                    }
                }

                parentOptions.Insert(0, new SelectListItem { Value = "", Text = "No Parent (Root Level)" });

                ParentAccounts = new SelectList(parentOptions, "Value", "Text", Account.ParentId?.ToString());
                AccountTypes = new SelectList(Enum.GetNames(typeof(AccountType)), Account.Type.ToString());

                if (!ModelState.IsValid)
                {
                    return Page();
                }

                await _accountRepository.UpdateAsync(Account);
                return RedirectToPage("./List");
            }
            catch (UnauthorizedAccessException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred while updating the account: {ex.Message}");
                return Page();
            }
        }

    }
}
