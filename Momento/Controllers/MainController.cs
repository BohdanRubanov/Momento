using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Momento.Data;
using Momento.Models;

namespace Momento.Controllers
{
    public class MainController : Controller
    {
        private readonly AppDbContext _context;

        public MainController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var publications = await _context.Publications
                .Include(p => p.User)
                .ToListAsync(); 
            return View(publications);
        }
    }
}