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
        public async Task<IActionResult> Index()
        {
            var ahoyDbContext = _context.Resservation.Include(r => r.Charter).Include(r => r.User);
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
                .Include(r => r.Charter)
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
            /*//*ViewData["CharterId"] = new SelectList(_context.Charters, "Id", "currency");*/
            ViewData["CharterStartDate"] = charter.startDate.ToString("yyyy-MM-dd");
            ViewData["CharterEndDate"] = charter.endDate.ToString("yyyy-MM-dd");
            ViewData["CharterId"] = new SelectList(_context.Charters, "Id", "currency", charterId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
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

            if (users == null)
            {
                return NotFound("Nie znaleziono użytkownika.");
            }
            reservation.User = users;
            reservation.status = RequestStatus.Pending;

            if (ModelState.IsValid)
            {
                _context.Add(reservation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
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
