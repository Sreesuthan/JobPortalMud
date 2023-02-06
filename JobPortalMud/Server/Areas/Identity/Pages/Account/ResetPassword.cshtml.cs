using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using JobPortalMud.Server.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using JobPortalMud.Shared;

namespace JobPortalMud.Server.Areas.Identity.Pages.Account
{
    public class ResetPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        [BindProperty]
        public ResetPassword Model { get; set; }

        [BindProperty]
        public string Code { get; set; } = string.Empty;

        public ResetPasswordModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public IActionResult OnGet(string? code = null)
        {
            if (code == null)
            {
                return BadRequest("Code not provided");
            }
            else
            {
                Code = code;
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                String token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(Code ?? String.Empty));
                var user = await _userManager.FindByEmailAsync(Model.Email);
                var result = await _userManager.ResetPasswordAsync(user, token, Model.UserPassword);

                if (result.Succeeded)
                {
                    TempData["AlertMessage"] = "Password reset successfully...!";
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                return Page();
            }
            return Page();
        }
    }
}
