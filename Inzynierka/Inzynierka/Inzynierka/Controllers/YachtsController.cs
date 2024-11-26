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
    public class YachtsController : Controller
    {
        private readonly AhoyDbContext _context;

        public YachtsController(AhoyDbContext context)
        {
            _context = context;
        }

        // GET: Yachts
        public async Task<IActionResult> Index()
        {
            var ahoyDbContext = _context.Yachts.Include(y => y.Image).Include(y => y.Owner);
            return View(await ahoyDbContext.ToListAsync());
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
        public async Task<IActionResult> Create([Bind("Id,name,description,type,manufacturer,model,year,length,width,crew,cabins,beds,toilets,showers,location,capacity,OwnerId,ImageId")] Yachts yachts)
        {
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,name,description,type,manufacturer,model,year,length,width,crew,cabins,beds,toilets,showers,location,capacity,OwnerId,ImageId")] Yachts yachts)
        {
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
            var yachts = await _context.Yachts.FindAsync(id);
            if (yachts != null)
            {
                _context.Yachts.Remove(yachts);
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
