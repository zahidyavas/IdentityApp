using IdentityApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace IdentityApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;
        private IEmailSender _emailSender;
        public AccountController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, SignInManager<AppUser> signInManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if(ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if(user != null)
                {
                    await _signInManager.SignOutAsync();

                    if(!await _userManager.IsEmailConfirmedAsync(user))
                    {
                        ModelState.AddModelError("", "Please confirm your account");
                        return View(model);
                    }

                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe,true);

                    if(result.Succeeded)
                    {
                        await _userManager.ResetAccessFailedCountAsync(user);
                        await _userManager.SetLockoutEndDateAsync(user, null);

                        return RedirectToAction("Index", "Home");
                    }
                    else if (result.IsLockedOut) {
                        var lockoutDate = await _userManager.GetLockoutEndDateAsync(user);

                        if (lockoutDate.HasValue)
                        {
                            var timeleft = lockoutDate.Value - DateTimeOffset.UtcNow;
                            var minutes = Math.Ceiling(timeleft.TotalMinutes);
                            ModelState.AddModelError("", $"Your account has been locked. Please try again in {minutes} minutes");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Your account has been locked.");
                        }
                    } else
                    {
                        ModelState.AddModelError("", "Your password is wrong");
                    }
                } else
                {
                    ModelState.AddModelError("", "No account was found for this email address");
                }
            }
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(CreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser
                {
                    UserName = model.FullName,
                    Email = model.Email,
                    FullName = model.FullName,
                };

                var result = await _userManager.CreateAsync(user, model.Password ?? string.Empty);
                if (result.Succeeded)
                {
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var url = Url.Action("ConfirmEmail", "Account", new {user.Id, token});

                    // email
                    await _emailSender.SendEmailAsync(user.Email ?? "", "Verify account.", $"Please verify your email <a href='http://localhost:5250{url}'></a>");

                    TempData["message"] = "Please verify your email account.";
                    return RedirectToAction("Login", "Account");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        public async Task<IActionResult> ConfirmEmail(string Id, string token)
        {
            if(Id == null || token == null)
            {
                TempData["message"] = "Invalid token information.";
                return View();
            }

            var user = await _userManager.FindByIdAsync(Id);

            if(user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);

                if(result.Succeeded)
                {
                    TempData["message"] = "Confirm your account.";
                    return RedirectToAction("Login", "Account");
                }
            }
            
            TempData["message"] = "User not found.";
            return View();
        }
    
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
    
        public IActionResult ForgotPassword()
        {
            return View();
        }

        public async Task<IActionResult> ForgotPassword(string Email)
        {
            if(string.IsNullOrEmpty(Email))
            {
                return View();
            }

            var user = await _userManager.FindByEmailAsync(Email);
            
            if(user == null)
            {
                return View();
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var url = Url.Action("ResetPassword", "Account", new {user.Id, token});

            await _emailSender.SendEmailAsync(Email, "Reset Password", $"<a href='http://localhost:5250{url}'>Click</a> the link to reset your password.");

            TempData["message"] = "You can reset your password using the link sent to your email address";

            return View();
            
        }
    
        public IActionResult ResetPassword(string Id, string token)
        {
            if(Id == null || token == null)
            {
                return RedirectToAction("Login");
            }

            var model = new ResetPasswordModel
            {
              Token = token  
            };
            return View(model);
        }
    
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if(ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if(user == null)
                {
                    return RedirectToAction("Login");
                }
                var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);

                if(result.Succeeded)
                {
                    return RedirectToAction("Login");
                }
            }
            return View(model);
        }
    }
}
