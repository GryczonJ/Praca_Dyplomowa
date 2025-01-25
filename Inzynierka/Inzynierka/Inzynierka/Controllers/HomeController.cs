using Inzynierka.Data;
using Inzynierka.Data.Tables;
using Inzynierka.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using System.Diagnostics;
using System.Security.Claims;

namespace Inzynierka.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly AhoyDbContext _context;

        public HomeController(AhoyDbContext context)
        {
            _context = context;
        }
        
        private Guid? GetLoggedInUserId()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(userIdString, out Guid userId) ? userId : null;
        }
        public IActionResult Index()
        {
            var userId = GetLoggedInUserId();

            // Pobierz u¿ytkownika z bazy danych
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);

            if (user != null &&
                user.firstName == "Jan" &&
                user.surName == "Kowalski" &&
                user.aboutMe == "Nazywam sie Jan Kowalski" &&
                user.age == null)
            {
                // Jeœli któryœ z warunków jest spe³niony, u¿ytkownik nie uzupe³ni³ danych
                return RedirectToAction("Edit", "Users");
            }
            
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
/*
        public IActionResult Register()
        {
            // Mo¿esz przekazaæ dodatkowe dane, np. listê w³aœcicieli
            var owners = _dbContext.Users.Select(u => new { u.Id, u.Name }).ToList();
            ViewBag.Owners = new SelectList(owners, "Id", "Name");

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(YachtRegistrationViewModel model)
        {
            if (ModelState.IsValid)
            {
                var yacht = new Yachts
                {
                    Name = model.Name,
                    Description = model.Description,
                    Type = model.Type,
                    Manufacturer = model.Manufacturer,
                    Model = model.Model,
                    Year = model.Year,
                    Length = model.Length,
                    Width = model.Width,
                    Crew = model.Crew,
                    Cabins = model.Cabins,
                    Beds = model.Beds,
                    Toilets = model.Toilets,
                    Showers = model.Showers,
                    Location = model.Location,
                    Capacity = model.Capacity,
                    OwnerId = model.OwnerId // Powi¹zanie z w³aœcicielem
                };

                _dbContext.Yachts.Add(yacht);
                _dbContext.SaveChanges();

                return RedirectToAction("Index"); // Przekierowanie na listê jachtów
            }

            return View(model); // Powrót do widoku w przypadku b³êdów
        }*/
        /* public IActionResult showYachts()
         {

             return View();
         }*/
    }
}
