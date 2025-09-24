using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using Momento.Models;
using Momento.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http; 

namespace Momento.Controllers
{
    public class PublicationsController : Controller
    {
        private readonly IWebHostEnvironment _environment;
        private readonly AppDbContext _context;

        public PublicationsController(IWebHostEnvironment environment, AppDbContext context)
        {
            _environment = environment;
            _context = context;
        }

        public IActionResult Create()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Index", "Registration"); // Если не авторизован, редирект на регистрацию
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Publication publication, IFormFile photo)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Index", "Registration");
            }

            ModelState.Remove("User");

            if (photo == null || photo.Length == 0)
            {
                ModelState.AddModelError("", "Пожалуйста, выберите фото");
            }

            if (string.IsNullOrWhiteSpace(publication.Caption))
            {
                ModelState.AddModelError("Caption", "Добавьте описание");
            }

            if (!ModelState.IsValid)
            {
                return View(publication);
            }

            try
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + photo.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await photo.CopyToAsync(fileStream);
                }

                var newPublication = new Publication
                {
                    PhotoUrl = "/uploads/" + uniqueFileName,
                    Caption = publication.Caption,
                    UserId = userId.Value
                };

                _context.Publications.Add(newPublication);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Main");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Ошибка при сохранении публикации: {ex.Message}");
                return View(publication);
            }
        }


        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Index", "Registration");
            }

            var publications = await _context.Publications
                .Where(p => p.UserId == userId.Value)
                .Include(p => p.User) 
                .OrderByDescending(p => p.Id) 
                .ToListAsync();
                
            return View(publications);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Index", "Registration");
            }

            var publication = await _context.Publications
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId.Value);

            if (publication == null)
            {
                return NotFound();
            }

            try
            {
                if (!string.IsNullOrEmpty(publication.PhotoUrl))
                {
                    var filePath = Path.Combine(_environment.WebRootPath, publication.PhotoUrl.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                _context.Publications.Remove(publication);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Ошибка при удалении публикации: {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }
    }
}