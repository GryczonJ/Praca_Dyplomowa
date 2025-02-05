
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
    public class RolesTest
    {
        private DbContextOptions<AhoyDbContext> options; // Przenieœ deklaracjê tutaj
        private AhoyDbContext _context;
        private RolesController _controller;
        [TestInitialize] // Inicjalizacja przed ka¿dym testem
        public void Setup()
        {
            options = new DbContextOptionsBuilder<AhoyDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _context = new AhoyDbContext(options); // Utwórz instancjê kontekstu

            //_controller = new RolesController(_context); // Utwórz kontroler
            _controller = new RolesController(_context)
            {
                TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>())
            };
        }

        [TestMethod]
        public async Task ChangeRole_ShouldReturnBadRequest_WhenUserIdIsEmpty()
        {
            // Act
            var result = await _controller.ChangeRole(Guid.Empty, Guid.NewGuid());

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual("Nieprawid³owe dane wejœciowe.", badRequestResult.Value);
        }

        [TestMethod]
        public async Task ChangeRole_ShouldReturnUnauthorized_WhenNotLoggedIn()
        {
            // Ustawienie HttpContext bez u¿ytkownika
            var httpContext = new DefaultHttpContext();
            _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

            // Ustawienie braku zalogowanego u¿ytkownika
            _controller.ControllerContext.HttpContext.User = null;

            // Act
            var result = await _controller.ChangeRole(Guid.NewGuid(), Guid.NewGuid());

            // Assert
            var unauthorizedResult = result as UnauthorizedObjectResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual("Brak dostêpu.", unauthorizedResult.Value);
        }

        [TestMethod]
        public async Task ChangeRole_ShouldReturnNotFound_WhenModeratorRoleNotFound()
        {
            // Ustawienie HttpContext bez u¿ytkownika
            var httpContext = new DefaultHttpContext();
            _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

            // Arrange: Utworzenie u¿ytkownika bez roli moderatora
            var userId = Guid.NewGuid();
            var roleId = Guid.NewGuid();

            _context.Users.Add(new Users { Id = userId, firstName = "Test", lastName = "User" });
            // Ustawienie zalogowanego u¿ytkownika w HttpContext.User
            var claimsIdentity = new ClaimsIdentity(new[] {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            }, "test");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            httpContext.User = claimsPrincipal; // Zalogowany u¿ytkownik

            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.ChangeRole(userId, roleId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual("Rola 'Moderator' nie zosta³a znaleziona.", notFoundResult.Value);
        }

        [TestMethod]
        public async Task ChangeRole_ShouldReturnForbid_WhenModeratorTriesToRemoveModeratorRole()
        {
            // Arrange: Dodanie roli "Moderator" do u¿ytkownika
            var moderatorRoleId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var loggedInUserId = Guid.NewGuid();

            _context.Users.Add(new Users { Id = userId, firstName = "Test", lastName = "User" });
            _context.Users.Add(new Users { Id = loggedInUserId, firstName = "Moderator", lastName = "User" });
            _context.Roles.Add(new Roles { Id = moderatorRoleId, Name = "Moderacja" });
            _context.UserRoles.Add(new IdentityUserRole<Guid> { UserId = loggedInUserId, RoleId = moderatorRoleId });
            var initialRoleId = Guid.NewGuid();
            _context.Roles.Add(new Roles { Id = initialRoleId, Name = "InitialRole" });
            _context.UserRoles.Add(new IdentityUserRole<Guid> { UserId = userId, RoleId = initialRoleId });
            _context.SaveChanges();

            // Przygotowanie HttpContext dla u¿ytkownika moderatora
            var httpContext = new DefaultHttpContext();
            var claimsIdentity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, loggedInUserId.ToString())
            }, "test");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            httpContext.User = claimsPrincipal; // Ustawienie poprawnego u¿ytkownika w HttpContext

            _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

            // Act
            var result = await _controller.ChangeRole(userId, moderatorRoleId);

            // Assert
            var forbidResult = result as ForbidResult;
            Assert.IsNotNull(forbidResult);
        }

        [TestMethod]
        public async Task ChangeRole_ShouldAssignRoleSuccessfully()
        {
            // Arrange: Utworzenie u¿ytkownika i roli
            var userId = Guid.NewGuid();
            var roleId = Guid.NewGuid();
            var moderatorRoleId = Guid.NewGuid();
            var loggedInUserId = Guid.NewGuid();

            _context.Users.Add(new Users { Id = userId, firstName = "Test", lastName = "User" });
            _context.Users.Add(new Users { Id = loggedInUserId, firstName = "LoggedIn", lastName = "User" }); // Dodajemy zalogowanego u¿ytkownika
                                                                                                              // Dodanie roli docelowej
            _context.Roles.Add(new Roles { Id = roleId, Name = "UserRole" });

            // ?? Kluczowa poprawka: dodanie roli "Moderacja"
            _context.Roles.Add(new Roles { Id = moderatorRoleId, Name = "Moderacja" });
            await _context.SaveChangesAsync();

            _context.UserRoles.Add(new IdentityUserRole<Guid> { UserId = userId, RoleId = moderatorRoleId });
            await _context.SaveChangesAsync();

            // Przygotowanie HttpContext dla u¿ytkownika moderatora
            var httpContext = new DefaultHttpContext();
            var claimsIdentity = new ClaimsIdentity(new[]
            {
                 new Claim(ClaimTypes.NameIdentifier, loggedInUserId.ToString())
            }, "test");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            httpContext.User = claimsPrincipal; // Ustawienie poprawnego u¿ytkownika w HttpContext

            // ?? Najwa¿niejsza poprawka: Przypisanie HttpContext do kontrolera!
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            // Act
            var result = await _controller.ChangeRole(userId, roleId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var updatedUserRole = await _context.UserRoles.FirstOrDefaultAsync(ur => ur.UserId == userId);

            Assert.IsNotNull(updatedUserRole);
            Assert.AreEqual(roleId, updatedUserRole.RoleId);
        }

        [TestCleanup] // Metoda czyszcz¹ca (uruchamiana po ka¿dym teœcie)
        public void TearDown()
        {
            using (var context = new AhoyDbContext(options)) // U¿yj opcji z Setup
            {
                context.Database.EnsureDeleted(); // Usuñ bazê danych w pamiêci
            }
        }

    }
}
