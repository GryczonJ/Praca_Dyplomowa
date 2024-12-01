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

namespace Inzynierka.Controllers
{
    public class YachtsController : Controller
    {
        private readonly AhoyDbContext _context;

        public YachtsController(AhoyDbContext context)
        {
            _context = context;
        }

        // GET: Yachts
        /* public async Task<IActionResult> Index()
         {
             var ahoyDbContext = _context.Yachts.Include(y => y.Image).Include(y => y.Owner);
             return View(await ahoyDbContext.ToListAsync());
         }*/

        // GET: Yachts
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // ID zalogowanego użytkownika
            var userIdGuid = Guid.Parse(userId);

            var userYachts = await _context.Yachts
                .Where(y => y.OwnerId == userIdGuid)
                .Include(y => y.Image)
                .Include(y => y.Owner) // Dodanie Ownera
                .ToListAsync();

            var otherYachts = await _context.Yachts
                .Where(y => y.OwnerId != userIdGuid)
                .Include(y => y.Image)
                .Include(y => y.Owner) // Dodanie Ownera
                .ToListAsync();

            var model = new YachtsIndexViewModel
            {
                UserYachts = userYachts,
                OtherYachts = otherYachts
            };

            return View(model);
        }

        // GET: Yachts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var yachts = await _context.Yachts
                .Include(y => y.Image)
                .Include(y => y.Owner)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (yachts == null)
            {
                return NotFound();
            }

            return View(yachts);
        }

        // GET: Yachts/Create
        public IActionResult Create()
        {
            ViewData["ImageId"] = new SelectList(_context.Image, "Id", "link");
            ViewData["OwnerId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Yachts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,name,description,type,manufacturer,model,year,length,width,crew,cabins,beds,toilets,showers,location,capacity,OwnerId,ImageId,ImageLink")] Yachts yachts)
        {
            // Pobranie zalogowanego użytkownika jako string
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Sprawdzenie, czy wartość istnieje
            if (userIdString != null && Guid.TryParse(userIdString, out Guid ownerId))
            {
                yachts.OwnerId = ownerId;
            }
            else
            {
                // Obsługa błędu: brak lub nieprawidłowy identyfikator użytkownika
                //ModelState.AddModelError(string.Empty, "Nie można przypisać użytkownika jako właściciela.");
                // Obsługa błędu: brak lub nieprawidłowy identyfikator użytkownika
                ModelState.AddModelError(string.Empty, "Nie jesteś zalogowany jako właściciel. Proszę zalogować się.");
                return View(yachts);
            }

            // Jeśli użytkownik wprowadził nowy link do obrazu
            if (!string.IsNullOrWhiteSpace(yachts.ImageLink))
            {
                var newImage = new Image { link = yachts.ImageLink };
                _context.Image.Add(newImage);
                await _context.SaveChangesAsync();

                // Przypisz ID nowo utworzonego obrazu do jachtu
                yachts.ImageId = newImage.Id;
            }

            ModelState.Clear();
            if (ModelState.IsValid)
            {
                _context.Add(yachts);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ImageId"] = new SelectList(_context.Image, "Id", "link", yachts.ImageId);
            ViewData["OwnerId"] = new SelectList(_context.Users, "Id", "Id", yachts.OwnerId);
            return View(yachts);
        }

        // GET: Yachts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var yachts = await _context.Yachts.FindAsync(id);
            if (yachts == null)
            {
                return NotFound();
            }
            ViewData["ImageId"] = new SelectList(_context.Image, "Id", "link", yachts.ImageId);
            ViewData["OwnerId"] = new SelectList(_context.Users, "Id", "Id", yachts.OwnerId);
            return View(yachts);
        }

        // POST: Yachts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,name,description,type,manufacturer,model,year,length,width,crew,cabins,beds,toilets,showers,location,capacity,OwnerId,ImageId,ImageLink")] Yachts yachts)
        {
            // Pobranie zalogowanego użytkownika jako string
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Sprawdzenie, czy wartość istnieje
            if (userIdString != null && Guid.TryParse(userIdString, out Guid ownerId))
            {
                yachts.OwnerId = ownerId;
            }
            else
            {
                // Obsługa błędu: brak lub nieprawidłowy identyfikator użytkownika
                //ModelState.AddModelError(string.Empty, "Nie można przypisać użytkownika jako właściciela.");
                // Obsługa błędu: brak lub nieprawidłowy identyfikator użytkownika
                ModelState.AddModelError(string.Empty, "Nie jesteś zalogowany jako właściciel. Proszę zalogować się.");
                return View(yachts);
            }
            // Jeśli użytkownik wprowadził nowy link do obrazu
            if (!string.IsNullOrWhiteSpace(yachts.ImageLink))
            {
                var newImage = new Image { link = yachts.ImageLink };
                _context.Image.Add(newImage);
                await _context.SaveChangesAsync();

                // Przypisz ID nowo utworzonego obrazu do jachtu
                yachts.ImageId = newImage.Id;
            }

            ModelState.Clear();

            if (id != yachts.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(yachts);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!YachtsExists(yachts.Id))
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
            ViewData["ImageId"] = new SelectList(_context.Image, "Id", "link", yachts.ImageId);
            ViewData["OwnerId"] = new SelectList(_context.Users, "Id", "Id", yachts.OwnerId);
            return View(yachts);
        }

        // GET: Yachts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var yachts = await _context.Yachts
                .Include(y => y.Image)
                .Include(y => y.Owner)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (yachts == null)
            {
                return NotFound();
            }

            return View(yachts);
        }

        // POST: Yachts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Usuń wszystkie powiązane rejsy
            
           // var yachts = await _context.Yachts.FindAsync(id);
            // Pobierz jacht wraz z powiązanymi rejsami
            var yacht = await _context.Yachts
                .Include(y => y.Cruises) // Załaduj powiązane rejsy
                .FirstOrDefaultAsync(y => y.Id == id);

            if (yacht != null)
            {
                _context.Yachts.Remove(yacht);
            }
            if (yacht.Cruises != null) 
            {
                _context.Cruises.RemoveRange(yacht.Cruises);
            }   
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool YachtsExists(int id)
        {
            return _context.Yachts.Any(e => e.Id == id);
        }
    }
}
