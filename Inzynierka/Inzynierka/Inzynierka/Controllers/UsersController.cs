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

namespace Inzynierka.Controllers
{
    [Authorize(Roles = "User,Moderacja,Kapitan")]
    public class UsersController : Controller
    {
        private readonly AhoyDbContext _context;

        private Guid? GetLoggedInUserId()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(userIdString, out Guid userId) ? userId : null;
        }


        public UsersController(AhoyDbContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            var ahoyDbContext = _context.Users.Include(u => u.Photos);
            return View(await ahoyDbContext.ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var users = await _context.Users
                .Include(u => u.Photos)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (users == null)
            {
                return NotFound();
            }

            return View(users);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            ViewData["PhotosId"] = new SelectList(_context.Image, "Id", "link");
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("age,surName,aboutMe,firstName,lastName,banned,Public,PhotosId,Id,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount")] Users users)
        {
            if (ModelState.IsValid)
            {
                users.Id = Guid.NewGuid();
                _context.Add(users);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PhotosId"] = new SelectList(_context.Image, "Id", "link", users.PhotosId);
            return View(users);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            Guid? getLoggedInUserId = GetLoggedInUserId();
            //ViewData["ActivePage"] = "Edit";
            if (id == null && getLoggedInUserId == null)
            {
                return NotFound();
            }
            if (id == null)
            {
                id = getLoggedInUserId;
            }
            var users = await _context.Users.FindAsync(id);
            if (users == null)
            {
                return NotFound();
            }
            ViewData["PhotosId"] = new SelectList(_context.Image, "Id", "link", users.PhotosId);
            return View(users);
            /*  return RedirectToAction("Index", "Home");*/
        }
        /*public async Task<IActionResult> Edit()
        {
            var id = GetLoggedInUserId;

            ViewData["ActivePage"] = "Edit";


            var users = await _context.Users.FindAsync(id);
            if (users == null)
            {
                return NotFound();
            }
            ViewData["PhotosId"] = new SelectList(_context.Image, "Id", "link", users.PhotosId);
            return View(users);
        }*/

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("age,surName,aboutMe,firstName,lastName,banned,Public,PhotosId,Id,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount")] Users users)
        {
            ViewData["ActivePage"] = "Edit"; 
            if (id != users.Id)
            {
                return NotFound();
            }
            //ModelState.Clear();
            if (ModelState.IsValid)
            {
                try
                {
                    var existingUser = await _context.Users.FindAsync(id);
                    if (existingUser == null)
                    {
                        return NotFound();
                    }
                    // Ustawienie wartości, jeśli różnią się od istniejących wartości
                    existingUser.age = users.age;
                    existingUser.firstName = users.firstName ?? existingUser.firstName;
                    //existingUser.lastName = users.lastName ?? existingUser.lastName;
                    existingUser.Email = users.Email ?? existingUser.Email;
                    existingUser.PhoneNumber = users.PhoneNumber ?? existingUser.PhoneNumber;
                    existingUser.aboutMe = users.aboutMe ?? existingUser.aboutMe;
                    existingUser.PhotosId = users.PhotosId ?? existingUser.PhotosId;
                    _context.Update(existingUser);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsersExists(users.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", "Home");
                /*return RedirectToAction(nameof(Index));*/
            }
            else
            {
                // Przechwytywanie błędów walidacji
                var validationErrors = new List<string>();

                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        validationErrors.Add(error.ErrorMessage);
                    }
                }
            }

                ViewData["PhotosId"] = new SelectList(_context.Image, "Id", "link", users.PhotosId);
            /*return View(users);*/
            return RedirectToAction("Index", "Home");
        }


        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var users = await _context.Users
                .Include(u => u.Photos)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (users == null)
            {
                return NotFound();
            }

            return View(users);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var users = await _context.Users.FindAsync(id);
            if (users != null)
            {
                _context.Users.Remove(users);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsersExists(Guid id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
