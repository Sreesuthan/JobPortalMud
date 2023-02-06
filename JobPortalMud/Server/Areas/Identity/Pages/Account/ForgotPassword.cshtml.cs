using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using JobPortalMud.Server.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using JobPortalMud.Shared;
using MimeKit.Text;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace JobPortalMud.Server.Areas.Identity.Pages.Account
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;

        [BindProperty]
        public ForgotPassword Model { get; set; }
        public ForgotPasswordModel(UserManager<ApplicationUser> userManager, IConfiguration config)
        {
            _userManager = userManager;
            _config = config;
        }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Model.Email);
                if (user != null && await _userManager.IsEmailConfirmedAsync(user))
                {
                    var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                    String? link = Url.Page("./ResetPassword", null, new { code = code }, Request.Scheme);
                    SendEmail(link);

                    TempData["AlertMessage"] = "We have emailed a link for reset password.";

                    //return RedirectToPage("ResetPassword", new { code = code });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Email not registered yet...");
                    return Page();
                }
            }
            return Page();
        }

        void SendEmail(string? body)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_config.GetSection("EmailUsername").Value));
            email.To.Add(MailboxAddress.Parse(Model.Email));
            email.Subject = "Reset your Password";
            email.Body = new TextPart(TextFormat.Text) { Text = body };

            using var smtp = new SmtpClient();
            smtp.Connect(_config.GetSection("EmailHost").Value, 587, SecureSocketOptions.StartTls);
            smtp.Authenticate(_config.GetSection("EmailUsername").Value, _config.GetSection("EmailPassword").Value);
            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }
}
