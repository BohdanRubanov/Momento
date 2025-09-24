using Microsoft.AspNetCore.Mvc;
using Momento.Data;
using Momento.Models;

namespace Momento.Controllers
{
    public class ProfileController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProfileController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpGet("/Profile")]
        public IActionResult Index()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Index", "Registration");

            var user = _context.Users.Find(userId);
            if (user == null)
                return RedirectToAction("Index", "Registration");

            return View(user);
        }

        [HttpPost("/Profile")]
        public IActionResult Update(Registration data, IFormFile? avatarFile)
        {

            ModelState.Remove("Password");
            
            if (!ModelState.IsValid)
                return View("Index", data);

            var user = _context.Users.Find(data.Id);
            if (user == null)
                return RedirectToAction("Index", "Registration");


            user.Nickname = data.Nickname;
            user.Email = data.Email;
            user.Status = data.Status;


            if (avatarFile != null && avatarFile.Length > 0)
            {
                string fileName = $"{Guid.NewGuid()}{Path.GetExtension(avatarFile.FileName)}";
                string path = Path.Combine(_env.WebRootPath, "uploads", fileName);

                Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                using var stream = new FileStream(path, FileMode.Create);
                avatarFile.CopyTo(stream);

                user.AvatarUrl = "/uploads/" + fileName;
            }

            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
