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
    public class CruisesParticipantsController : Controller
    {
        private readonly AhoyDbContext _context;

        public CruisesParticipantsController(AhoyDbContext context)
        {
            _context = context;
        }
        private Guid? GetLoggedInUserId()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(userIdString, out Guid userId) ? userId : null;
        }

        // GET: CruisesParticipants
        /* public async Task<IActionResult> Index()
         {
             var ahoyDbContext = _context.CruisesParticipants
                 .Include(c => c.Cruises)
                 .Include(c => c.Users);
             return View(await ahoyDbContext.ToListAsync());
         }
 */
        public async Task<IActionResult> Index()
        {
            // Pobranie ID zalogowanego użytkownika
            var loggedInUserId = GetLoggedInUserId();

            // Sprawdzenie, czy użytkownik jest zalogowany
            if (loggedInUserId == null)
            {
                return Unauthorized(); // Jeśli nie, zwróć błąd 401 (Unauthorized)
            }

            // Filtrowanie danych na podstawie ID użytkownika
            var ahoyDbContext = _context.CruisesParticipants
                .Include(c => c.Cruises)
                .Include(c => c.Users)
                .Where(c => c.Cruises.Capitan.Id == loggedInUserId); // Filtr dla użytkownika

            return View(await ahoyDbContext.ToListAsync());
        }

        // GET: CruisesParticipants/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cruisesParticipants = await _context.CruisesParticipants
                .Include(c => c.Cruises)
                .Include(c => c.Users)
                .FirstOrDefaultAsync(m => m.UsersId == id);
            if (cruisesParticipants == null)
            {
                return NotFound();
            }

            return View(cruisesParticipants);
        }

        // GET: CruisesParticipants/Create
        public IActionResult Create()
        {
            ViewData["CruisesId"] = new SelectList(_context.Cruises, "Id", "currency");
            ViewData["UsersId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: CruisesParticipants/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UsersId,CruisesId")] CruisesParticipants cruisesParticipants)
        {
            if (ModelState.IsValid)
            {
                cruisesParticipants.UsersId = Guid.NewGuid();
                _context.Add(cruisesParticipants);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CruisesId"] = new SelectList(_context.Cruises, "Id", "currency", cruisesParticipants.CruisesId);
            ViewData["UsersId"] = new SelectList(_context.Users, "Id", "Id", cruisesParticipants.UsersId);
            return View(cruisesParticipants);
        }

        // GET: CruisesParticipants/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cruisesParticipants = await _context.CruisesParticipants.FindAsync(id);
            if (cruisesParticipants == null)
            {
                return NotFound();
            }
            ViewData["CruisesId"] = new SelectList(_context.Cruises, "Id", "currency", cruisesParticipants.CruisesId);
            ViewData["UsersId"] = new SelectList(_context.Users, "Id", "Id", cruisesParticipants.UsersId);
            return View(cruisesParticipants);
        }

        // POST: CruisesParticipants/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("UsersId,CruisesId")] CruisesParticipants cruisesParticipants)
        {
            if (id != cruisesParticipants.UsersId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cruisesParticipants);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CruisesParticipantsExists(cruisesParticipants.UsersId))
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
            ViewData["CruisesId"] = new SelectList(_context.Cruises, "Id", "currency", cruisesParticipants.CruisesId);
            ViewData["UsersId"] = new SelectList(_context.Users, "Id", "Id", cruisesParticipants.UsersId);
            return View(cruisesParticipants);
        }

        // GET: CruisesParticipants/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cruisesParticipants = await _context.CruisesParticipants
                .Include(c => c.Cruises)
                .Include(c => c.Users)
                .FirstOrDefaultAsync(m => m.UsersId == id);
            if (cruisesParticipants == null)
            {
                return NotFound();
            }

            return View(cruisesParticipants);
        }

        // POST: CruisesParticipants/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var cruisesParticipants = await _context.CruisesParticipants.FindAsync(id);
            if (cruisesParticipants != null)
            {
                _context.CruisesParticipants.Remove(cruisesParticipants);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveCrewmate(int cruiseId, Guid userId)
        {
            /*var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdString == null || !Guid.TryParse(userIdString, out Guid userId))
            {
                return RedirectToAction("Login", "Account");
            }*/

            var participation = await _context.CruisesParticipants
                .FirstOrDefaultAsync(cp => cp.UsersId == userId && cp.CruisesId == cruiseId);
            if (participation == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (participation != null)
            {
                _context.CruisesParticipants.Remove(participation);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Zrezygnowałeś z rejsu.";
            }
            else
            {
                TempData["Error"] = "Nie jesteś uczestnikiem tego rejsu.";
            }

            return RedirectToAction(nameof(Index));
            //return RedirectToAction(nameof(Details), new { id = cruiseId });
        }

        private bool CruisesParticipantsExists(Guid id)
        {
            return _context.CruisesParticipants.Any(e => e.UsersId == id);
        }
    }
}
