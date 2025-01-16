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
    public class ReportsController : Controller
    {
        private readonly AhoyDbContext _context;

        public ReportsController(AhoyDbContext context)
        {
            _context = context;
        }
        private Guid? GetLoggedInUserId()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(userIdString, out Guid userId) ? userId : null;
        }

        // GET: Reports
        public async Task<IActionResult> Index()
        {
            var ahoyDbContext = _context.Reports.Include(r => r.DocumentVerification).Include(r => r.Moderator).Include(r => r.SuspectCharter).Include(r => r.SuspectComment).Include(r => r.SuspectCruise).Include(r => r.SuspectRole).Include(r => r.SuspectUser).Include(r => r.SuspectYacht).Include(r => r.SuspectYachtSale);
            return View(await ahoyDbContext.ToListAsync());
        }

        // GET: Reports/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reports = await _context.Reports
                .Include(r => r.DocumentVerification)
                .Include(r => r.Moderator)
                .Include(r => r.SuspectCharter)
                .Include(r => r.SuspectComment)
                .Include(r => r.SuspectCruise)
                .Include(r => r.SuspectRole)
                .Include(r => r.SuspectUser)
                .Include(r => r.SuspectYacht)
                .Include(r => r.SuspectYachtSale)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reports == null)
            {
                return NotFound();
            }

            return View(reports);
        }

        // GET: Reports/Create
        public IActionResult Create()
        {
            ViewData["DocumentVerificationId"] = new SelectList(_context.Roles, "Id", "Id");
            ViewData["ModeratorId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["SuspectCharterId"] = new SelectList(_context.Charters, "Id", "currency");
            ViewData["SuspectCommentId"] = new SelectList(_context.Comments, "Id", "Message");
            ViewData["SuspectCruiseId"] = new SelectList(_context.Cruises, "Id", "currency");
            ViewData["SuspectRoleId"] = new SelectList(_context.Roles, "Id", "Id");
            ViewData["SuspectUserId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["SuspectYachtId"] = new SelectList(_context.Yachts, "Id", "name");
            ViewData["SuspectYachtSaleId"] = new SelectList(_context.YachtSale, "Id", "currency");
            return View();
        }

        // POST: Reports/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,status,date,message,note,ModeratorId,SuspectUserId,SuspectCruiseId,SuspectYachtId,DocumentVerificationId,SuspectYachtSaleId,SuspectCharterId,SuspectCommentId,SuspectRoleId")] Reports reports)
        {
            reports.date = DateTime.Now;
            reports.CreatorId = GetLoggedInUserId();
            if (ModelState.IsValid)
            {
                _context.Add(reports);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DocumentVerificationId"] = new SelectList(_context.Roles, "Id", "Id", reports.DocumentVerificationId);
            ViewData["ModeratorId"] = new SelectList(_context.Users, "Id", "Id", reports.ModeratorId);
            ViewData["SuspectCharterId"] = new SelectList(_context.Charters, "Id", "currency", reports.SuspectCharterId);
            ViewData["SuspectCommentId"] = new SelectList(_context.Comments, "Id", "Message", reports.SuspectCommentId);
            ViewData["SuspectCruiseId"] = new SelectList(_context.Cruises, "Id", "currency", reports.SuspectCruiseId);
            ViewData["SuspectRoleId"] = new SelectList(_context.Roles, "Id", "Id", reports.SuspectRoleId);
            ViewData["SuspectUserId"] = new SelectList(_context.Users, "Id", "Id", reports.SuspectUserId);
            ViewData["SuspectYachtId"] = new SelectList(_context.Yachts, "Id", "name", reports.SuspectYachtId);
            ViewData["SuspectYachtSaleId"] = new SelectList(_context.YachtSale, "Id", "currency", reports.SuspectYachtSaleId);
            return View(reports);
        }

        // GET: Reports/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reports = await _context.Reports.FindAsync(id);
            if (reports == null)
            {
                return NotFound();
            }
            ViewData["DocumentVerificationId"] = new SelectList(_context.Roles, "Id", "Id", reports.DocumentVerificationId);
            ViewData["ModeratorId"] = new SelectList(_context.Users, "Id", "Id", reports.ModeratorId);
            ViewData["SuspectCharterId"] = new SelectList(_context.Charters, "Id", "currency", reports.SuspectCharterId);
            ViewData["SuspectCommentId"] = new SelectList(_context.Comments, "Id", "Message", reports.SuspectCommentId);
            ViewData["SuspectCruiseId"] = new SelectList(_context.Cruises, "Id", "currency", reports.SuspectCruiseId);
            ViewData["SuspectRoleId"] = new SelectList(_context.Roles, "Id", "Id", reports.SuspectRoleId);
            ViewData["SuspectUserId"] = new SelectList(_context.Users, "Id", "Id", reports.SuspectUserId);
            ViewData["SuspectYachtId"] = new SelectList(_context.Yachts, "Id", "name", reports.SuspectYachtId);
            ViewData["SuspectYachtSaleId"] = new SelectList(_context.YachtSale, "Id", "currency", reports.SuspectYachtSaleId);
            return View(reports);
        }

        // POST: Reports/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,status,date,message,note,ModeratorId,SuspectUserId,SuspectCruiseId,SuspectYachtId,DocumentVerificationId,SuspectYachtSaleId,SuspectCharterId,SuspectCommentId,SuspectRoleId")] Reports reports)
        {
            if (id != reports.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reports);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReportsExists(reports.Id))
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
            ViewData["DocumentVerificationId"] = new SelectList(_context.Roles, "Id", "Id", reports.DocumentVerificationId);
            ViewData["ModeratorId"] = new SelectList(_context.Users, "Id", "Id", reports.ModeratorId);
            ViewData["SuspectCharterId"] = new SelectList(_context.Charters, "Id", "currency", reports.SuspectCharterId);
            ViewData["SuspectCommentId"] = new SelectList(_context.Comments, "Id", "Message", reports.SuspectCommentId);
            ViewData["SuspectCruiseId"] = new SelectList(_context.Cruises, "Id", "currency", reports.SuspectCruiseId);
            ViewData["SuspectRoleId"] = new SelectList(_context.Roles, "Id", "Id", reports.SuspectRoleId);
            ViewData["SuspectUserId"] = new SelectList(_context.Users, "Id", "Id", reports.SuspectUserId);
            ViewData["SuspectYachtId"] = new SelectList(_context.Yachts, "Id", "name", reports.SuspectYachtId);
            ViewData["SuspectYachtSaleId"] = new SelectList(_context.YachtSale, "Id", "currency", reports.SuspectYachtSaleId);
            return View(reports);
        }

        // GET: Reports/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reports = await _context.Reports
                .Include(r => r.DocumentVerification)
                .Include(r => r.Moderator)
                .Include(r => r.SuspectCharter)
                .Include(r => r.SuspectComment)
                .Include(r => r.SuspectCruise)
                .Include(r => r.SuspectRole)
                .Include(r => r.SuspectUser)
                .Include(r => r.SuspectYacht)
                .Include(r => r.SuspectYachtSale)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reports == null)
            {
                return NotFound();
            }

            return View(reports);
        }

        // POST: Reports/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reports = await _context.Reports.FindAsync(id);
            if (reports != null)
            {
                _context.Reports.Remove(reports);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReportsExists(int id)
        {
            return _context.Reports.Any(e => e.Id == id);
        }
    }
}
