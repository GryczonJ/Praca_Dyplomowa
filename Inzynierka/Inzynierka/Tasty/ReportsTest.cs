
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Inzynierka.Controllers;
using Inzynierka.Data.Tables;
using Inzynierka.Data;
using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Inzynierka.Controllers;
using Inzynierka.Data;
using Inzynierka.Models;
using System;

using RedirectResult = Microsoft.AspNetCore.Mvc.RedirectResult;
using ControllerContext = Microsoft.AspNetCore.Mvc.ControllerContext;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Security.Claims;
using System.Net.Http;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Identity;

namespace Tasty
{
    [TestClass]
    public class ReportsTest
    {
        private AhoyDbContext _context;
        private ReportsController _controller;
        private DbContextOptions<AhoyDbContext> options; // Przenie� deklaracj� tutaj

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AhoyDbContext>()
           .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unikalna baza dla ka�dego testu
           .Options;

            _context = new AhoyDbContext(options);
            _controller = new ReportsController(_context)
            {
                TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>())
            };
        }

        /* [TestCleanup] // Metoda czyszcz�ca (uruchamiana po ka�dym te�cie)
         public void TearDown()
         {
             using (var context = new AhoyDbContext(options)) // U�yj opcji z Setup
             {
                 context.Database.EnsureDeleted(); // Usu� baz� danych w pami�ci
             }
         }*/
        /*    
         *    Utworzenie u�ytkownika i dodanie mu roli "Banned".
         *    Wywo�anie metody UnBanned.
         *    Sprawdzenie, czy u�ytkownik zosta� odbanowany(banned == false).
         *    Sprawdzenie, czy rola "Banned" zosta�a usuni�ta.
         *    Sprawdzenie, czy u�ytkownik dosta� rol� "User".
         *    Sprawdzenie, czy TempData["Message"] zawiera poprawny komunikat.
         */

        [TestMethod]
        public async Task UnBanned_ShouldUnbanUser_AndAssignUserRole()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var bannedRoleId = Guid.NewGuid();
            var userRoleId = Guid.NewGuid();

            // Dodanie r�l do bazy
            var bannedRole = new Roles { Id = bannedRoleId, Name = "Banned" };
            var userRole = new Roles{ Id = userRoleId, Name = "User" };
            _context.Roles.AddRange(bannedRole, userRole);
            await _context.SaveChangesAsync();

            // Dodanie zbanowanego u�ytkownika
            var user = new Users
            {
                Id = userId,
                firstName = "Jan",
                lastName = "Kowalski",
                banned = true,
                reasonBan = "Spam"
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Przypisanie roli "Banned"
            var userBannedRole = new IdentityUserRole<Guid>
            {
                UserId = userId,
                RoleId = bannedRoleId
            };
            _context.UserRoles.Add(userBannedRole);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.UnBanned(userId) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Reports", result.ActionName); // Sprawdzamy, czy przekierowuje na Reports

            // Sprawdzenie, czy u�ytkownik nie jest ju� zbanowany
            var updatedUser = await _context.Users.FindAsync(userId);
            Assert.IsNotNull(updatedUser);
            Assert.IsFalse(updatedUser.banned);
            Assert.AreEqual("", updatedUser.reasonBan);

            // Sprawdzenie, czy u�ytkownik nie ma ju� roli "Banned"
            var stillBanned = await _context.UserRoles
                .AnyAsync(ur => ur.UserId == userId && ur.RoleId == bannedRoleId);
            Assert.IsFalse(stillBanned);

            // Sprawdzenie, czy u�ytkownik ma teraz rol� "User"
            var hasUserRole = await _context.UserRoles
                .AnyAsync(ur => ur.UserId == userId && ur.RoleId == userRoleId);
            Assert.IsTrue(hasUserRole);

            // Sprawdzenie, czy `TempData["Message"]` zawiera poprawny komunikat
            Assert.AreEqual("U�ytkownik Jan Kowalski zosta� odbanowany.", _controller.TempData["Message"]);
        }

        [TestMethod]
        public async Task Banned_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            // Act
            var result = await _controller.Banned(Guid.NewGuid(), "Spam");

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task Banned_ShouldRedirect_WhenUserTriesToBanHimselfOrModerator()
        {

            var options = new DbContextOptionsBuilder<AhoyDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            // Tworzenie DbContext PRZED kontrolerem
            _context = new AhoyDbContext(options);

            // Tworzenie kontrolera PRZED ustawieniem ControllerContext
            _controller = new ReportsController(_context);

            // Tworzymy HttpContext i przypisujemy do kontrolera
            var httpContext = new DefaultHttpContext();
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Arrange
            var userId = Guid.NewGuid();
            var moderatorRoleId = Guid.NewGuid();

            var user = new Users { Id = userId, firstName = "Jan", lastName = "Kowalski" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Tworzymy lub za�adowujemy rol� "Moderacja"
            var moderatorRole = await _context.Roles
                .FirstOrDefaultAsync(r => r.Name == "Moderacja");

            // Je�li rola "Moderacja" nie istnieje, tworzymy j�
            if (moderatorRole == null)
            {
                moderatorRole = new Roles { Id = moderatorRoleId, Name = "Moderacja" };
                _context.Roles.Add(moderatorRole);
            }

            await _context.SaveChangesAsync();

            _context.UserRoles.Add(new IdentityUserRole<Guid> { UserId = userId, RoleId = moderatorRoleId });
            await _context.SaveChangesAsync();

            // Tworzymy ClaimsPrincipal i przypisujemy do HttpContext.User
            var claimsIdentity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            }, "test");

            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            _controller.ControllerContext.HttpContext.User = claimsPrincipal;

            // Act
            var result = await _controller.Banned(userId, "Spam") as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("BannedUsers", result.ActionName);
            Assert.AreEqual("Reports", result.ControllerName);
        }

        [TestMethod]
        public async Task Banned_ShouldBanUser_AndAssignBannedRole()
        {
            // Arrange
            var userId = Guid.NewGuid();  // Id u�ytkownika, kt�rego chcemy zbanowa�
            var loggedInUserId = Guid.NewGuid();  // Id u�ytkownika, kt�ry wykonuje akcj� (np. moderator)
            var bannedRoleId = Guid.NewGuid();

            var user = new Users { Id = userId, firstName = "Jan", lastName = "Kowalski", banned = false };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var loggedInUser = new Users { Id = loggedInUserId, firstName = "Anna", lastName = "Nowak" };
            _context.Users.Add(loggedInUser);
            await _context.SaveChangesAsync();

            var bannedRole = new Roles { Id = bannedRoleId, Name = "Banned" };
            _context.Roles.Add(bannedRole);
            await _context.SaveChangesAsync();

            // Przygotowanie HttpContext dla zalogowanego u�ytkownika
            var httpContext = new DefaultHttpContext();
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Ustawienie zalogowanego u�ytkownika w HttpContext.User
            var claimsIdentity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, loggedInUserId.ToString())
            }, "test");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            httpContext.User = claimsPrincipal;  // Zalogowany u�ytkownik

            httpContext.Items["UserId"] = loggedInUserId; // Symulacja u�ytkownika wykonuj�cego akcj�


            // Act
            var result = await _controller.Banned(userId, "Spam") as RedirectToActionResult;
            //var result = await _controller.Banned(bannedRoleId, "Spam") as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("BannedUsers", result.ActionName);

            var updatedUser = await _context.Users.FindAsync(userId);
            Assert.IsNotNull(updatedUser);
            Assert.IsTrue(updatedUser.banned);
            Assert.AreEqual("Spam", updatedUser.reasonBan);

            var hasBannedRole = await _context.UserRoles.AnyAsync(ur => ur.UserId == userId && ur.RoleId == bannedRoleId);
            Assert.IsTrue(hasBannedRole);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Dispose();
        }
    }
}
