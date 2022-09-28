using Acceptance.Domain;
using Acceptance.Service.Services;
using Acceptance.ViewModels.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Acceptance.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService userService;

        public AccountController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        [Authorize(Roles = "owner")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userService.GetById(Guid.Parse(model.Key));

                if(user is null)
                {
                    ModelState.AddModelError(string.Empty, "Kalit xato");
                    return View();
                }

                if(!user.Enabled)
                {
                    ModelState.AddModelError(string.Empty, "Ushbu kalit vaqtinchalik faol emas");
                    return View();
                }

                var claims = new List<Claim>{
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.NameIdentifier, model.Key),
                    new Claim(ClaimTypes.Role, user.Role),
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties(){
                    IsPersistent = false
                });

                return RedirectToAction("index", "home");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userService.GetById(Guid.Parse(model.Key));

                if(user is not null)
                {
                    ModelState.AddModelError(string.Empty, "Ushbu kalit band");
                    return View();
                }

                User newUser = new User
                {
                    Id = Guid.Parse(model.Key),
                    Username = model.Username,
                    Role = model.Role,
                    Enabled = true
                };

                await userService.Create(newUser);

                await userService.CompleteAsync();

                return RedirectToAction("index", "home");
            }

            return View();
        }

        [Authorize(Roles = "owner")]
        [HttpGet]
        public async Task<IActionResult> Users()
        {
            UsersViewModel model = new UsersViewModel
            {
                Users = await userService.GetAll()
            };

            return View(model);
        }

        [Authorize(Roles = "owner")]
        [HttpGet]
        public async Task<IActionResult> Edit(Guid Id)
        {
            User user = await userService.GetById(Id);

            EditViewModel model = new EditViewModel
            {
                Id = user.Id,
                Username = user.Username
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await userService.GetById(model.Id);

                user.Enabled = bool.Parse(model.Enabled);
                user.Username = model.Username;
                user.Role = model.Role;

                userService.Update(user);
                await userService.CompleteAsync();

                return View(model);
            }

            return RedirectToAction("users", "account");
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return View("login");
        }
    }
}
