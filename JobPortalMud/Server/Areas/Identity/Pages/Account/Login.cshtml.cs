// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using JobPortalMud.Server.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using JobPortalMud.Shared;
using System.Security.Claims;

namespace JobPortalMud.Server.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public LoginModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        [BindProperty]
        public Login Model { get; set; }

        [BindProperty]
        public string ReturnUrl { get; set; }
        [BindProperty]
        public IList<AuthenticationScheme> ExternalLogins { get; set; }
        public async Task OnGetAsync(string returnUrl = null)
        {

            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);


            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }


        public async Task<IActionResult> OnPostLoginAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(Model.UserName);
                if (await _userManager.IsEmailConfirmedAsync(user))
                {
                    var identityResult = await _signInManager.PasswordSignInAsync(user, Model.Password, Model.RememberMe, false);
                    if (identityResult.Succeeded)
                    {
                        var userRoles = await _userManager.GetRolesAsync(user);
                        var authClaims = new List<Claim>
                        {
                        new Claim(ClaimTypes.Name, user.UserName),
                        };

                        foreach (var userRole in userRoles)
                        {
                            authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                        }
                        if (returnUrl == null || returnUrl == "/")
                        {
                            return LocalRedirect(returnUrl);
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                            return Page();
                        }
                    }
                    else
                    {
                        TempData["AlertMessage"] = "Login credentials are incorrect...!";
                    }
                }
                else
                {
                    TempData["Message"] = "Email not confirmed...!";
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

                        var result = await _userManager.CreateAsync(user);
                        if (result.Succeeded)
                        {
                            if (!await _roleManager.RoleExistsAsync("Job Seeker"))
                                await _roleManager.CreateAsync(new IdentityRole("Job Seeker"));


                            if (await _roleManager.RoleExistsAsync("Job Seeker"))
                            {
                                await _userManager.AddToRoleAsync(user, "Job Seeker");
                            }
                        }
                    }

                    await _userManager.AddLoginAsync(user, info);
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return LocalRedirect(returnUrl);
                }

                TempData["AlertMessage"] = $"Email claim not received from: {info.LoginProvider}";
                return Page();
            }
        }
    }
}
