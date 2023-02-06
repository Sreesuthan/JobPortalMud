// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using JobPortalMud.Server.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using JobPortalMud.Shared;
using System.Security.Claims;
using MailKit.Security;
using MimeKit.Text;
using MimeKit;
using static Org.BouncyCastle.Math.EC.ECCurve;
using MailKit.Net.Smtp;

namespace JobPortalMud.Server.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _config;

        [BindProperty]
        public Register Model { get; set; }

        [BindProperty]
        public string ReturnUrl { get; set; }
        [BindProperty]
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public RegisterModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager,RoleManager<IdentityRole> roleManager, IConfiguration config)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _config = config;
            _signInManager = signInManager;
        }
        public async Task OnGetAsync(string returnUrl = null)
        {

            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);


            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                //var model = new Register
                //{
                //    Email = "sree@gmail.com",
                //    Username = "Sree",
                //    Password = "Sree@123",
                //    ConfirmPassword = "Sree@123",
                //    Role = "Admin"
                //};
                if (Model.Agreement)
                {
                    var user = new ApplicationUser()
                    {
                        UserName = Model.Username,
                        Email = Model.Email
                    };
                    var result = await _userManager.CreateAsync(user, Model.Password);
                    if (result.Succeeded)
                    {
                        if (!await _roleManager.RoleExistsAsync(Model.Role))
                            await _roleManager.CreateAsync(new IdentityRole(Model.Role));


                        if (await _roleManager.RoleExistsAsync(Model.Role))
                        {
                            await _userManager.AddToRoleAsync(user, Model.Role);
                        }

                        ////If uncomment it. It dirctly singin to application without login page
                        ////await _signInManager.SignInAsync(user, false);
                        ////return RedirectToPage("Index");

                        //TempData["AlertMessage"] = "Registered Successfully...!";
                        //return Page();

                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                        String link = Url.Page("./ConfirmEmail", null, new { userId = user.Id, code = code }, Request.Scheme);
                        SendEmail(link);

                        TempData["AlertMessage"] = "We have emailed a link for confirm account.";
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "You must accept the terms of service to continue.");
                    return Page();
                }
            }
            return Page();
        }

        public IActionResult OnPostExternalLoginAsync(string provider, string returnUrl)
        {
            var redirectUrl = Url.Page("./Login", "ExternalLoginCallback", new { ReturnUrl = returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> OnGetExternalLoginCallbackAsync(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");

                return Page();
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ModelState.AddModelError(string.Empty, "Error loading external login information.");

                return Page();
            }

            var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

            if (signInResult.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }

            else
            {
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);

                if (email != null)
                {
                    var user = await _userManager.FindByEmailAsync(email);

                    if (user == null)
                    {
                        user = new ApplicationUser
                        {
                            UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
                            Email = info.Principal.FindFirstValue(ClaimTypes.Email)
                        };

                        await _userManager.CreateAsync(user);
                    }

                    await _userManager.AddLoginAsync(user, info);
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return LocalRedirect(returnUrl);
                }

                TempData["AlertMessage"] = $"Email claim not received from: {info.LoginProvider}";
                return Page();
            }
        }

        void SendEmail(string body)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_config.GetSection("EmailUsername").Value));
            email.To.Add(MailboxAddress.Parse(Model.Email));
            email.Subject = "Confirm your Email";
            email.Body = new TextPart(TextFormat.Text) { Text = body };

            using var smtp = new SmtpClient();
            smtp.Connect(_config.GetSection("EmailHost").Value, 587, SecureSocketOptions.StartTls);
            smtp.Authenticate(_config.GetSection("EmailUsername").Value, _config.GetSection("EmailPassword").Value);
            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }
}
