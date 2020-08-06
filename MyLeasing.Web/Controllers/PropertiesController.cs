using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyLeasing.Web.Data;
using MyLeasing.Web.Data.Entities;
using System.Threading.Tasks;

namespace MyLeasing.Web.Controllers
{
    public class PropertiesController : Controller
    {
        private readonly DataContext _context;

        public PropertiesController(DataContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View(_context.Properties
                .Include(p     => p.PropertyType)
                .Include(p     => p.PropertyImages)
                .Include(p     => p.Contracts)
                .Include(p     => p.Owner)
                .ThenInclude(o => o.User));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Property property = await _context.Properties
                .Include(o             => o.Owner)
                .ThenInclude(o         => o.User)
                .Include(o             => o.Contracts)
                .ThenInclude(c         => c.Lessee)
                .ThenInclude(l         => l.User)
                .Include(o             => o.PropertyType)
                .Include(p             => p.PropertyImages)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (property == null)
            {
                return NotFound();
            }

            return View(property);
        }
    }
}
