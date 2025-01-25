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
    /*[Authorize(Roles = "User,Moderacja,Kapitan")]*/
    public class CruisesController : Controller
    {
        private readonly AhoyDbContext _context;

        public CruisesController(AhoyDbContext context)
        {
            _context = context;
        }

        // GET: Cruises
        /*public async Task<IActionResult> Index()
        {
            var ahoyDbContext = _context.Cruises.Include(c => c.Capitan).Include(c => c.Yacht);
            return View(await ahoyDbContext.ToListAsync());
        }*/
        public async Task<IActionResult> Index()
        {
            // Pobranie identyfikatora zalogowanego użytkownika
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid? userId = Guid.TryParse(userIdString, out Guid parsedUserId) ? parsedUserId : null;

            bool isLogged = false;

            List<Cruises> myCruises = new List<Cruises>();
            List<Cruises> otherCruises = new List<Cruises>();

            if (userId != null)
            {
                // Rejsy użytkownika (gdzie użytkownik jest kapitanem)
                myCruises = await _context.Cruises
                    .Include(c => c.Capitan)
                    .Include(c => c.Yacht)
                    .Where(c => c.CapitanId == userId)
                    .ToListAsync();

                // Rejsy pozostałe (nie prowadzone przez użytkownika)
                otherCruises = await _context.Cruises
                    .Include(c => c.Capitan)
                    .Include(c => c.Yacht)
                    .Where(c => c.CapitanId != userId)
                    .ToListAsync();
                isLogged = true;
            }
            else
            {
                // Jeśli użytkownik nie jest zalogowany, pokazujemy wszystkie rejsy
                otherCruises = await _context.Cruises
                    .Include(c => c.Capitan)
                    .Include(c => c.Yacht)
                    .ToListAsync();
            }
            ViewData["isLogged"] = isLogged;
            // Przekazanie obu list do widoku jako krotki
            //return View((myCruises, otherCruises));
            return View(((IEnumerable<Cruises>)myCruises, (IEnumerable<Cruises>)otherCruises));
        }

        
        [HttpPost]
        public async Task<IActionResult> JoinCruise(int cruiseId)
        {
            var userId = GetLoggedInUserId();
            if (userId == null) return RedirectToAction("Login", "Account");

            if (await IsUserAlreadyParticipantOrPendingAsync(userId.Value, cruiseId))
            {
                TempData["Error"] = "Jesteś już uczestnikiem lub złożyłeś zgłoszenie.";
                return RedirectToAction(nameof(Details), new { id = cruiseId });
            }

            var cruise = await GetCruiseWithCaptainAsync(cruiseId);
            if (cruise == null)
            {
                TempData["Error"] = "Rejs nie istnieje.";
                return RedirectToAction(nameof(Details), new { id = cruiseId });
            }

            await CreateJoinRequestAsync(userId.Value, cruise);

            TempData["Success"] = "Prośba o dołączenie została wysłana.";
            return RedirectToAction(nameof(Details), new { id = cruiseId });
        }

        private Guid? GetLoggedInUserId()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(userIdString, out Guid userId) ? userId : null;
        }

        private async Task<bool> IsUserAlreadyParticipantOrPendingAsync(Guid userId, int cruiseId)
        {
            return await _context.CruisesParticipants.AnyAsync(cp => cp.UsersId == userId && cp.CruisesId == cruiseId)
                || await _context.CruiseJoinRequest.AnyAsync(cjr => cjr.UserId == userId && cjr.CruiseId == cruiseId);
        }

        private async Task<Cruises> GetCruiseWithCaptainAsync(int cruiseId)
        {
            return await _context.Cruises
                .Include(c => c.Capitan)
                .FirstOrDefaultAsync(c => c.Id == cruiseId);
        }

        private async Task CreateJoinRequestAsync(Guid userId, Cruises cruise)
        {
            var joinRequest = new CruiseJoinRequest
            {
                CruiseId = cruise.Id,
                UserId = userId,
                CapitanId = cruise.Capitan.Id,
                status = (RequestStatus)(int)TransactionStatus.Pending,
                date = DateTime.UtcNow
            };
            _context.CruiseJoinRequest.Add(joinRequest);
            await _context.SaveChangesAsync();
        }

        [HttpPost]
        public async Task<IActionResult> LeaveCruise(int cruiseId)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdString == null || !Guid.TryParse(userIdString, out Guid userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var participation = await _context.CruisesParticipants
                .FirstOrDefaultAsync(cp => cp.UsersId == userId && cp.CruisesId == cruiseId);

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

            return RedirectToAction(nameof(Details), new { id = cruiseId });
        }

        // GET: Cruises/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid? userId = Guid.TryParse(userIdString, out Guid parsedUserId) ? parsedUserId : null;
            
            if (id == null)
            {
                return NotFound();
            }

            var cruises = await _context.Cruises
                .Include(c => c.Capitan)
                .Include(c => c.Yacht)
                .Include(c => c.CruisesParticipants)
                .Include(c => c.CruiseJoinRequests)
                .Include(c => c.Comments)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (cruises == null)
            {
                return NotFound();
            }

            // Sprawdzenie, czy użytkownik jest członkiem rejsu
            bool isMember = userId != null && await _context.CruisesParticipants
                .AnyAsync(cp => cp.UsersId == userId && cp.CruisesId == id);

            // Sprawdzenie, czy użytkownik złożył prośbę o dołączenie do rejsu
            bool isPending = userId != null && await _context.CruiseJoinRequest
                .AnyAsync(cjr => cjr.UserId == userId && cjr.CruiseId == id);

            // Sprawdzenie, czy użytkownik jest właścicielem (kapitanem) rejsu
            bool isOwner = userId != null && cruises.CapitanId == userId;
           
            bool isFavorite = userId != null && await _context.FavoriteCruises
            .AnyAsync(fc => fc.CruiseId == id && fc.UserId == userId);


            // Przekazanie danych do ViewData
            ViewData["IsMember"] = isMember;
            ViewData["IsPending"] = isPending;
            ViewData["isOwner"] = isOwner;
            ViewData["ulubiony"] = isFavorite;

            return View(cruises);
        }

        // GET: Cruises/Create
        [Authorize(Roles = "Moderacja,Kapitan")]
        public IActionResult Create()
        {
            /*ViewData["CapitanId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["YachtId"] = new SelectList(_context.Yachts, "Id", "name");
            return View();*/

            // Pobranie zalogowanego użytkownika jako string
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Sprawdzenie, czy wartość istnieje i jest poprawnym Guidem
            if (userIdString != null && Guid.TryParse(userIdString, out Guid ownerId))
            {
                // Filtrowanie jachtów tylko dla zalogowanego użytkownika
                var userYachts = _context.Yachts.Where(y => y.OwnerId == ownerId).ToList();

                // Przekazanie tylko jachtów zalogowanego użytkownika do widoku
                ViewData["YachtId"] = new SelectList(userYachts, "Id", "name");
            }
            else
            {
                // Jeśli użytkownik nie jest zalogowany, lista jachtów będzie pusta
                ViewData["YachtId"] = new SelectList(Enumerable.Empty<object>(), "Id", "name");
                ModelState.AddModelError(string.Empty, "Nie jesteś zalogowany. Proszę zalogować się.");
            }

            return View();
        }

        // POST: Cruises/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Moderacja,Kapitan")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,name,description,destination,start_date,end_date,price,currency,maxParticipants,YachtId")] Cruises cruises)
        {
            // Pobranie zalogowanego użytkownika jako string
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Sprawdzenie, czy wartość istnieje
            if (userIdString != null && Guid.TryParse(userIdString, out Guid ownerId))
            {
                cruises.CapitanId = ownerId;
            }
            else
            {
                // Obsługa błędu: brak lub nieprawidłowy identyfikator użytkownika
                //ModelState.AddModelError(string.Empty, "Nie można przypisać użytkownika jako właściciela.");
                // Obsługa błędu: brak lub nieprawidłowy identyfikator użytkownika
                ModelState.AddModelError(string.Empty, "Nie jesteś zalogowany jako właściciel. Proszę zalogować się.");
                return View(cruises);
            }

            ModelState.Clear();
            cruises.status = CruiseStatus.Planned;
            if (ModelState.IsValid)
            {
                _context.Add(cruises);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["CapitanId"] = new SelectList(_context.Users, "Id", "Id", cruises.CapitanId);
            ViewData["YachtId"] = new SelectList(_context.Yachts, "Id", "name", cruises.YachtId);
            return View(cruises);
        }

        // GET: Cruises/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cruises = await _context.Cruises.FindAsync(id);
            if (cruises == null)
            {
                return NotFound();
            }
            // Pobranie zalogowanego użytkownika jako string
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Sprawdzenie, czy wartość istnieje i jest poprawnym Guidem
            if (userIdString != null && Guid.TryParse(userIdString, out Guid ownerId))
            {
                // Filtrowanie jachtów tylko dla zalogowanego użytkownika
                var userYachts = _context.Yachts.Where(y => y.OwnerId == ownerId).ToList();

                // Przekazanie tylko jachtów zalogowanego użytkownika do widoku
                ViewData["YachtId"] = new SelectList(userYachts, "Id", "name");
            }
            else
            {
                // Jeśli użytkownik nie jest zalogowany, lista jachtów będzie pusta
                ViewData["YachtId"] = new SelectList(Enumerable.Empty<object>(), "Id", "name");
                ModelState.AddModelError(string.Empty, "Nie jesteś zalogowany. Proszę zalogować się.");
            }

            //ViewData["CapitanId"] = new SelectList(_context.Users, "Id", "Id", cruises.CapitanId);
            ViewData["YachtId"] = new SelectList(_context.Yachts, "Id", "name", cruises.YachtId);
            return View(cruises);
        }

        // POST: Cruises/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,name,description,destination,start_date,end_date,price,currency,status,maxParticipants,YachtId,CapitanId")] Cruises cruises)
        {
            if (id != cruises.Id)
            {
                return NotFound();
            }

            // Pobranie zalogowanego użytkownika jako string
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Sprawdzenie, czy wartość istnieje
            if (userIdString != null && Guid.TryParse(userIdString, out Guid ownerId))
            {
                cruises.CapitanId = ownerId;
            }
            else
            {
                // Obsługa błędu: brak lub nieprawidłowy identyfikator użytkownika
                //ModelState.AddModelError(string.Empty, "Nie można przypisać użytkownika jako właściciela.");
                // Obsługa błędu: brak lub nieprawidłowy identyfikator użytkownika
                ModelState.AddModelError(string.Empty, "Nie jesteś zalogowany jako właściciel. Proszę zalogować się.");
                return View(cruises);
            }

            ModelState.Clear();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cruises);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CruisesExists(cruises.Id))
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
            ViewData["CapitanId"] = new SelectList(_context.Users, "Id", "Id", cruises.CapitanId);
            ViewData["YachtId"] = new SelectList(_context.Yachts, "Id", "name", cruises.YachtId);
            return View(cruises);
        }

        // GET: Cruises/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cruises = await _context.Cruises
                .Include(c => c.Capitan)
                .Include(c => c.Yacht)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cruises == null)
            {
                return NotFound();
            }

            return View(cruises);
        }

        // POST: Cruises/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cruises = await _context.Cruises.FindAsync(id);
            if (cruises != null)
            {
                _context.Cruises.Remove(cruises);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CruisesExists(int id)
        {
            return _context.Cruises.Any(e => e.Id == id);
        }
    }
}
