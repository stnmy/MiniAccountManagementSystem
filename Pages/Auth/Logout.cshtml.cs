using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MiniAccountManagementSystem.Models.Identity;

namespace MiniAccountManagementSystem.Pages.Auth
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;

        public LogoutModel(SignInManager<User> signInManager)
        {
            _signInManager = signInManager;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            await _signInManager.SignOutAsync();
            return RedirectToPage("/Auth/Login");
        }

    }
}
