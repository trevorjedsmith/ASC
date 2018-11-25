﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ASC.Web.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ASC.Web.Models;
using ASC.Web.Models.AccountViewModels;
using ASC.Web.Models.BaseTypes;
using ASC.Web.Services;
using ASC.Web.Utilities;

namespace ASC.Web.Controllers
{
  [Authorize]
  public class AccountController : Controller
  {
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IEmailSender _emailSender;
    private readonly ISmsSender _smsSender;
    private readonly ILogDataOperations _dbLogger;
    private readonly IDatabaseHelper _dbHelper;
    private readonly ILogger _logger;
    private readonly string _externalCookieScheme;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IOptions<IdentityCookieOptions> identityCookieOptions,
        IEmailSender emailSender,
        ISmsSender smsSender,
        ILoggerFactory loggerFactory,
        ILogDataOperations dbLogger,
        IDatabaseHelper dbHelper)
    {
      _userManager = userManager;
      _signInManager = signInManager;
      _externalCookieScheme = identityCookieOptions.Value.ExternalCookieAuthenticationScheme;
      _emailSender = emailSender;
      _smsSender = smsSender;
      _dbLogger = dbLogger;
      _dbHelper = dbHelper;
      _logger = loggerFactory.CreateLogger<AccountController>();
    }

    //
    // GET: /Account/Login
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Login(string returnUrl = null)
    {
      // Clear the existing external cookie to ensure a clean login process
      await HttpContext.Authentication.SignOutAsync(_externalCookieScheme);

      ViewData["ReturnUrl"] = returnUrl;
      return View();
    }

    //
    // POST: /Account/Login
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
    {
      ViewData["ReturnUrl"] = returnUrl;
      if (ModelState.IsValid)
      {
        // This doesn't count login failures towards account lockout
        // To enable password failures to trigger account lockout, set lockoutOnFailure: true

        var user = await _userManager.FindByEmailAsync(model.Email);

        if (user == null)
        {
          ModelState.AddModelError(string.Empty, "Invalid login attempt");
          return View(model);
        }

        var isActive = await _dbHelper.GetClaim(user.Id, "IsActive");
        if (isActive != null && isActive.ClaimValue != "True")
        {
          ModelState.AddModelError(string.Empty, "Account has been locked.");
          return View(model);
        }


        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
        if (result.Succeeded)
        {
          _logger.LogInformation(1, "User logged in.");
          if (!String.IsNullOrWhiteSpace(returnUrl))
          {
            return RedirectToLocal(returnUrl);
          }
          else
          {
            return RedirectToAction("Dashboard", "Dashboard");
          }

        }
        if (result.RequiresTwoFactor)
        {
          return RedirectToAction(nameof(SendCode), new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
        }
        if (result.IsLockedOut)
        {
          _logger.LogWarning(2, "User account locked out.");
          return View("Lockout");
        }
        else
        {
          ModelState.AddModelError(string.Empty, "Invalid login attempt.");
          return View(model);
        }
      }

      // If we got this far, something failed, redisplay form
      return View(model);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> InitiateResetPassword(string email)
    {

      //Find Customs Claims
      if (string.IsNullOrEmpty(email))
      {
        //This should never happen
        throw new Exception("No email has been supplied");
      }

      var user = await _userManager.FindByEmailAsync(email);

      // Generate User code
      var code = await _userManager.GeneratePasswordResetTokenAsync(user);
      var callbackUrl = Url.Action(nameof(ResetPassword), "Account", new
      {
        userId = user.Id,
        code = code
      }, protocol: HttpContext.Request.Scheme);
      // Send Email
      await _emailSender.SendEmailAsync(email, "Reset Password",
        $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>");
      return View("ResetPasswordConfirmation");
    }

    //
    // GET: /Account/Register
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register(string returnUrl = null)
    {
      ViewData["ReturnUrl"] = returnUrl;
      return View();
    }

    //
    // POST: /Account/Register
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
    {
      ViewData["ReturnUrl"] = returnUrl;
      if (ModelState.IsValid)
      {
        var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
        var result = await _userManager.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
          // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=532713
          // Send an email with this link
          //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
          //var callbackUrl = Url.Action(nameof(ConfirmEmail), "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
          //await _emailSender.SendEmailAsync(model.Email, "Confirm your account",
          //    $"Please confirm your account by clicking this link: <a href='{callbackUrl}'>link</a>");
          await _signInManager.SignInAsync(user, isPersistent: false);
          _logger.LogInformation(3, "User created a new account with password.");
          return RedirectToLocal(returnUrl);
        }
        AddErrors(result);
      }

      // If we got this far, something failed, redisplay form
      return View(model);
    }

    //
    // POST: /Account/Logout
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
      await _signInManager.SignOutAsync();
      _logger.LogInformation(4, "User logged out.");
      return RedirectToAction(nameof(HomeController.Index), "Home");
    }

    //
    // POST: /Account/ExternalLogin
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public IActionResult ExternalLogin(string provider, string returnUrl = null)
    {
      // Request a redirect to the external login provider.
      var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { ReturnUrl = returnUrl });
      var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
      return Challenge(properties, provider);
    }

    //
    // GET: /Account/ExternalLoginCallback
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
    {
      if (remoteError != null)
      {
        ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
        return View(nameof(Login));
      }
      var info = await _signInManager.GetExternalLoginInfoAsync();
      if (info == null)
      {
        return RedirectToAction(nameof(Login));
      }

      // Sign in the user with this external login provider if the user already has a login.
      var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
      if (result.Succeeded)
      {
        _logger.LogInformation(5, "User logged in with {Name} provider.", info.LoginProvider);
        return RedirectToLocal(returnUrl);
      }
      if (result.RequiresTwoFactor)
      {
        return RedirectToAction(nameof(SendCode), new { ReturnUrl = returnUrl });
      }
      if (result.IsLockedOut)
      {
        return View("Lockout");
      }
      else
      {
        // If the user does not have an account, then ask the user to create an account.
        ViewData["ReturnUrl"] = returnUrl;
        ViewData["LoginProvider"] = info.LoginProvider;
        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = email });
      }
    }

    //
    // POST: /Account/ExternalLoginConfirmation
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl = null)
    {
      if (ModelState.IsValid)
      {
        // Get the information about the user from the external login provider
        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
          return View("ExternalLoginFailure");
        }
        var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
        var result = await _userManager.CreateAsync(user);
        if (result.Succeeded)
        {
          result = await _userManager.AddLoginAsync(user, info);
          if (result.Succeeded)
          {
            await _signInManager.SignInAsync(user, isPersistent: false);
            _logger.LogInformation(6, "User created an account using {Name} provider.", info.LoginProvider);
            return RedirectToLocal(returnUrl);
          }
        }
        AddErrors(result);
      }

      ViewData["ReturnUrl"] = returnUrl;
      return View(model);
    }

    // GET: /Account/ConfirmEmail
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmEmail(string userId, string code)
    {
      if (userId == null || code == null)
      {
        return View("Error");
      }
      var user = await _userManager.FindByIdAsync(userId);
      if (user == null)
      {
        return View("Error");
      }
      var result = await _userManager.ConfirmEmailAsync(user, code);
      return View(result.Succeeded ? "ConfirmEmail" : "Error");
    }

    //
    // GET: /Account/ForgotPassword
    [HttpGet]
    [AllowAnonymous]
    public IActionResult ForgotPassword()
    {
      return View();
    }

    //
    // POST: /Account/ForgotPassword
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
      if (ModelState.IsValid)
      {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
        {
          // Don't reveal that the user does not exist or is not confirmed
          return View("ResetPasswordConfirmation");
        }
        // Send an email with this link
        var code = await _userManager.GeneratePasswordResetTokenAsync(user);
        var callbackUrl = Url.Action(nameof(ResetPassword), "Account", new
        {
          userId = user.
            Id,
          code = code
        }, protocol: HttpContext.Request.Scheme);
        await _emailSender.SendEmailAsync(model.Email, "Reset Password",
          $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>");
        return View("ResetPasswordConfirmation");
      }
      // If we got this far, something failed, redisplay form
      return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> ServiceEngineers()
    {
      try
      {
        var serviceEngineers = await _userManager.GetUsersInRoleAsync(Roles.Engineer.ToString());
        // Hold all service engineers in session
        HttpContext.Session.SetSession("ServiceEngineers", serviceEngineers);

        var svm = new ServiceEngineerViewModel
        {
          ServiceEngineers = serviceEngineers == null ? null : serviceEngineers.ToList(),
          Registration = new ServiceEngineerRegistrationViewModel() { IsEdit = false }
        };

        if (svm.ServiceEngineers.Any())
        {
          foreach (var srcEngineer in svm.ServiceEngineers)
          {
            var claims = await _dbHelper.GetUserClaims(srcEngineer.Id);
            svm.ServiceEngineerClaims.AddRange(claims);
          }
        }
        return View(svm);
      }
      catch (Exception ex)
      {
        _logger.LogError($"Error in [HTTPGET]: {nameof(ServiceEngineers)}",ex);
        return BadRequest();
      }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles="Admin")]
    public async Task<IActionResult> ServiceEngineers(ServiceEngineerViewModel serviceEngineer)
    {
      try
      {
        serviceEngineer.ServiceEngineers = HttpContext.Session.GetSession<List<ApplicationUser>>
         ("ServiceEngineers");

        if (!ModelState.IsValid)
        {
          return View(serviceEngineer);
        }

        if (serviceEngineer.Registration.IsEdit)
        {
          var user = await _userManager.FindByEmailAsync(serviceEngineer.Registration.Email);
          user.UserName = serviceEngineer.Registration.UserName;
          IdentityResult result = await _userManager.UpdateAsync(user);

          if (!result.Succeeded)
          {
            result.Errors.ToList().ForEach(p => ModelState.AddModelError("", p.Description));
            return View(serviceEngineer);
          }

          //Update Password: Test when at home
          var token = await _userManager.GeneratePasswordResetTokenAsync(user);
          IdentityResult passwordResult =
            await _userManager.ResetPasswordAsync(user, token, serviceEngineer.Registration.Password);

          if (!result.Succeeded)
          {
            passwordResult.Errors.ToList().ForEach(p => ModelState.AddModelError("", p.Description));
            return View(serviceEngineer);
          }

          //Update Custom Claims
          user = await _userManager.FindByEmailAsync(serviceEngineer.Registration.Email);
          await _dbHelper.UpdateClaimValues(user.Id, "IsActive", serviceEngineer.Registration.IsActive.ToString());
        }
        else
        {
          //Create User and Custom Claims
          ApplicationUser user = new ApplicationUser
          {
            UserName = serviceEngineer.Registration.UserName,
            Email = serviceEngineer.Registration.Email,
            EmailConfirmed = true
          };

          var result = await _userManager.CreateAsync(user, serviceEngineer.Registration.Password);
          if (result.Succeeded)
          {
            var userClaimsAdd = await _userManager.FindByEmailAsync(serviceEngineer.Registration.Email);

            List<CustomUserClaims> claimList = new List<CustomUserClaims>();
            CustomUserClaims c1 = new CustomUserClaims
            {
              UserId = userClaimsAdd.Id,
              ClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress",
              ClaimValue = serviceEngineer.Registration.Email
            };
            CustomUserClaims c2 = new CustomUserClaims
            {
              UserId = userClaimsAdd.Id,
              ClaimType = "IsActive",
              ClaimValue = serviceEngineer.Registration.IsActive.ToString()
            };
            //Add Claims
            claimList.Add(c1);
            claimList.Add(c2);
            await _dbHelper.AddClaims(claimList);

            //Add Roles
            var roleResult = await _userManager.AddToRoleAsync(user, Roles.Engineer.ToString());
            if (!roleResult.Succeeded)
            {
              result.Errors.ToList().ForEach(p => ModelState.AddModelError("",
                p.Description));
              return View(serviceEngineer);
            }

            if (serviceEngineer.Registration.IsActive)
            {
              await _emailSender.SendEmailAsync(serviceEngineer.Registration.Email,
                  "Account Created/Modified", $"Email : {serviceEngineer.Registration.Email}/n Passowrd : {serviceEngineer.Registration.Password}");
            }
            else
            {
              await _emailSender.SendEmailAsync(serviceEngineer.Registration.Email,
                "Account Deactivated",
                $"Your account has been deactivated.");
            }
          }
          else
          {
            result.Errors.ToList().ForEach(p => ModelState.AddModelError("",
              p.Description));
            return View(serviceEngineer);
          }

        }

        return RedirectToAction("ServiceEngineers");
      }
      catch (Exception ex)
      {
        _logger.LogError($"Error in [HTTPPOST]: {nameof(ServiceEngineers)}", ex);
        return BadRequest();
      }
    }


    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Customers()
    {
      var users = await _userManager.GetUsersInRoleAsync(Roles.User.ToString());

      // Hold all customers in session
      HttpContext.Session.SetSession("Customers", users);

      var svm = new CustomersViewModel()
      {
        Customers = users == null ? null : users.ToList(),
        Registration = new CustomerRegistrationViewModel()
      };

      if (svm.Customers.Any())
      {
        foreach (var cust in svm.Customers)
        {
          var claims = await _dbHelper.GetUserClaims(cust.Id);
          svm.CustomerClaims.AddRange(claims);
        }
      }
      return View(svm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Customers(CustomersViewModel customer)
    {
      customer.Customers = HttpContext.Session.GetSession<List<ApplicationUser>>("Customers");
      if (!ModelState.IsValid)
      {
        return View(customer);
      }

      var user = await _userManager.FindByEmailAsync(customer.Registration.Email);

      // Update claims

      List<CustomUserClaims> claimList = new List<CustomUserClaims>();

      var claims = await _dbHelper.GetUserClaims(user.Id);

      if (claims.Any())
      {
        await _dbHelper.UpdateClaimValues(user.Id, "IsActive", customer.Registration.IsActive.ToString());
      }
      else
      {
        CustomUserClaims cust = new CustomUserClaims
        {
          UserId = user.Id,
          ClaimType = "IsActive",
          ClaimValue = customer.Registration.IsActive.ToString()
        };
        //Add Claims
        claimList.Add(cust);
        await _dbHelper.AddClaims(claimList);
      }

      if (!customer.Registration.IsActive)
      {
        await _emailSender.SendEmailAsync(customer.Registration.Email,
          "Account Deativated",
          $"Your account has been Deactivated!!!");
      }

      return RedirectToAction("Customers");
    }

    //
    // GET: /Account/ForgotPasswordConfirmation
    [HttpGet]
    [AllowAnonymous]
    public IActionResult ForgotPasswordConfirmation()
    {
      return View();
    }

    //
    // GET: /Account/ResetPassword
    [HttpGet]
    [AllowAnonymous]
    public IActionResult ResetPassword(string code = null)
    {
      return code == null ? View("Error") : View();
    }

    //
    // POST: /Account/ResetPassword
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
      if (!ModelState.IsValid)
      {
        return View(model);
      }
      var user = await _userManager.FindByEmailAsync(model.Email);
      if (user == null)
      {
        // Don't reveal that the user does not exist
        return RedirectToAction(nameof(AccountController.ResetPasswordConfirmation), "Account");
      }
      var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
      if (result.Succeeded)
      {
        return RedirectToAction(nameof(AccountController.ResetPasswordConfirmation), "Account");
      }
      AddErrors(result);
      return View();
    }

    //
    // GET: /Account/ResetPasswordConfirmation
    [HttpGet]
    [AllowAnonymous]
    public IActionResult ResetPasswordConfirmation()
    {
      return View();
    }

    //
    // GET: /Account/SendCode
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult> SendCode(string returnUrl = null, bool rememberMe = false)
    {
      var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
      if (user == null)
      {
        return View("Error");
      }
      var userFactors = await _userManager.GetValidTwoFactorProvidersAsync(user);
      var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
      return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
    }

    //
    // POST: /Account/SendCode
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SendCode(SendCodeViewModel model)
    {
      if (!ModelState.IsValid)
      {
        return View();
      }

      var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
      if (user == null)
      {
        return View("Error");
      }

      // Generate the token and send it
      var code = await _userManager.GenerateTwoFactorTokenAsync(user, model.SelectedProvider);
      if (string.IsNullOrWhiteSpace(code))
      {
        return View("Error");
      }

      var message = "Your security code is: " + code;
      if (model.SelectedProvider == "Email")
      {
        await _emailSender.SendEmailAsync(await _userManager.GetEmailAsync(user), "Security Code", message);
      }
      else if (model.SelectedProvider == "Phone")
      {
        await _smsSender.SendSmsAsync(await _userManager.GetPhoneNumberAsync(user), message);
      }

      return RedirectToAction(nameof(VerifyCode), new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
    }

    //
    // GET: /Account/VerifyCode
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> VerifyCode(string provider, bool rememberMe, string returnUrl = null)
    {
      // Require that the user has already logged in via username/password or external login
      var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
      if (user == null)
      {
        return View("Error");
      }
      return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
    }

    //
    // POST: /Account/VerifyCode
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> VerifyCode(VerifyCodeViewModel model)
    {
      if (!ModelState.IsValid)
      {
        return View(model);
      }

      // The following code protects for brute force attacks against the two factor codes.
      // If a user enters incorrect codes for a specified amount of time then the user account
      // will be locked out for a specified amount of time.
      var result = await _signInManager.TwoFactorSignInAsync(model.Provider, model.Code, model.RememberMe, model.RememberBrowser);
      if (result.Succeeded)
      {
        return RedirectToLocal(model.ReturnUrl);
      }
      if (result.IsLockedOut)
      {
        _logger.LogWarning(7, "User account locked out.");
        return View("Lockout");
      }
      else
      {
        ModelState.AddModelError(string.Empty, "Invalid code.");
        return View(model);
      }
    }

    //
    // GET /Account/AccessDenied
    [HttpGet]
    public IActionResult AccessDenied()
    {
      return View();
    }

    #region Helpers

    private void AddErrors(IdentityResult result)
    {
      foreach (var error in result.Errors)
      {
        ModelState.AddModelError(string.Empty, error.Description);
      }
    }

    private IActionResult RedirectToLocal(string returnUrl)
    {
      if (Url.IsLocalUrl(returnUrl))
      {
        return Redirect(returnUrl);
      }
      else
      {
        return RedirectToAction(nameof(HomeController.Index), "Home");
      }
    }

    #endregion
  }
}
