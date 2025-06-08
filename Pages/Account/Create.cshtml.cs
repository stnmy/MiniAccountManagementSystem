using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MiniAccountManagementSystem.Dtos;
using MiniAccountManagementSystem.Interfaces;

namespace MiniAccountManagementSystem.Pages.Account
{
    public class CreateModel : PageModel
    {
        private readonly IAccountRepository _accountRepository;

        public CreateModel(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        [BindProperty]
        public AccountCreateDto Input { get; set; } = new();

        public List<SelectListItem> ParentAccounts { get; set; } = new();

        public async Task OnGetAsync()
        {
            var accounts = await _accountRepository.GetAllAsync();
            var accountList = new List<AccountTreeDto>();
            foreach (var account in accounts)
            {
                accountList.Add(account);
            }

            for (int i = 0; i < accountList.Count - 1; i++)
            {
                for (int j = 0; j < accountList.Count - i - 1; j++)
                {
                    if (string.Compare(accountList[j].HierarchyPath, accountList[j + 1].HierarchyPath) > 0)
                    {
                        var temp = accountList[j];
                        accountList[j] = accountList[j + 1];
                        accountList[j + 1] = temp;
                    }
                }
            }

            foreach (var account in accountList)
            {
                ParentAccounts.Add(new SelectListItem
                {
                    Value = account.Id.ToString(),
                    Text = CreateIndentedText(account.Level, account.Name)
                });
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                await _accountRepository.CreateAsync(Input);
                return RedirectToPage("/Account/List");
            }
            catch (UnauthorizedAccessException ex)
            {

                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }
        }

        private string CreateIndentedText(int level, string name)
        {
            string indentation = "";
            for (int i = 0; i < level * 2; i++)
            {
                indentation += "─";
            }
            return $"{indentation} {name}";
        }
    }
}
