using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Inzynierka.Data;
using Inzynierka.Data.Tables;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Inzynierka.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace Inzynierka.Controllers
{
    [Authorize(Roles = "Moderacja")]
    public class RolesController : Controller
    {
        private readonly AhoyDbContext _context;

        public RolesController(AhoyDbContext context)
        {
            _context = context;
        }

        // GET: Roles
        public async Task<IActionResult> Index()
        {
            var roles = await _context.Roles.ToListAsync();
            var userRoles = await (
           from r in _context.Roles
           join ur in _context.UserRoles on r.Id equals ur.RoleId
           join u in _context.Users on ur.UserId equals u.Id
           select new UserRoleViewModel
           {
               UserId = u.Id,
               UserName = u.UserName,
               RoleId = r.Id,
               RoleName = r.Name
           }).ToListAsync();
            var userRolesViewModel = new UserRolesViewModel
            {
                Roles = roles,  // Lista wszystkich ról
                UserRoles = userRoles     // Lista użytkowników z przypisanymi rolami
            };
            // Przekazujemy dane do widoku
            return View(userRolesViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeRole(Guid userId, Guid roleId)
        {
            if (userId == Guid.Empty || roleId == Guid.Empty)
            {
                return BadRequest("Nieprawidłowe dane wejściowe.");
            }

            var userRole = await _context.UserRoles.FirstOrDefaultAsync(ur => ur.UserId == userId);

            if (userRole == null)
            {
                return NotFound("Użytkownik lub przypisana rola nie istnieje.");
            }

            if (userRole != null)
            {
                // Usuń istniejącą relację
                _context.UserRoles.Remove(userRole);
                await _context.SaveChangesAsync();
            }

            var newUserRole = new IdentityUserRole<Guid>
            {
                UserId = userId,
                RoleId = roleId
            };

            _context.UserRoles.Add(newUserRole);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // GET: Roles/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var roles = await _context.Roles
                .FirstOrDefaultAsync(m => m.Id == id);
            if (roles == null)
            {
                return NotFound();
            }

            return View(roles);
        }

        // GET: Roles/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Roles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("certificates,Id,Name,NormalizedName,ConcurrencyStamp")] Roles roles)
        {
            if (ModelState.IsValid)
            {
                roles.Id = Guid.NewGuid();
                _context.Add(roles);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(roles);
        }

        // GET: Roles/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var roles = await _context.Roles.FindAsync(id);
            if (roles == null)
            {
                return NotFound();
            }
            return View(roles);
        }

        // POST: Roles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("certificates,Id,Name,NormalizedName,ConcurrencyStamp")] Roles roles)
        {
            if (id != roles.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(roles);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RolesExists(roles.Id))
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
            return View(roles);
        }

        // GET: Roles/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var roles = await _context.Roles
                .FirstOrDefaultAsync(m => m.Id == id);
            if (roles == null)
            {
                return NotFound();
            }

            return View(roles);
        }

        // POST: Roles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var roles = await _context.Roles.FindAsync(id);
            if (roles != null)
            {
                _context.Roles.Remove(roles);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RolesExists(Guid id)
        {
            return _context.Roles.Any(e => e.Id == id);
        }
    }
}
