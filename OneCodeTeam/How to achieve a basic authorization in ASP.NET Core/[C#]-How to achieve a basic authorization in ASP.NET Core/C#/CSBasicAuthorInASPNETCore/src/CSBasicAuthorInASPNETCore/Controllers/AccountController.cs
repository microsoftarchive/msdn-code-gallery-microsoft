using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CSBasicAuthorInASPNETCore.Model;
using System.Security.Claims;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace CSBasicAuthorInASPNETCore.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Login(User userFromFore)
        {
            var userFromStorage = TestUserStorage.UserList
                .FirstOrDefault(m => m.UserName == userFromFore.UserName && m.Password == userFromFore.Password);

            if (userFromStorage != null)
            {
                //you can add all of ClaimTypes in this collection
                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name,userFromStorage.UserName),
                    new Claim(ClaimTypes.Role,"Admin")
                    //,new Claim(ClaimTypes.Email,"emailaccount@microsoft.com") 
                };

                //init the identity instances
                var userPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "SuperSecureLogin"));

                //signin
                await HttpContext.Authentication.SignInAsync("Cookie", userPrincipal, new AuthenticationProperties
                {
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(20),
                    IsPersistent = false,
                    AllowRefresh = false
                });

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.ErrMsg = "UserName or Password is invalid";

                return View();
            }
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.Authentication.SignOutAsync("Cookie");

            return RedirectToAction("Index", "Home");
        }
    }

    //for simple, I'm not using the database to store the user data, just using a static class to replace it.
    public static class TestUserStorage
    {
        public static List<User> UserList { get; set; } = new List<User>() {
            new User { UserName = "User1",Password = "112233"}
        };
    }
}
