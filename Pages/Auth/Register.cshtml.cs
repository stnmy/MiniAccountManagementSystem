using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MiniAccountManagementSystem.Models.Identity;
using System.ComponentModel.DataAnnotations;

namespace MiniAccountManagementSystem.Pages.Auth
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<User> _userManager;


        public RegisterModel(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty]
        public RegisterInput Input { get; set; } = new();

        public class RegisterInput
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

            var user = new User { Username = Input.Username };
            var result = await _userManager.CreateAsync(user, Input.Password);

            if (result.Succeeded)
            {
                return RedirectToPage("/Auth/Login");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return Page();
        }
    }
}
