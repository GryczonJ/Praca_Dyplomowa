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

namespace Inzynierka.Controllers
{
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
                .FirstOrDefaultAsync(m => m.Id == id);
            if (charters == null)
            {
                return NotFound();
            }

            return View(charters);
        }

        // GET: Charters/Create
        public IActionResult Create()
        {
            ViewData["OwnerId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["YachtId"] = new SelectList(_context.Yachts, "Id", "name");
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
                return RedirectToAction("Login", "Account");
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
            ViewData["OwnerId"] = new SelectList(_context.Users, "Id", "Id", charters.OwnerId);
            ViewData["YachtId"] = new SelectList(_context.Yachts, "Id", "name", charters.YachtId);
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
