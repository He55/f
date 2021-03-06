using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Hummer.Fly.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using AspNet.Security.OAuth.GitHub;

namespace Hummer.Fly.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult CookieLogin()
        {
            return Challenge(new AuthenticationProperties() { RedirectUri = "/" }, CookieAuthenticationDefaults.AuthenticationScheme);
        }

        public IActionResult GoogleLogin()
        {
            return Challenge(new AuthenticationProperties() { RedirectUri = "/" }, GoogleDefaults.AuthenticationScheme);
        }

        public IActionResult GithubLogin()
        {
            return Challenge(new AuthenticationProperties() { RedirectUri = "/" }, GitHubAuthenticationDefaults.AuthenticationScheme);
        }

        public IActionResult MicrosoftAccountLogin()
        {
            return Challenge(new AuthenticationProperties() { RedirectUri = "/" }, MicrosoftAccountDefaults.AuthenticationScheme);
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromForm]LoginViewModel model, string returnUrl)
        {
            if (!(model.Email == "1" && model.Password == "1"))
            {
                return RedirectToAction();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, model.Email),
                new Claim(ClaimTypes.Role, "Administrator"),
                new Claim("LastChanged", DateTime.Now.Minute.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authenticationProperties = new AuthenticationProperties
            {
                // 持久化cookie
                IsPersistent = (model.Remember != null),
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(15)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authenticationProperties);

            if (string.IsNullOrEmpty(returnUrl)
                || !Url.IsLocalUrl(returnUrl))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return LocalRedirect(returnUrl);
            }
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult Forget()
        {
            return View();
        }
    }
}
