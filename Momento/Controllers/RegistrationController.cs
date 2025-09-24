using Momento.Data;
using Momento.Models;
using Microsoft.AspNetCore.Mvc;

namespace Momento.Controllers
{
    public class RegistrationController : Controller
    {
        private readonly AppDbContext _context;

        public RegistrationController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("/")] 
        public IActionResult Index()
        {
            return View(new Registration());
        }

       [HttpPost("/Register")]
        public IActionResult Register(Registration data)
        {
            if (!ModelState.IsValid)
                return View("Index", data);

            var existingUser = _context.Users.FirstOrDefault(u => u.Email == data.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "Email уже используется");
                return View("Index", data);
            }

            _context.Users.Add(data);
            _context.SaveChanges();
            HttpContext.Session.SetInt32("UserId", data.Id);
    
            return RedirectToAction("Index", "Main");
        }
    }
}
