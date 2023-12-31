﻿// Licensed to the .NET Foundation under one or more agreements.
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
using Lekkerbek.Web.Models;
using Lekkerbek.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace Lekkerbek.Web.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly ICustomerService _customerService;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            IUserStore<IdentityUser> userStore,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            ICustomerService customerService)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _customerService = customerService;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            [Required]
            [Display(Name = "Naam")]
            public string FName { get; set; }
            [Required]
            [Display(Name = "Familienaam")]
            public string LName { get; set; }
            [Required]
            [Display(Name = "GSM")]
            [DataType(DataType.PhoneNumber)]
            public string? PhoneNumber { get; set; }
            [Required]
            [Display(Name = "Geboortedatum")]
            [DataType(DataType.Date)]
            public DateTime? Birthday { get; set; }

            [StringLength(30, ErrorMessage = "Bedrijfsnaam mag maar 30 tekens bevatten")]
            [Display(Name = "Bedrijfsnaam")]
            public string? FirmName { get; set; } = string.Empty;

            [StringLength(30, ErrorMessage = "Contactpersoon mag maar 30 tekens bevatten")]
            [Display(Name = "Contactpersoon")]
            public string? ContactPerson { get; set; } = string.Empty;
            [Required]
            [StringLength(450, ErrorMessage = "Straat mag maar 450 tekens bevatten")]
            [Display(Name = "Straat")]
            public string? StreetName { get; set; } = string.Empty;
            [Required]
            [StringLength(20, ErrorMessage = "Stad mag maar 20 tekens bevatten")]
            [Display(Name = "Stad")]
            public string? City { get; set; } = string.Empty;
            [Required]
            [Display(Name = "Postcode")]
            public string? PostalCode { get; set; }

            [MaxLength(2)]
            [Display(Name = "BTW")]
            [RegularExpression("^[a-zA-Z]{2}", ErrorMessage = "Enkel geldige landcodes mogen ingevuld worden")]
            public string? Btw { get; set; } = string.Empty;

            [Display(Name = "BTW nummer")]
            [RegularExpression("^[0][0-9]{9}", ErrorMessage = "Het btw nummer moet beginnen met een 0 en dan 9 cijfers bevatten")]
            public string? BtwNumber { get; set; }

            //Foreign Key van Preferred Dish
            [Display(Name = "Favoriete Gerechten")]
            public int? PreferredDishId { get; set; }
            [Display(Name = "Favoriete Gerechten")]
            public virtual PreferredDish PreferredDish { get; set; }
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "De {0} mag minimum {2} en maximum {1} tekens bevatten.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Wachtwoord")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Bevestig wachtwoord")]
            [Compare("Password", ErrorMessage = "Het wachtwoord en het bevestigingswachtwoord komen niet overeen.")]
            public string ConfirmPassword { get; set; }
        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ViewData["PreferredDishId"] = _customerService.GetPreferredDishes();
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            ViewData["PreferredDishId"] = _customerService.GetPreferredDishes();
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);


                if (result.Succeeded)
                {
                    var userAddRole = await _userManager.FindByEmailAsync(Input.Email);
                    await _userManager.AddToRoleAsync(userAddRole, "Customer");
                    Customer customer = new Customer
                    {
                        FName = Input.FName,
                        LName = Input.LName,
                        Email = Input.Email,
                        PreferredDishId = Input.PreferredDishId,
                        PhoneNumber = Input.PhoneNumber,
                        Birthday = Input.Birthday,
                        FirmName = Input.FirmName,
                        ContactPerson = Input.ContactPerson,
                        StreetName = Input.StreetName,
                        City = Input.City,
                        PostalCode = Input.PostalCode,
                        Btw = Input.Btw,
                        BtwNumber = Input.BtwNumber,
                        IdentityUser = userAddRole
                    };
                    _customerService.Create(customer);
                    _logger.LogInformation("User created a new account with password.");

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Bevestig uw email",
                        $"Hallo Meneer/Mevrouv{customer.LName},<br> Als de Lekkerbek familie, we zijn blij dat u klant bij ons bent geworden. Bevestig uw account door <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'> hier te klikken.</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private IdentityUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<IdentityUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'. " +
                    $"Ensure that '{nameof(IdentityUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<IdentityUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<IdentityUser>)_userStore;
        }
    }
}
