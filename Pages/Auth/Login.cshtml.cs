using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MiniAccountManagementSystem.Models.Identity;
using System.ComponentModel.DataAnnotations;

namespace MiniAccountManagementSystem.Pages.Auth
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;

        public LoginModel(SignInManager<User> signInManager)
        {
            _signInManager = signInManager;
        }

        [BindProperty]
        public LoginInput Input { get; set; } = new();

        public string? LoginError { get; set; }

        public class LoginInput
        {
            [Required]
            public string Username { get; set; } = string.Empty;

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;

        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var result = await _signInManager.PasswordSignInAsync(
                Input.Username,
                Input.Password,
                isPersistent: false,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return RedirectToPage("/Index"); // or Dashboard page
            }

            LoginError = "Invalid username or password.";
            return Page();
        }
    }
}
