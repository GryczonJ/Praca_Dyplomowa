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
using Microsoft.IdentityModel.Tokens;

namespace Inzynierka.Controllers
{
    [Authorize(Roles = "User,Moderacja,Kapitan")]
    public class CommentsController : Controller
    {
        private readonly AhoyDbContext _context;

        public CommentsController(AhoyDbContext context)
        {
            _context = context;
        }


        private Guid? GetLoggedInUserId()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(userIdString, out Guid userId) ? userId : null;
        }
        // GET: Comments
        public async Task<IActionResult> Index()
        {
            var loggedInUserId = GetLoggedInUserId();
            bool isLogged = loggedInUserId != null;

            var ahoyDbContext = _context.Comments
                .Include(c => c.Charter)
                .Include(c => c.Creator)
                .Include(c => c.Cruises)
                .Include(c => c.Profile)
                .Include(c => c.Yachts)
                .Where(c => c.CreatorId == loggedInUserId);
            return View(await ahoyDbContext.ToListAsync());
        }

        // GET: Comments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comments = await _context.Comments
                .Include(c => c.Charter)
                .Include(c => c.Creator)
                .Include(c => c.Cruises)
                .Include(c => c.Profile)
                .Include(c => c.Yachts)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (comments == null)
            {
                return NotFound();
            }

            return View(comments);
        }

        // GET: Comments/Create
        public IActionResult Create()
        {
            ViewData["CharterId"] = new SelectList(_context.Charters, "Id", "currency");
            ViewData["CreatorId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["CruisesId"] = new SelectList(_context.Cruises, "Id", "currency");
            ViewData["ProfileId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["YachtsId"] = new SelectList(_context.Yachts, "Id", "name");
            return View();
        }

        // POST: Comments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //ModelState.Clear();
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Message,CreateDate,Rating,CreatorId,ProfileId,CharterId,CruisesId,YachtsId,YachtSaleId")] Comments comments)
        {
            try
            {
                if (comments.Message == null) { comments.Message = ""; }
                if (comments.Rating == null) { comments.Rating = 1; }
                if (ModelState.IsValid)
                {
                    _context.Add(comments);
                    await _context.SaveChangesAsync();
                    return Redirect(Request.Headers["Referer"].ToString()); // Powrót do strony poprzedniej
                }
                else
                {
                    foreach (var state in ModelState)
                    {
                        foreach (var error in state.Value.Errors)
                        {
                            Console.WriteLine($"Pole: {state.Key}, Błąd: {error.ErrorMessage}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Obsługa wyjątków i ustawienie komunikatu o błędzie
                TempData["Message"] = $"Wystąpił błąd podczas dodawania komentarza: {ex.Message}";
                TempData["AlertType"] = "danger"; // Alert typu "błąd"
            }
            ViewData["CharterId"] = new SelectList(_context.Charters, "Id", "currency", comments.CharterId);
            ViewData["CreatorId"] = new SelectList(_context.Users, "Id", "Id", comments.CreatorId);
            ViewData["CruisesId"] = new SelectList(_context.Cruises, "Id", "currency", comments.CruisesId);
            ViewData["ProfileId"] = new SelectList(_context.Users, "Id", "Id", comments.ProfileId);
            ViewData["YachtsId"] = new SelectList(_context.Yachts, "Id", "name", comments.YachtsId);
            ViewData["YachtSaleId"] = new SelectList(_context.YachtSale, "Id", "saleDate", comments.YachtSaleId);
            //return View(comments);
            return Redirect(Request.Headers["Referer"].ToString());
        }

        // GET: Comments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comments = await _context.Comments.FindAsync(id);
            if (comments == null)
            {
                return NotFound();
            }
            ViewData["CharterId"] = new SelectList(_context.Charters, "Id", "currency", comments.CharterId);
            ViewData["CreatorId"] = new SelectList(_context.Users, "Id", "Id", comments.CreatorId);
            ViewData["CruisesId"] = new SelectList(_context.Cruises, "Id", "currency", comments.CruisesId);
            ViewData["ProfileId"] = new SelectList(_context.Users, "Id", "Id", comments.ProfileId);
            ViewData["YachtsId"] = new SelectList(_context.Yachts, "Id", "name", comments.YachtsId);
            return View(comments);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Message,CreateDate,Rating,CreatorId,ProfileId,CharterId,CruisesId,YachtsId")] Comments comments)
        {
            var komentarz = await _context.Comments
                             .Include(c => c.Creator) // Jeżeli potrzebujesz szczegółowych danych o twórcy
                             .FirstOrDefaultAsync(c => c.Id == id);
            if (id != comments.Id)
            {
                TempData["Message"] = "Edycja nie powiodła się: błędny identyfikator komentarza.";
                TempData["AlertType"] = "danger";
                return RedirectToAction("Index");
                /*return NotFound();*/
            }
            else if (komentarz == null)
            {
                TempData["Message"] = "Edycja nie powiodła się: komentarz nie został znaleziony.";
                TempData["AlertType"] = "warning";
                return RedirectToAction("Index");
               /* return NotFound();*/
            }

            if (ModelState.IsValid)
            {

                try
                {
                    komentarz.Message = comments.Message;
                    komentarz.Rating = comments.Rating;
                    _context.Update(komentarz);
                    await _context.SaveChangesAsync();
                    // Komunikat o sukcesie
                   
                    TempData["Message"] = "Komentarz został pomyślnie zaktualizowany!";
                    TempData["AlertType"] = "success"; // Typ alertu: sukces
                }
                catch (DbUpdateConcurrencyException)
                {
                    TempData["Message"] = "Wystąpił błąd podczas aktualizacji komentarza.";
                    TempData["AlertType"] = "danger"; // Typ alertu: błąd
                }
                return RedirectToAction(nameof(Index));
            }
                TempData["Message"] = "Nie udało się zaktualizować komentarza. Sprawdź wprowadzone dane.";
                TempData["AlertType"] = "warning"; // Typ alertu: ostrzeżenie
            ViewData["CharterId"] = new SelectList(_context.Charters, "Id", "currency", comments.CharterId);
            ViewData["CreatorId"] = new SelectList(_context.Users, "Id", "Id", comments.CreatorId);
            ViewData["CruisesId"] = new SelectList(_context.Cruises, "Id", "currency", comments.CruisesId);
            ViewData["ProfileId"] = new SelectList(_context.Users, "Id", "Id", comments.ProfileId);
            ViewData["YachtsId"] = new SelectList(_context.Yachts, "Id", "name", comments.YachtsId);
            return View(comments);
        }

        // GET: Comments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comments = await _context.Comments
                .Include(c => c.Charter)
                .Include(c => c.Creator)
                .Include(c => c.Cruises)
                .Include(c => c.Profile)
                .Include(c => c.Yachts)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (comments == null)
            {
                return NotFound();
            }

            return View(comments);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var comments = await _context.Comments.FindAsync(id);
            if (comments != null)
            {
                _context.Comments.Remove(comments);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CommentsExists(int id)
        {
            return _context.Comments.Any(e => e.Id == id);
        }


    }
}
