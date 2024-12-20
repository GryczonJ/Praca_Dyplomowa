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

namespace Inzynierka.Controllers
{
    public class FavoriteYachtsForSalesController : Controller
    {
        private readonly AhoyDbContext _context;

        public FavoriteYachtsForSalesController(AhoyDbContext context)
        {
            _context = context;
        }
        private Guid? GetLoggedInUserId()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(userIdString, out Guid userId) ? userId : null;
        }

        // GET: FavoriteYachtsForSales
        public async Task<IActionResult> Index()
        {
            var ahoyDbContext = _context.FavoriteYachtsForSale.Include(f => f.User).Include(f => f.YachtForSale);
            return View(await ahoyDbContext.ToListAsync());
        }

        // GET: FavoriteYachtsForSales/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var favoriteYachtsForSale = await _context.FavoriteYachtsForSale
                .Include(f => f.User)
                .Include(f => f.YachtForSale)
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (favoriteYachtsForSale == null)
            {
                return NotFound();
            }

            return View(favoriteYachtsForSale);
        }

        // GET: FavoriteYachtsForSales/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["YachtSaleId"] = new SelectList(_context.YachtSale, "Id", "currency");
            return View();
        }

        // POST: FavoriteYachtsForSales/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,YachtSaleId")] FavoriteYachtsForSale favoriteYachtsForSale)
        {
            //var loggedInUserId = GetLoggedInUserId();
            if (ModelState.IsValid)
            {
                favoriteYachtsForSale.UserId = Guid.NewGuid();
                _context.Add(favoriteYachtsForSale);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", favoriteYachtsForSale.UserId);
            ViewData["YachtSaleId"] = new SelectList(_context.YachtSale, "Id", "currency", favoriteYachtsForSale.YachtSaleId);
            return View(favoriteYachtsForSale);
        }

        /*[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToFavorites(int YachtSaleId)
        {
            // Pobierz ID zalogowanego użytkownika
            Guid? loggedInUserId = GetLoggedInUserId();
           if(loggedInUserId == null)
            {
                TempData["Message"] = "Musisz być zalogowany, aby dodać jacht do ulubionych!";
                return RedirectToAction("Details", "YachtSales", new { id = YachtSaleId });
            }

            // Sprawdź, czy jacht nie został już dodany do ulubionych
            var existingFavorite = await _context.FavoriteYachtsForSale
                .FirstOrDefaultAsync(f => f.YachtForSale.Id == YachtSaleId && f.UserId == loggedInUserId);

            if (existingFavorite != null)
            {
                // Jeśli już istnieje, możesz zwrócić komunikat lub odświeżyć stronę
                TempData["Message"] = "Ten jacht już znajduje się w Twoich ulubionych!";
                return RedirectToAction("Details", "YachtSales", new { id = YachtSaleId });
            }

            // Dodaj jacht do ulubionych
            var favorite = new FavoriteYachtsForSale
            {
                UserId = (Guid)loggedInUserId,
                YachtSaleId = YachtSaleId
            };

            _context.FavoriteYachtsForSale.Add(favorite);
            await _context.SaveChangesAsync();

            // Powrót do strony szczegółów jachtu z komunikatem
            TempData["Message"] = "Jacht został dodany do ulubionych!";
            return RedirectToAction("Details", "YachtSales", new { id = YachtSaleId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromFavorites(int YachtSaleId)
        {
            var loggedInUserId = GetLoggedInUserId();

            // Znajdź ulubiony jacht
            var favorite = await _context.FavoriteYachtsForSale
                .FirstOrDefaultAsync(f => f.YachtForSale.Id == YachtSaleId && f.UserId == loggedInUserId);

            if (favorite != null)
            {
                _context.FavoriteYachtsForSale.Remove(favorite);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Jacht został usunięty z ulubionych.";
            }
            else
            {
                TempData["Message"] = "Nie znaleziono jachtu w ulubionych.";
            }

            return RedirectToAction("Details", "YachtSales", new { id = YachtSaleId });
        }*/

        // GET: FavoriteYachtsForSales/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var favoriteYachtsForSale = await _context.FavoriteYachtsForSale.FindAsync(id);
            if (favoriteYachtsForSale == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", favoriteYachtsForSale.UserId);
            ViewData["YachtSaleId"] = new SelectList(_context.YachtSale, "Id", "currency", favoriteYachtsForSale.YachtSaleId);
            return View(favoriteYachtsForSale);
        }

        // POST: FavoriteYachtsForSales/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("UserId,YachtSaleId")] FavoriteYachtsForSale favoriteYachtsForSale)
        {
            if (id != favoriteYachtsForSale.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(favoriteYachtsForSale);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FavoriteYachtsForSaleExists(favoriteYachtsForSale.UserId))
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
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", favoriteYachtsForSale.UserId);
            ViewData["YachtSaleId"] = new SelectList(_context.YachtSale, "Id", "currency", favoriteYachtsForSale.YachtSaleId);
            return View(favoriteYachtsForSale);
        }

        // GET: FavoriteYachtsForSales/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var favoriteYachtsForSale = await _context.FavoriteYachtsForSale
                .Include(f => f.User)
                .Include(f => f.YachtForSale)
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (favoriteYachtsForSale == null)
            {
                return NotFound();
            }

            return View(favoriteYachtsForSale);
        }

        // POST: FavoriteYachtsForSales/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var favoriteYachtsForSale = await _context.FavoriteYachtsForSale.FindAsync(id);
            if (favoriteYachtsForSale != null)
            {
                _context.FavoriteYachtsForSale.Remove(favoriteYachtsForSale);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FavoriteYachtsForSaleExists(Guid id)
        {
            return _context.FavoriteYachtsForSale.Any(e => e.UserId == id);
        }
    }
}
