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
using Microsoft.AspNetCore.Authorization;

namespace Inzynierka.Controllers
{
    /*[Authorize(Roles = "User,Moderacja,Kapitan")]*/
    public class YachtSalesController : Controller
    {
        private readonly AhoyDbContext _context;

        public YachtSalesController(AhoyDbContext context)
        {
            _context = context;
        }

        private Guid? GetLoggedInUserId()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(userIdString, out Guid userId) ? userId : null;
        }

        // GET: YachtSales
        /* public async Task<IActionResult> Index()
         {
             *//*if(GetLoggedInUserId() == null)
             {
                 return NotFound();
             }*//*
             var ahoyDbContext = _context.YachtSale.Include(y => y.BuyerUser).Include(y => y.Owner).Include(y => y.Yacht);
             return View(await ahoyDbContext.ToListAsync());
         }*/
        public async Task<IActionResult> Index()
        {
            var userIdGuid = GetLoggedInUserId();

            List<YachtSale> userSales = new List<YachtSale>();
            List<YachtSale> otherSales;

            bool isLogged = false;

            if (userIdGuid != null)
            {
                // Pobranie ofert sprzedaży użytkownika
                userSales = await _context.YachtSale
                    .Where(s => s.OwnerId == userIdGuid && s.status != TransactionStatus.Accepted)
                    .Include(s => s.Yacht)
                    .Include(s => s.Owner)
                    .ToListAsync();
                isLogged = true;
            }

            // Pobranie ofert sprzedaży innych użytkowników
            otherSales = await _context.YachtSale
                .Where(s => s.OwnerId != userIdGuid && !s.Owner.banned && s.status != TransactionStatus.Accepted)
                .Include(s => s.Yacht)
                .Include(s => s.Owner)
                .ToListAsync();

            // Przekazanie danych do modelu widoku
            var model = new YachtSalesIndexViewModel
            {
                UserSales = userSales,
                OtherSales = otherSales
            };

            ViewData["isLogged"] = isLogged;

            return View(model);
        }


        // GET: YachtSales/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var yachtSale = await _context.YachtSale
                .Include(y => y.BuyerUser)
                .Include(y => y.Owner)
                .Include(y => y.Yacht)
                .Include(y => y.Comments)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (yachtSale == null)
            {
                return NotFound();
            }

            // Sprawdź, czy jacht jest w ulubionych
            var loggedInUserId = GetLoggedInUserId();
            bool isFavorite = await _context.FavoriteYachtsForSale
                .AnyAsync(f => f.YachtSaleId == id && f.UserId == loggedInUserId);

            ViewData["ulubiony"] = isFavorite;
            bool isOwner = loggedInUserId != null && yachtSale.Owner.Id == loggedInUserId;
            
            ViewData["isOwner"] = isOwner;
            
            ViewData["isLogged"] = loggedInUserId != null ? true : false;

            return View(yachtSale);
        }

        // GET: YachtSales/Create
        /*public IActionResult Create()
        {
            ViewData["BuyerUserId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["OwnerId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["YachtId"] = new SelectList(_context.Yachts, "Id", "name");
            return View();
        }*/
        // Obsługa zakupu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Buy(int id)
        {
            var yachtSale = await _context.YachtSale.FindAsync(id);

            if (yachtSale == null)
            {
                TempData["Message"] = "Jacht nie został znaleziony.";
                TempData["AlertType"] = "danger";
                return RedirectToAction("Index", "Home");
                /*return NotFound();*/
            }

            var loggedInUserId = GetLoggedInUserId(); // Funkcja pobierająca ID zalogowanego użytkownika

            if (loggedInUserId == null)
            {
                /*return RedirectToAction("Login", "Account");*/
                TempData["Message"] = "Musisz być zalogowany, aby dokonać zakupu.";
                TempData["AlertType"] = "warning";
                return RedirectToAction("Index", "Home");
            }

            if (yachtSale.BuyerUserId != null)
            {
                TempData["Message"] = "Ten jacht został już sprzedany.";
                TempData["AlertType"] = "danger";
                return RedirectToAction(nameof(Details), new { id });
            }

            yachtSale.BuyerUserId = loggedInUserId.Value;
            yachtSale.status = TransactionStatus.Pending; // Ustaw status na "Oczekujący"
            
            // Zakładając, że posiadasz właściwość Yacht, która wskazuje na aktualnego właściciela jachtu
            var yacht = await _context.Yachts.FindAsync(yachtSale.YachtId); // Wyszukiwanie jachtu
            if (yacht != null)
            {
                yacht.OwnerId = loggedInUserId.Value; // Zmiana właściciela jachtu na nowego użytkownika
                _context.Update(yacht); // Zaktualizowanie rekordu jachtu
            }
            _context.Update(yachtSale);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Zakup został zainicjowany. Oczekuje na akceptację sprzedającego.";
            TempData["AlertType"] = "success";

            return RedirectToAction(nameof(Details), new { id });
        }

        // Akceptacja zakupu przez właściciela
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Accept(int id)
        {
            var yachtSale = await _context.YachtSale.FindAsync(id);

            if (yachtSale == null)
            {
                return NotFound();
            }

            if (yachtSale.status != TransactionStatus.Pending)
            {
                TempData["Message"] = "Ta transakcja nie jest już w stanie oczekiwania.";
                TempData["AlertType"] = "warning";
                return RedirectToAction(nameof(Details), new { id });
            }

            yachtSale.status = TransactionStatus.Accepted;

            // Ustaw nowego właściciela jachtu na kupującego
            if (yachtSale.Yacht != null && yachtSale.BuyerUserId != null)
            {
                yachtSale.Yacht.OwnerId = (Guid)yachtSale.BuyerUserId;
                _context.Update(yachtSale.Yacht);
            }


            _context.Update(yachtSale);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Zakup został zaakceptowany.";
            TempData["AlertType"] = "success";

            return RedirectToAction(nameof(Details), new { id });
        }

        // Odrzucenie zakupu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            var yachtSale = await _context.YachtSale.FindAsync(id);

            if (yachtSale == null)
            {
                return NotFound();
            }

            if (yachtSale.status != TransactionStatus.Pending)
            {
                TempData["Message"] = "Ta transakcja nie jest już w stanie oczekiwania.";
                TempData["AlertType"] = "warning";
                return RedirectToAction(nameof(Details), new { id });
            }

            yachtSale.status = TransactionStatus.Pending;
            yachtSale.BuyerUserId = null; // Resetuj kupującego

            _context.Update(yachtSale);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Zakup został odrzucony.";
            TempData["AlertType"] = "danger";

            return RedirectToAction(nameof(Details), new { id });
        }

        public IActionResult Create()
        {
            // Pobranie ID zalogowanego użytkownika
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userIdString == null || !Guid.TryParse(userIdString, out Guid ownerId))
            {
                return NotFound(); // Jeśli użytkownik nie jest zalogowany, zwróć 404
            }

            // Pobranie tylko jachtów należących do zalogowanego użytkownika
            var userYachts = _context.Yachts
                .Where(y => y.OwnerId == ownerId) // Filtr na właściciela
                .Select(y => new { y.Id, y.name }) // Pobranie tylko potrzebnych danych
                .ToList();

            // Ustawienie listy w ViewData dla selecta
            ViewData["YachtId"] = new SelectList(userYachts, "Id", "name");

            return View();
        }

        // POST: YachtSales/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,price,currency,location,notes,YachtId")] YachtSale yachtSale)
        {
            // Pobranie zalogowanego użytkownika jako string
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Sprawdzenie, czy wartość istnieje
            if (userIdString != null && Guid.TryParse(userIdString, out Guid ownerId))
            {
                yachtSale.OwnerId = ownerId;
            }
            // Sprawdzenie, czy jacht jest dostępny do sprzedaży
            var yacht = await _context.Yachts
                .Include(y => y.Charters)
                .Include(y => y.Cruises)
                .FirstOrDefaultAsync(y => y.Id == yachtSale.YachtId);

            if (yacht == null)
            {
                TempData["Message"] = "Wybrany jacht nie istnieje.";
                TempData["AlertType"] = "danger"; // Typ alertu: błąd
                return View(yachtSale);
            }
            // Sprawdzenie czarterów
            var currentDate = DateOnly.FromDateTime(DateTime.Now);
            if (yacht.Charters.Any(c => c.startDate <= currentDate && c.endDate >= currentDate))
            {
                TempData["Message"] = "Nie można sprzedać jachtu, ponieważ jest obecnie czarterowany.";
                TempData["AlertType"] = "danger"; // Typ alertu: błąd
                return View(yachtSale);
            }

            // Sprawdzenie rejsów
            if (yacht.Cruises.Any(c => c.start_date <= currentDate && c.end_date >= currentDate))
            {
                TempData["Message"] = "Nie można sprzedać jachtu, ponieważ ma zaplanowane rejsy.";
                TempData["AlertType"] = "danger"; // Typ alertu: błąd
                return View(yachtSale);
            }
            if (yacht.Charters.Any(c => c.startDate >= currentDate))
            {
                TempData["Message"] = "Nie można sprzedać jachtu, ponieważ ma obecne lub zaplanowane czartery.";
                TempData["AlertType"] = "danger";
                return View(yachtSale);
            }

            // Sprawdzenie bieżących i zaplanowanych rejsów
            if (yacht.Cruises.Any(c => c.start_date >= currentDate))
            {
                TempData["Message"] = "Nie można sprzedać jachtu, ponieważ ma obecne lub zaplanowane rejsy.";
                TempData["AlertType"] = "danger";
                return View(yachtSale);
            }
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                _context.Add(yachtSale);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Jacht został pomyślnie wystawiony na sprzedaż!";
                TempData["AlertType"] = "success"; // Typ alertu: sukces
                return RedirectToAction(nameof(Index));
            }

            ViewData["BuyerUserId"] = new SelectList(_context.Users, "Id", "Id", yachtSale.BuyerUserId);
            ViewData["OwnerId"] = new SelectList(_context.Users, "Id", "Id", yachtSale.OwnerId);
            ViewData["YachtId"] = new SelectList(_context.Yachts, "Id", "name", yachtSale.YachtId);
            return View(yachtSale);
        }

        // GET: YachtSales/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var yachtSale = await _context.YachtSale.FindAsync(id);
            if (yachtSale == null)
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

            ViewData["BuyerUserId"] = new SelectList(_context.Users, "Id", "Id", yachtSale.BuyerUserId);
            ViewData["OwnerId"] = new SelectList(_context.Users, "Id", "Id", yachtSale.OwnerId);
            /*ViewData["YachtId"] = new SelectList(_context.Yachts, "Id", "name", yachtSale.YachtId);*/
            return View(yachtSale);
        }

        // POST: YachtSales/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,saleDate,price,currency,location,availabilityStatus,status,notes,YachtId,BuyerUserId,OwnerId")] YachtSale yachtSale)
        {
            if (id != yachtSale.Id)
            {
                /*return NotFound();*/
                TempData["Message"] = "Nie znaleziono wskazanej sprzedaży jachtu.";
                TempData["AlertType"] = "danger";
                return RedirectToAction(nameof(Index));
            }
            // Pobranie jachtu do sprawdzenia jego dostępności
            var yacht = await _context.Yachts
                .Include(y => y.Charters)
                .Include(y => y.Cruises)
                .FirstOrDefaultAsync(y => y.Id == yachtSale.YachtId);

            if (yacht == null)
            {
                TempData["Message"] = "Wybrany jacht nie istnieje.";
                TempData["AlertType"] = "danger";
                return RedirectToAction(nameof(Index));
            }

            // Sprawdzenie czarterów
            var currentDate = DateOnly.FromDateTime(DateTime.Now);
            if (yacht.Charters.Any(c => c.startDate <= currentDate && c.endDate >= currentDate))
            {
                TempData["Message"] = "Nie można edytować sprzedaży jachtu, ponieważ jest obecnie czarterowany.";
                TempData["AlertType"] = "danger";
                return View(yachtSale);
            }

            // Sprawdzenie rejsów
            if (yacht.Cruises.Any(c => c.start_date <= currentDate && c.end_date >= currentDate))
            {
                TempData["Message"] = "Nie można edytować sprzedaży jachtu, ponieważ ma zaplanowane rejsy.";
                TempData["AlertType"] = "danger";
                return View(yachtSale);
            }

            if (yacht.Charters.Any(c => c.startDate >= currentDate))
            {
                TempData["Message"] = "Nie można sprzedać jachtu, ponieważ ma obecne lub zaplanowane czartery.";
                TempData["AlertType"] = "danger";
                    
            }

            // Sprawdzenie bieżących i zaplanowanych rejsów
            if (yacht.Cruises.Any(c => c.start_date >= currentDate))
            {
                TempData["Message"] = "Nie można sprzedać jachtu, ponieważ ma obecne lub zaplanowane rejsy.";
                TempData["AlertType"] = "danger";
                return View(yachtSale);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(yachtSale);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!YachtSaleExists(yachtSale.Id))
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
            ViewData["BuyerUserId"] = new SelectList(_context.Users, "Id", "Id", yachtSale.BuyerUserId);
            ViewData["OwnerId"] = new SelectList(_context.Users, "Id", "Id", yachtSale.OwnerId);
            ViewData["YachtId"] = new SelectList(_context.Yachts, "Id", "name", yachtSale.YachtId);
            return View(yachtSale);
        }

        // GET: YachtSales/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var yachtSale = await _context.YachtSale
                .Include(c => c.Comments.Where(comment => !comment.Creator.banned)).ThenInclude(comment => comment.Creator)
                .Include(y => y.BuyerUser)
                .Include(y => y.Owner)
                .Include(y => y.Yacht)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (yachtSale == null)
            {
                return NotFound();
            }

            return View(yachtSale);
        }

        // POST: YachtSales/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var yachtSale = await _context.YachtSale.FindAsync(id);
            if (yachtSale != null)
            {
                _context.YachtSale.Remove(yachtSale);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool YachtSaleExists(int id)
        {
            return _context.YachtSale.Any(e => e.Id == id);
        }
    }
}
