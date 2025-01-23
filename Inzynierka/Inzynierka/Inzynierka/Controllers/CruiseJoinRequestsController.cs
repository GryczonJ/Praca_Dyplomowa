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
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Authorization;

namespace Inzynierka.Controllers
{
    [Authorize(Roles = "User,Moderacja,Kapitan")]
    public class CruiseJoinRequestsController : Controller
    {
        private readonly AhoyDbContext _context;

        public CruiseJoinRequestsController(AhoyDbContext context)
        {
            _context = context;
        }

        private Guid? GetLoggedInUserId()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(userIdString, out Guid userId) ? userId : null;
        }
        // GET: CruiseJoinRequests
        /*  public async Task<IActionResult> Index()
          {
              var ahoyDbContext = _context.CruiseJoinRequest
                  .Include(c => c.Capitan)
                  .Include(c => c.Cruise)
                  .Include(c => c.User);
              return View(await ahoyDbContext.ToListAsync());
          }*/

        // GET: CruiseJoinRequest
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
            var ahoyDbContext = _context.CruiseJoinRequest
                .Include(c => c.Cruise)
                .Include(c => c.Capitan)
                .Include(c => c.User)
                .Where(c => c.Capitan.Id == loggedInUserId); // Filtr dla użytkownika

            return View(await ahoyDbContext.ToListAsync());
        }

        // GET: CruiseJoinRequests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cruiseJoinRequest = await _context.CruiseJoinRequest
                .Include(c => c.Capitan)
                .Include(c => c.Cruise)
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cruiseJoinRequest == null)
            {
                return NotFound();
            }

            return View(cruiseJoinRequest);
        }

        // GET: CruiseJoinRequests/Create
        public IActionResult Create()
        {
            ViewData["CapitanId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["CruiseId"] = new SelectList(_context.Cruises, "Id", "currency");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: CruiseJoinRequests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,status,date,CruiseId,UserId,CapitanId")] CruiseJoinRequest cruiseJoinRequest)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cruiseJoinRequest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CapitanId"] = new SelectList(_context.Users, "Id", "Id", cruiseJoinRequest.CapitanId);
            ViewData["CruiseId"] = new SelectList(_context.Cruises, "Id", "currency", cruiseJoinRequest.CruiseId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", cruiseJoinRequest.UserId);
            return View(cruiseJoinRequest);
        }

        // GET: CruiseJoinRequests/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cruiseJoinRequest = await _context.CruiseJoinRequest.FindAsync(id);
            if (cruiseJoinRequest == null)
            {
                return NotFound();
            }
            ViewData["CapitanId"] = new SelectList(_context.Users, "Id", "Id", cruiseJoinRequest.CapitanId);
            ViewData["CruiseId"] = new SelectList(_context.Cruises, "Id", "currency", cruiseJoinRequest.CruiseId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", cruiseJoinRequest.UserId);
            return View(cruiseJoinRequest);
        }

        // POST: CruiseJoinRequests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,status,date,CruiseId,UserId,CapitanId")] CruiseJoinRequest cruiseJoinRequest)
        {
            if (id != cruiseJoinRequest.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cruiseJoinRequest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CruiseJoinRequestExists(cruiseJoinRequest.Id))
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
            ViewData["CapitanId"] = new SelectList(_context.Users, "Id", "Id", cruiseJoinRequest.CapitanId);
            ViewData["CruiseId"] = new SelectList(_context.Cruises, "Id", "currency", cruiseJoinRequest.CruiseId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", cruiseJoinRequest.UserId);
            return View(cruiseJoinRequest);
        }

        // GET: CruiseJoinRequests/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cruiseJoinRequest = await _context.CruiseJoinRequest
                .Include(c => c.Capitan)
                .Include(c => c.Cruise)
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cruiseJoinRequest == null)
            {
                return NotFound();
            }

            return View(cruiseJoinRequest);
        }

        // POST: CruiseJoinRequests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cruiseJoinRequest = await _context.CruiseJoinRequest.FindAsync(id);
            if (cruiseJoinRequest != null)
            {
                _context.CruiseJoinRequest.Remove(cruiseJoinRequest);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AcceptUserToCruise(Guid userId, int cruiseId)
        {
            try
            {
                var joinRequest = await _context.CruiseJoinRequest
                .FirstOrDefaultAsync(r => r.UserId == userId && r.CruiseId == cruiseId);
                // Sprawdzanie, czy rejs istnieje
                var cruise = await _context.Cruises.Include(c => c.CruisesParticipants).FirstOrDefaultAsync(c => c.Id == cruiseId);
                if (cruise == null)
                {
                    return NotFound("Rejs nie został znaleziony.");
                }

                // Sprawdzanie, czy użytkownik istnieje
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return NotFound("Użytkownik nie został znaleziony.");
                }

                // Sprawdzanie, czy użytkownik jest już uczestnikiem rejsu
                var isAlreadyParticipant = await _context.CruisesParticipants.AnyAsync(cp => cp.UsersId == userId && cp.CruisesId == cruiseId);
                if (isAlreadyParticipant)
                {
                    return BadRequest("Użytkownik jest już uczestnikiem rejsu.");
                }

                // Sprawdzanie limitu uczestników
                var currentParticipantsCount = cruise.CruisesParticipants.Count;
                if (currentParticipantsCount >= cruise.maxParticipants)
                {
                    return BadRequest("Liczba uczestników rejsu osiągnęła maksymalny limit.");
                }

                // Dodanie użytkownika do uczestników rejsu
                var newParticipant = new CruisesParticipants
                {
                    UsersId = userId,
                    CruisesId = cruiseId
                };
            
                    _context.CruisesParticipants.Add(newParticipant);
                    // Usunięcie zgłoszenia
                    _context.CruiseJoinRequest.Remove(joinRequest);

                    await _context.SaveChangesAsync();

                // Aktualizacja statusu rejsu, jeśli osiągnięto maksymalną liczbę uczestników
                if (cruise.CruisesParticipants.Count == cruise.maxParticipants)
                {
                    cruise.status = CruiseStatus.Ongoing;
                    _context.Cruises.Update(cruise);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                // Możesz zalogować wyjątek lub rzucić dalej
                throw new InvalidOperationException($"Wystąpił problem przy dodawaniu użytkownika do rejsu: {ex.Message}", ex);
            }

            return RedirectToAction(nameof(Index));
        }


        private bool CruiseJoinRequestExists(int id)
        {
            return _context.CruiseJoinRequest.Any(e => e.Id == id);
        }
    }
}
