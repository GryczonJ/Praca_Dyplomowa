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
using Inzynierka.Models;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Authorization;

namespace Inzynierka.Controllers
{
    /*[Authorize(Roles = "User,Moderacja,Kapitan")]*/
    public class ChartersController : Controller
    {
        private readonly AhoyDbContext _context;

        public ChartersController(AhoyDbContext context)
        {
            _context = context;
        }
        private Guid? GetLoggedInUserId()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(userIdString, out Guid userId) ? userId : null;
        }

        // GET: Charters
        /* public async Task<IActionResult> Index()
         {
             var ahoyDbContext = _context.Charters.Include(c => c.Owner).Include(c => c.Yacht);
             return View(await ahoyDbContext.ToListAsync());
         }
         */
        public async Task<IActionResult> Index()
        {
            var loggedInUserId = GetLoggedInUserId();
            bool isLogged = loggedInUserId != null;

            var charters = await _context.Charters
             .Include(c => c.Owner)
             .Include(c => c.Yacht)
             .ToListAsync();

            var myCharters = isLogged
                ? charters.Where(c => c.Owner.Id == loggedInUserId).ToList()
                : new List<Charters>();


            var otherCharters = charters.Where(c => c.Owner.Id != loggedInUserId).ToList();

            var viewModel = new ChartersViewModel
            {
                MyCharters = myCharters,
                OtherCharters = otherCharters
            };

            ViewData["isLogged"] = isLogged;
            return View(viewModel);
        }



        // GET: Charters/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var charters = await _context.Charters
                .Include(c => c.Owner)
                .Include(c => c.Yacht)
                .Include(c=>c.Comments)
                /*.ThenInclude(c=>c.Creator)*/
                .FirstOrDefaultAsync(m => m.Id == id);
            if (charters == null)
            {
                return NotFound();
            }

            Guid? loggedUserId = GetLoggedInUserId();
            bool isOwner = charters.OwnerId == loggedUserId;
            bool isLogged = loggedUserId.HasValue;

            ViewData["isOwner"] = isOwner;
            ViewData["isLogged"] = isLogged;

            return View(charters);
        }

        // GET: Charters/Create
        public IActionResult Create()
        {// Pobranie zalogowanego użytkownika jako string
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

         /*   ViewData["OwnerId"] = new SelectList(_context.Users, "Id", "name");
            ViewData["YachtId"] = new SelectList(_context.Yachts, "Id", "name");*/
            return View();
        }

        // POST: Charters/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,startDate,endDate,price,location,description,currency,YachtId")] Charters charters)
        {
            var loggedInUserId = GetLoggedInUserId();

            if (loggedInUserId == null)
            {
                // Jeśli użytkownik nie jest zalogowany, możesz wywołać wyjątek lub przekierować na stronę logowania
                TempData["Message"] = "Aby dodać czarter, musisz się zalogować.";
                TempData["AlertType"] = "warning"; // Typ alertu: ostrzeżenie
                return RedirectToAction(nameof(Index));
            }

            var loggedInUser = await _context.Users.FindAsync(loggedInUserId);
            if (loggedInUser?.banned == true)
            {
                // Komunikat o błędzie dla zbanowanego użytkownika
                TempData["Message"] = "Twoje konto jest zbanowane. Nie możesz dodawać nowych czarterów.";
                TempData["AlertType"] = "danger"; // Typ alertu: ostrzeżenie
                return RedirectToAction(nameof(Index));
            }
            if (charters.YachtId != null) { 
                // Sprawdzenie, czy dla jachtu nie ma zaplanowanego rejsu
                var currentDate = DateOnly.FromDateTime(DateTime.Now);
                /*var hasPlannedCruise = _context.Cruises
                    .Any(c => c.YachtId == charters.YachtId && c.start_date >= currentDate);*/
                var hasPlannedCruise = _context.Cruises
                   .Any(c => c.YachtId == charters.YachtId &&
                 ((charters.startDate >= c.start_date && charters.startDate <= c.end_date) ||
                  (charters.endDate >= c.start_date && charters.endDate <= c.end_date) ||
                  (charters.startDate <= c.start_date && charters.endDate >= c.end_date)));

                if (hasPlannedCruise)
                {
                    TempData["Message"] = "Nie można dodać czarteru, ponieważ jacht ma zaplanowany rejs.";
                    TempData["AlertType"] = "danger";
                    return RedirectToAction(nameof(Index));
                }

                // Sprawdzenie, czy jacht nie jest wystawiony na sprzedaż
                var isYachtForSale = _context.YachtSale
                    .Any(y => y.YachtId == charters.YachtId && y.status == TransactionStatus.Pending);
                if (isYachtForSale)
                {
                    TempData["Message"] = "Nie można dodać czarteru, ponieważ jacht jest wystawiony na sprzedaż.";
                    TempData["AlertType"] = "danger";
                    return RedirectToAction(nameof(Index));
                }

                // Sprawdzenie, czy istnieje już czarter pokrywający się z podanymi datami
                var overlappingCharter = _context.Charters
                    .Any(c => c.YachtId == charters.YachtId &&
                              ((charters.startDate >= c.startDate && charters.startDate <= c.endDate) ||
                               (charters.endDate >= c.startDate && charters.endDate <= c.endDate) ||
                               (charters.startDate <= c.startDate && charters.endDate >= c.endDate)));

                if (overlappingCharter)
                {
                    TempData["Message"] = "Nie można dodać czarteru, ponieważ pokrywa się z istniejącym czarterem.";
                    TempData["AlertType"] = "danger";
                    return RedirectToAction(nameof(Index));
                }
            }
            // Ustaw właściciela na zalogowanego użytkownika
            charters.OwnerId = loggedInUserId.Value;

            // Ustaw domyślny status na "Planowane"
            charters.status = CharterStatus.Planowane;

            ModelState.Clear();
            if (ModelState.IsValid)
            {
                _context.Add(charters);
                await _context.SaveChangesAsync();
                // Ustaw komunikat o sukcesie
                TempData["Message"] = "Charter został pomyślnie dodany!";
                TempData["AlertType"] = "success"; // Typ alertu: sukces
                return RedirectToAction(nameof(Index));

            }
            ViewData["OwnerId"] = new SelectList(_context.Users, "Id", "Id", charters.OwnerId);
            ViewData["YachtId"] = new SelectList(_context.Yachts, "Id", "name", charters.YachtId);
            return View(charters);
        }

        // GET: Charters/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var charters = await _context.Charters.FindAsync(id);
            if (charters == null)
            {
                return NotFound();
            }

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
            /*  ViewData["OwnerId"] = new SelectList(_context.Users, "Id", "name", charters.OwnerId);
              ViewData["YachtId"] = new SelectList(_context.Yachts, "Id", "name", charters.YachtId);*/

            return View(charters);
        }

        // POST: Charters/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,startDate,endDate,price,location,description,currency,status,YachtId,OwnerId")] Charters charters)
        {
            if (id != charters.Id)
            {
                return NotFound();
            }
            //
            // Pobierz aktualną datę
            var currentDate = DateOnly.FromDateTime(DateTime.Now);
            if (charters.YachtId != null)
            {
                // 1. Sprawdzenie, czy dla jachtu nie ma zaplanowanego rejsu, którego daty pokrywają się z datami czarteru
                /*var hasPlannedCruise = _context.Cruises
                    .Any(c => c.YachtId == charters.YachtId &&
                              ((charters.startDate <= c.end_date && charters.endDate >= c.start_date))); // Sprawdzenie pokrywających się dat*/
                var hasPlannedCruise = _context.Cruises
                .Any(c => c.YachtId == charters.YachtId &&
              ((charters.startDate >= c.start_date && charters.startDate <= c.end_date) ||
               (charters.endDate >= c.start_date && charters.endDate <= c.end_date) ||
               (charters.startDate <= c.start_date && charters.endDate >= c.end_date)));

                if (hasPlannedCruise)
                {
                    TempData["Message"] = "Nie można edytować czarteru, ponieważ jacht ma zaplanowany rejs, którego daty pokrywają się z datami czarteru.";
                    TempData["AlertType"] = "danger";
                    return RedirectToAction(nameof(Index));
                }

                // 2. Sprawdzenie, czy jacht nie jest wystawiony na sprzedaż
                var isYachtForSale = _context.YachtSale
                    .Any(y => y.YachtId == charters.YachtId && y.status == TransactionStatus.Pending);
                if (isYachtForSale)
                {
                    TempData["Message"] = "Nie można edytować czarteru, ponieważ jacht jest wystawiony na sprzedaż.";
                    TempData["AlertType"] = "danger";
                    return RedirectToAction(nameof(Index));
                }

                // 3. Sprawdzenie, czy istnieje już czarter pokrywający się z podanymi datami
                var overlappingCharter = _context.Charters
                    .Any(c => c.YachtId == charters.YachtId &&
                              ((charters.startDate >= c.startDate && charters.startDate <= c.endDate) ||
                               (charters.endDate >= c.startDate && charters.endDate <= c.endDate) ||
                               (charters.startDate <= c.startDate && charters.endDate >= c.endDate)) &&
                              c.Id != charters.Id); // Pomijamy obecny czarter, który jest edytowany
                if (overlappingCharter)
                {
                    TempData["Message"] = "Nie można edytować czarteru, ponieważ pokrywa się z istniejącym czarterem.";
                    TempData["AlertType"] = "danger";
                    return RedirectToAction(nameof(Index));
                }
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(charters);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChartersExists(charters.Id))
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
            ViewData["OwnerId"] = new SelectList(_context.Users, "Id", "Id", charters.OwnerId);
            ViewData["YachtId"] = new SelectList(_context.Yachts, "Id", "name", charters.YachtId);
            return View(charters);
        }

        // GET: Charters/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var charters = await _context.Charters
                .Include(c => c.Comments.Where(comment => !comment.Creator.banned)).ThenInclude(comment => comment.Creator)
                .Include(c => c.Owner)
                .Include(c => c.Yacht)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (charters == null)
            {
                return NotFound();
            }

            return View(charters);
        }

        // POST: Charters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var charters = await _context.Charters.FindAsync(id);
            if (charters != null)
            {
                _context.Charters.Remove(charters);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChartersExists(int id)
        {
            return _context.Charters.Any(e => e.Id == id);
        }
    }
}
