using Microsoft.AspNetCore.Mvc;
using Project.Models;
using BCrypt.Net;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;


namespace Project.Controllers
{
    public class AccountController : Controller
    {
        EcommerceContext db = new EcommerceContext();
        public IActionResult Register()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(Register rg)
        {
            ModelState.Remove("Role");
            if (ModelState.IsValid)
            {
                if(db.Registers.Any(u=>u.Email == rg.Email))
                {
                    ModelState.AddModelError("Email", "Email Already exists");
                    return View(rg);
                }
                var hashpassword = BCrypt.Net.BCrypt.HashPassword(rg.Password);

                var user = new Register
                {
                    Name = rg.Name,
                    Email = rg.Email,
                    Password = hashpassword,
                    Role = "User"

                };
                db.Registers.Add(user);
                db.SaveChanges();
                TempData["Success"] = "Register successfully Please Login";
                return RedirectToAction("Login");


            }

            return View(rg);
        }

        [HttpPost]
        public IActionResult Login(string email,string password)
        {
            var user = db.Registers.FirstOrDefault(u => u.Email == email);
            if(user != null && BCrypt.Net.BCrypt.Verify(password,user.Password))
            {
                var claim = new List<Claim> { 
                
                    new Claim(ClaimTypes.Name,user.Name),
                    new Claim(ClaimTypes.Role,user.Role),
                    new Claim("UserId",user.UserId.ToString())
                };

                var identity = new ClaimsIdentity(claim, "CookieAuth");
                HttpContext.SignInAsync("CookieAuth", new ClaimsPrincipal(identity));

                return (user.Role == "Admin") ? RedirectToAction("Index", "Admin") :
                    RedirectToAction("Index", "Home");

            }
            ViewBag.Error = "Invalid Credentials";
            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync("CookieAuth");
           
            return RedirectToAction("Login");
        }

    }
}

