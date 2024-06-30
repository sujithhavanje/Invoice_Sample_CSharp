using Invoice.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Invoice.Controllers
{
    public class AccountController : Controller
    {
        private readonly CsvHelperService _csvHelperService;

        public AccountController(CsvHelperService csvHelperService)
        {
            _csvHelperService = csvHelperService;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginView model)
        {
            if (ModelState.IsValid)
            {
                var user = _csvHelperService.GetUserByUsername(model.Username);

                if (user != null && user.Password == model.Password)
                {
                    var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username)
                };

                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity));

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterView model)
        {
            if (ModelState.IsValid)
            {
                _csvHelperService.AddUser(new User
                {
                    Username = model.Username,
                    Password = model.Password,
                    Email = model.Email,
                    Address = model.Address,
                    ContactNumber = model.ContactNumber
                });
                return RedirectToAction("Login", "Account");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Edit()
        {
            var username = User.Identity.Name;
            var user = _csvHelperService.GetUserByUsername(username);
            if (user == null)
            {
                return NotFound();
            }

            var model = new EditProfileView
            {
                Username = user.Username,
                Email = user.Email,
                Address = user.Address,
                ContactNumber = user.ContactNumber
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(EditProfileView model)
        {
            if (ModelState.IsValid)
            {
                var user = _csvHelperService.GetUserByUsername(model.Username);
                if (user != null)
                {
                    user.Email = model.Email;
                    user.Address = model.Address;
                    user.ContactNumber = model.ContactNumber;
                    _csvHelperService.UpdateUser(user);

                    return RedirectToAction("Index", "Home");
                }
                return NotFound();
            }

            return View(model);
        }



    }
}
