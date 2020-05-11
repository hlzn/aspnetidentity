using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using mvc.Models;

namespace mvc.Controllers
{
    public class HomeController : Controller
    {
        private UserManager<MvcUser> userManager;
        private IUserClaimsPrincipalFactory<MvcUser> claimsPrincipalFactory;
        private readonly SignInManager<MvcUser> signInManager;

        public HomeController(UserManager<MvcUser> userManager, 
                              IUserClaimsPrincipalFactory<MvcUser> claimsPrincipalFactory,
                              SignInManager<MvcUser> signInManager)
        {
            this.userManager = userManager;
            this.claimsPrincipalFactory = claimsPrincipalFactory;
            this.signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(model.UserName);

                if (user == null)
                {
                    user = new MvcUser
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserName = model.UserName
                    };
                    var result = await userManager.CreateAsync(user, model.Password);
                }
                return View("Success");
            }
            return View();
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                // var user = await userManager.FindByNameAsync(model.UserName);

                // if (user != null && await userManager.CheckPasswordAsync(user, model.Password))
                // {
                //     // var identity = new ClaimsIdentity("cookie");
                //     // identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));
                //     // identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));

                //     // await HttpContext.SignInAsync("Identity.Application", new ClaimsPrincipal(identity));

                //     var principal = await claimsPrincipalFactory.CreateAsync(user);
                //     await HttpContext.SignInAsync("Identity.Application", principal);
                //     return RedirectToAction("Index");
                // }
                // ModelState.AddModelError("", "Invalid username or password");

                var signinResult = await signInManager.PasswordSignInAsync(model.UserName, model.Password, false, false);
                if (signinResult.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("", "Invalid username or password");
            }
            return View();
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var token = await userManager.GeneratePasswordResetTokenAsync(user);
                    var resetUrl = Url.Action("ResetPassword", "Home", new { token = token, email = model.Email }, Request.Scheme);
                    System.IO.File.WriteAllText("resetLink.txt", resetUrl);
                } else {

                }

                return View("Success");
            }
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            return View(new ResetPasswordModel() { Token = token, Email = email});
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    var result = userManager.ResetPasswordAsync(user, model.Token, model.Password);
                    if (!result.IsCompletedSuccessfully)
                    {
                        ModelState.AddModelError("", result.Exception.Message);
                        return View();
                    }
                    return View("Success");
                }
                ModelState.AddModelError("", "Invalid Request");
            }
            return View();
        }
    }
}
