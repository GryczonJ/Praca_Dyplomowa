using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Inzynierka.Data;
using Inzynierka.Data.Tables;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Inzynierka.Controllers
{
    [Authorize(Roles = "Banned,User,Moderacja,Kapitan")]
    public class FavoriteCruisesController : Controller
    {
        private readonly AhoyDbContext _context;

        public FavoriteCruisesController(AhoyDbContext context)
        {
            _context = context;
        }
        private Guid? GetLoggedInUserId()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(userIdString, out Guid userId) ? userId : null;
        }

        // GET: FavoriteCruises
        public async Task<IActionResult> Index()
        {
            Guid? loggedInUserId = GetLoggedInUserId();
            // Pobranie ulubionych rejsów zalogowanego użytkownika
            var favoriteCruises = await _context.FavoriteCruises
                .Include(f => f.Cruise) // Załaduj dane powiązane z rejsami
                .Where(f => f.UserId == loggedInUserId) // Filtrowanie po użytkowniku
                .ToListAsync();

            return View(favoriteCruises);
        }

        // GET: FavoriteCruises/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var favoriteCruises = await _context.FavoriteCruises
                .Include(f => f.Cruise)
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (favoriteCruises == null)
            {
                return NotFound();
            }

            return View(favoriteCruises);
        }

        // GET: FavoriteCruises/Create
        public IActionResult Create()
        {
            ViewData["CruiseId"] = new SelectList(_context.Cruises, "Id", "currency");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: FavoriteCruises/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,CruiseId")] FavoriteCruises favoriteCruises)
        {
           // var loggedInUserId = GetLoggedInUserId();
            if (ModelState.IsValid)
            {
                favoriteCruises.UserId = Guid.NewGuid();
                _context.Add(favoriteCruises);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CruiseId"] = new SelectList(_context.Cruises, "Id", "currency", favoriteCruises.CruiseId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", favoriteCruises.UserId);
            return View(favoriteCruises);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToFavorites(int Id)
        {
            var loggedInUserId = GetLoggedInUserId();

            if (loggedInUserId == null)
            {
                TempData["Message"] = "Musisz być zalogowany, aby dodać jacht do ulubionych!";
                return RedirectToAction("Details", "Cruises", new { id = Id });
            }

            var existingFavorite = await _context.FavoriteCruises
                .FirstOrDefaultAsync(f => f.CruiseId == Id && f.UserId == loggedInUserId);

            if (existingFavorite != null)
            {
                TempData["Message"] = "Ten jacht już znajduje się w Twoich ulubionych!";
                return RedirectToAction("Details", "Cruises", new { id = Id });
            }

            var favorite = new FavoriteCruises
            {
                UserId = (Guid)loggedInUserId,
                CruiseId = Id
            };
            
            _context.FavoriteCruises.Add(favorite);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Jacht został dodany do ulubionych!";
            return RedirectToAction("Details", "Cruises", new { id = Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromFavorites(int Id)
        {
            var loggedInUserId = GetLoggedInUserId();

            var favorite = await _context.FavoriteCruises
                .FirstOrDefaultAsync(f => f.CruiseId == Id && f.UserId == loggedInUserId);

            if (favorite != null)
            {
                _context.FavoriteCruises.Remove(favorite);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Jacht został usunięty z ulubionych.";
            }
            else
            {
                TempData["Message"] = "Nie znaleziono jachtu w ulubionych.";
            }

            return RedirectToAction("Details", "Cruises", new { id = Id });
        }


        // GET: FavoriteCruises/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var favoriteCruises = await _context.FavoriteCruises.FindAsync(id);
            if (favoriteCruises == null)
            {
                return NotFound();
            }
            ViewData["CruiseId"] = new SelectList(_context.Cruises, "Id", "currency", favoriteCruises.CruiseId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", favoriteCruises.UserId);
            return View(favoriteCruises);
        }

        // POST: FavoriteCruises/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("UserId,CruiseId")] FavoriteCruises favoriteCruises)
        {
            if (id != favoriteCruises.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(favoriteCruises);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FavoriteCruisesExists(favoriteCruises.UserId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CruiseId"] = new SelectList(_context.Cruises, "Id", "currency", favoriteCruises.CruiseId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", favoriteCruises.UserId);
            return View(favoriteCruises);
        }

        // GET: FavoriteCruises/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var favoriteCruises = await _context.FavoriteCruises
                .Include(f => f.Cruise)
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.CruiseId == id);
            if (favoriteCruises == null)
            {
                return NotFound();
            }

            return View(favoriteCruises);
        }

        // POST: FavoriteCruises/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var favoriteCruises = await _context.FavoriteCruises.FindAsync(id);
            if (favoriteCruises != null)
            {
                _context.FavoriteCruises.Remove(favoriteCruises);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FavoriteCruisesExists(Guid id)
        {
            return _context.FavoriteCruises.Any(e => e.UserId == id);
        }
    }
}
