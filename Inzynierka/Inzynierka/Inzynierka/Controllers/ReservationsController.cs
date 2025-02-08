/**/using System;
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
    public class ReservationsController : Controller
    {
        private readonly AhoyDbContext _context;

        public ReservationsController(AhoyDbContext context)
        {
            _context = context;
        }
        private Guid? GetLoggedInUserId()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(userIdString, out Guid userId) ? userId : null;
        }


        // GET: Reservations
        /*  public async Task<IActionResult> Index()
          {
              var ahoyDbContext = _context.Resservation
                  .Include(r => r.Charter)
                  .Include(r => r.User);
              return View(await ahoyDbContext.ToListAsync());
          }*/
        public async Task<IActionResult> Index()
        {
            // Pobierz ID zalogowanego użytkownika
            var userId = GetLoggedInUserId();
            if (userId == null)
            {
                return NotFound("Nie znaleziono zalogowanego użytkownika.");
            }

            // Pobierz rezerwacje, które należą do zalogowanego użytkownika
            var userReservations = _context.Resservation
                .Include(r => r.Charter).ThenInclude(c => c.Owner) // Załaduj dane o czarterze
                .Include(r => r.User)
                .Where(r => r.UserId == userId); // Filtrowanie rezerwacji po UserId

            return View(await userReservations.ToListAsync());
        }
        public async Task<IActionResult> Zarzadzanie()
        {
            // Pobierz ID zalogowanego użytkownika
            var userId = GetLoggedInUserId();
            if (userId == null)
            {
                return NotFound("Nie znaleziono zalogowanego użytkownika.");
            }

            // Pobierz rezerwacje, gdzie użytkownik jest właścicielem czarteru
            var ahoyDbContext = _context.Resservation
                .Include(r => r.Charter)
                .Include(r => r.User)
                .Where(r => r.Charter.OwnerId == userId); // Załóżmy, że w tabeli `Charters` masz pole `OwnerId`, które przechowuje ID właściciela

            return View(await ahoyDbContext.ToListAsync());
        }

        // GET: Reservations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Resservation
                .Include(r => r.Charter).ThenInclude(c => c.Owner) // Załaduj dane o właścicielu czarteru
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // GET: Reservations/Create
        public IActionResult Create(int? charterId)
         {
            if (charterId == null)
            {
                return NotFound("Brak wymaganego ID czarteru.");
            }

            var charter = _context.Charters.FirstOrDefault(c => c.Id == charterId);
            if (charter == null)
            {
                return NotFound("Czarter nie został znaleziony.");
            }
            // Pobranie istniejących rezerwacji dla tego czarteru
            var reservations = _context.Resservation
                .Where(r => r.CharterId == charterId)
                .Where(r => r.Status == StatusReservation.Accepted) // Uwzględniamy tylko zatwierdzone rezerwacje
                .Select(r => new
                {
                    StartDate = r.startDate.ToString("yyyy-MM-dd"),
                    EndDate = r.endDate.ToString("yyyy-MM-dd")
                })
                .ToList();

            /*//*ViewData["CharterId"] = new SelectList(_context.Charters, "Id", "currency");*/
            ViewData["CharterStartDate"] = charter.startDate.ToString("yyyy-MM-dd");
            ViewData["CharterEndDate"] = charter.endDate.ToString("yyyy-MM-dd");
            ViewData["CharterId"] = new SelectList(_context.Charters, "Id", "currency", charterId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["ReservedDates"] = reservations;
            return View();
         }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AcceptReservation(int id)
        {
            // Znajdź rezerwację, którą chcemy zaakceptować
            var reservationToAccept = await _context.Resservation.FindAsync(id);
            if (reservationToAccept == null)
            {
                return NotFound("Rezerwacja nie została znaleziona.");
            }

            // Ustaw status na "Accepted"
            reservationToAccept.Status = StatusReservation.Accepted;

            // Znajdź wszystkie rezerwacje nakładające się terminami i zmień ich status na "Rejected"
            var overlappingReservations = _context.Resservation
                .Where(r => r.CharterId == reservationToAccept.CharterId) // Tylko dla tego samego czarteru
                .Where(r => r.Id != id) // Pomijamy rezerwację, którą akceptujemy
                .Where(r =>
                    r.Status != StatusReservation.Rejected && // Tylko rezerwacje z innym statusem
                    r.startDate < reservationToAccept.endDate && // Sprawdzenie, czy terminy się pokrywają
                    r.endDate > reservationToAccept.startDate
                );

            foreach (var reservation in overlappingReservations)
            {
                reservation.Status = StatusReservation.Rejected;
            }

            // Zapisz zmiany w bazie danych
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index)); // Powrót do listy rezerwacji
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectReservation(int id)
        {
            // Znajdź rezerwację, którą chcemy odrzucić
            var reservationToReject = await _context.Resservation.FindAsync(id);
            if (reservationToReject == null)
            {
                return NotFound("Rezerwacja nie została znaleziona.");
            }

            // Zmień status na "Rejected"
            reservationToReject.Status = StatusReservation.Rejected;

            // Zapisz zmiany w bazie danych
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index)); // Powrót do listy rezerwacji
        }

        // POST: Reservations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,startDate,endDate,CharterId")] Reservation reservation)
        {
            var userId = GetLoggedInUserId();
            
            if (userId == null)
            {
                return NotFound("Nie znaleziono zalogowanego użytkownika.");
            }

            reservation.UserId = userId;
            var users = _context.Users.FirstOrDefault(c => c.Id == userId);
            var Charters = _context.Charters.FirstOrDefault(c => c.Id == reservation.CharterId);
            if (users == null && Charters == null)
            {
                return NotFound("Nie znaleziono użytkownika albo Charters.");
            }
            reservation.User = users;
            reservation.Charter = Charters;
            reservation.Status = StatusReservation.Pending;

            var overlappingReservation = _context.Resservation
            .Where(r => r.CharterId == reservation.CharterId)
            .Where(r => r.startDate < reservation.endDate && r.endDate > reservation.startDate)
            .FirstOrDefault();

            if (overlappingReservation != null)
            {
              /*  // Jeśli istnieje nakładająca się rezerwacja z innym statusem niż Pending lub Rejected, odrzuć nową
                if (overlappingReservation.Status != StatusReservation.Pending && overlappingReservation.Status != StatusReservation.Rejected)
                {
                    ModelState.AddModelError(string.Empty, "Istnieje już aktywna rezerwacja w tym zakresie dat.");
                    ViewData["CharterId"] = new SelectList(_context.Charters, "Id", "currency", reservation.CharterId);
                    ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", reservation.UserId);
                    return View(reservation);
                }*/

                // Jeśli istniejąca rezerwacja jest Rejected, nowa rezerwacja również zostanie odrzucona
                if (overlappingReservation.Status == StatusReservation.Rejected)
                {
                    ModelState.AddModelError(string.Empty, "Nie można utworzyć rezerwacji, ponieważ istnieje odrzucona rezerwacja w tym zakresie dat.");
                    ViewData["CharterId"] = new SelectList(_context.Charters, "Id", "currency", reservation.CharterId);
                    ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", reservation.UserId);
                    return View(reservation);
                }
            }

                if (ModelState.IsValid)
            {
                _context.Add(reservation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToArray();

            }
            ViewData["CharterId"] = new SelectList(_context.Charters, "Id", "currency", reservation.CharterId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", reservation.UserId);
            return View(reservation);
        }

        // GET: Reservations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Resservation.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }
            ViewData["CharterId"] = new SelectList(_context.Charters, "Id", "currency", reservation.CharterId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", reservation.UserId);
            return View(reservation);
        }

        // POST: Reservations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,startDate,endDate,status,banned,CharterId,UserId")] Reservation reservation)
        {
            if (id != reservation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reservation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservationExists(reservation.Id))
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
            ViewData["CharterId"] = new SelectList(_context.Charters, "Id", "currency", reservation.CharterId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", reservation.UserId);
            return View(reservation);
        }

        // GET: Reservations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Resservation
                .Include(r => r.Charter)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // POST: Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reservation = await _context.Resservation.FindAsync(id);
            if (reservation != null)
            {
                _context.Resservation.Remove(reservation);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservationExists(int id)
        {
            return _context.Resservation.Any(e => e.Id == id);
        }
    }
}
