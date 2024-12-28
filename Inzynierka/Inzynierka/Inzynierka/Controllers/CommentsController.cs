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
    public class CommentsController : Controller
    {
        private readonly AhoyDbContext _context;

        public CommentsController(AhoyDbContext context)
        {
            _context = context;
        }

        // GET: Comments
        public async Task<IActionResult> Index()
        {
            var ahoyDbContext = _context.Comments.Include(c => c.Charter).Include(c => c.Creator).Include(c => c.Cruises).Include(c => c.Profile).Include(c => c.Yachts);
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Message,CreateDate,Rating,CreatorId,ProfileId,CharterId,CruisesId,YachtsId")] Comments comments)
        {
            if (ModelState.IsValid)
            {
                _context.Add(comments);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CharterId"] = new SelectList(_context.Charters, "Id", "currency", comments.CharterId);
            ViewData["CreatorId"] = new SelectList(_context.Users, "Id", "Id", comments.CreatorId);
            ViewData["CruisesId"] = new SelectList(_context.Cruises, "Id", "currency", comments.CruisesId);
            ViewData["ProfileId"] = new SelectList(_context.Users, "Id", "Id", comments.ProfileId);
            ViewData["YachtsId"] = new SelectList(_context.Yachts, "Id", "name", comments.YachtsId);
            return View(comments);
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
            if (id != comments.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(comments);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommentsExists(comments.Id))
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
