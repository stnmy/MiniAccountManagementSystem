using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MiniAccountManagementSystem.Dtos;
using MiniAccountManagementSystem.Interfaces;

namespace MiniAccountManagementSystem.Pages.Account
{
    [Authorize]
    public class ListModel : PageModel
    {
        private readonly IAccountRepository _accountRepository;

        public ListModel(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public List<AccountTreeDto> Accounts { get; set; } = new();

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

            Accounts = accountList;
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            try
            {
                await _accountRepository.DeleteAsync(id);
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToPage();
            }

            return RedirectToPage();
        }
    }
}
