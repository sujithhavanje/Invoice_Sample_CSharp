using Invoice.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Invoice.Services;

namespace Invoice.Controllers
{
    public class AccountController : Controller
    {
        private readonly CsvHelperService _csvHelperService;
        private readonly GenUtility _GenUtility;

        public AccountController(CsvHelperService csvHelperService, GenUtility GenUtility)
        {
            _csvHelperService = csvHelperService;
            _GenUtility = GenUtility;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginView model)
        {
            try
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

           
            }
            catch (Exception ex)
            {
                _GenUtility.LogError(ex);
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
           
            try
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
               
            }
          
            catch (Exception ex)
            {
                _GenUtility.LogError(ex);
            }
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
           
            try
            {
                if (ModelState.IsValid)
                {
                    _csvHelperService.AddUser(new User
                    {
                        Name =model.Name,
                        Username = model.Username,
                        Password = model.Password,
                        Email = model.Email,
                        Address = model.Address,
                        ContactNumber = model.ContactNumber
                    });
                    return RedirectToAction("Login", "Account");
                }

            }
            catch (Exception ex)
            {
                _GenUtility.LogError(ex);
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Edit()
        {
           
            try
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
                    Name = user.Name,
                    Email = user.Email,
                    Address = user.Address,
                    ContactNumber = user.ContactNumber
                };
                return View(model);
            }
            catch (Exception ex)
            {
                _GenUtility.LogError(ex);
                return View(new EditProfileView());
            }
          
        }

        [HttpPost]
        public IActionResult Edit(EditProfileView model)
        {
           
            try
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

            }
            catch (Exception ex)
            {
                _GenUtility.LogError(ex);
            }

            return View(model);
        }
    }
}
