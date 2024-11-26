using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Inzynierka.Data;
using Inzynierka.Data.Tables;

namespace Inzynierka.Controllers
{
    public class CruisesController : Controller
    {
        private readonly AhoyDbContext _context;

        public CruisesController(AhoyDbContext context)
        {
            _context = context;
        }

        // GET: Cruises
        public async Task<IActionResult> Index()
        {
            var ahoyDbContext = _context.Cruises.Include(c => c.Capitan).Include(c => c.Yacht);
            return View(await ahoyDbContext.ToListAsync());
        }

        // GET: Cruises/Details/5
        public async Task<IActionResult> Details(int? id)
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

        // GET: Cruises/Create
        public IActionResult Create()
        {
            ViewData["CapitanId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["YachtId"] = new SelectList(_context.Yachts, "Id", "name");
            return View();
        }

        // POST: Cruises/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,name,description,destination,start_date,end_date,price,currency,status,maxParticipants,YachtId,CapitanId")] Cruises cruises)
        {
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
            ViewData["CapitanId"] = new SelectList(_context.Users, "Id", "Id", cruises.CapitanId);
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
