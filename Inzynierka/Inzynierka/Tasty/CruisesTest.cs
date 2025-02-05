using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

using System.Collections.Generic;
using System.Linq;
using Inzynierka.Controllers;
using Inzynierka.Data.Tables;
using Inzynierka.Data;
using Azure.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Inzynierka.Models;
using System;
using System.Web.Mvc;
using RedirectResult = Microsoft.AspNetCore.Mvc.RedirectResult;
using ControllerContext = Microsoft.AspNetCore.Mvc.ControllerContext;
using ITempDataProvider = Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataProvider;
using TempDataDictionary = Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionary;
using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Reflection;


namespace Tasty
{
    [TestClass]
    public class CruisesTest
    {
        private AhoyDbContext _context;
        private CruisesController _controller;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AhoyDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unikalna baza danych dla każdego testu
                .Options;

            _context = new AhoyDbContext(options);
            /*  var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                      new Claim(ClaimTypes.NameIdentifier, "user-123"), // Możesz tu wstawić rzeczywisty identyfikator użytkownika
                }, "mock"));*/

            /*  var controllerContext = new ControllerContext
              {
                  HttpContext = new DefaultHttpContext { User = user }
              };*/
            _controller = new CruisesController(_context)
            {
                /*ControllerContext = controllerContext,*/
                TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>())
            };
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Dispose();
        }

        [TestMethod]
        public async Task JoinCruise_ShouldRedirectToLogin_WhenUserIsNotLoggedIn()
        {
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(c => c.User).Returns(new ClaimsPrincipal()); // Ustaw pusty ClaimsPrincipal dla niezalogowanego użytkownika
            _controller.ControllerContext = new ControllerContext() { HttpContext = mockHttpContext.Object };
            // Act
            var result = await _controller.JoinCruise(1) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Login", result.ActionName);
            Assert.AreEqual("Account", result.ControllerName);
        }

        [TestMethod]
        public async Task JoinCruise_ShouldReturnError_WhenUserAlreadyParticipantOrPending()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var cruise = new Cruises
            {
                Id = 1,
                name = "Test Cruise",
                currency = "USD",  // Dodane wymagane pola
                description = "Testowy rejs",
                destination = "Bahamy"
            };

            _context.Cruises.Add(cruise);
            _context.CruisesParticipants.Add(new CruisesParticipants { CruisesId = 1, UsersId = userId });
            await _context.SaveChangesAsync();

            /* _controller.SetLoggedInUser(userId);*/
            // Tworzymy zalogowanego użytkownika
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()) // Zapewniamy, że userId jest ustawione w Claims
            }, "mock"));

            // Ustawiamy kontroler z zalogowanym użytkownikiem
            var controllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
            // Upewnij się, że HttpContext.User jest ustawione w kontrolerze
            _controller.ControllerContext = controllerContext;

            // Act
            var result = await _controller.JoinCruise(1) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Details", result.ActionName);
            Assert.AreEqual(1, result.RouteValues["id"]);
            Assert.AreEqual("Jesteś już uczestnikiem lub złożyłeś zgłoszenie.", _controller.TempData["Error"]);
        }

        [TestMethod]
        public async Task JoinCruise_ShouldReturnError_WhenCruiseDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            /* _controller.SetLoggedInUser(userId);*/
            // Tworzymy zalogowanego użytkownika
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()) // Dodajemy Claims z userId
            }, "mock"));

            // Ustawiamy kontroler z zalogowanym użytkownikiem
            var controllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            _controller.ControllerContext = controllerContext;
            // Act
            var result = await _controller.JoinCruise(99) as RedirectToActionResult; // Nieistniejący rejs

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Details", result.ActionName);
            Assert.AreEqual(99, result.RouteValues["id"]);
            Assert.AreEqual("Rejs nie istnieje.", _controller.TempData["Error"]);
        }

        [TestMethod]
        public async Task JoinCruise_ShouldCreateRequest_WhenUserIsEligible()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var cruise = new Cruises
            {
                Id = 1,
                name = "Test Cruise",
                currency = "USD",  // Dodajemy brakujące pole
                description = "Testowy rejs",  // Dodajemy brakujące pole
                destination = "Bahamy"  // Dodajemy brakujące pole
            };
            // Tworzymy kapitana
            var captain = new Users { Id = userId, firstName = "Test User" };
            // Dodajemy kapitana do rejsu
            cruise.Capitan = captain;
            // Dodajemy rejs i kapitana do bazy danych
            _context.Cruises.Add(cruise);
            _context.Cruises.Add(cruise);
            await _context.SaveChangesAsync();

            /* _controller.SetLoggedInUser(userId);*/
            // Tworzymy użytkownika
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()) // Dodajemy Claims z userId
            }, "mock"));

            // Tworzymy kontroler z zalogowanym użytkownikiem
            var controllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            _controller.ControllerContext = controllerContext;
            // Act
            var result = await _controller.JoinCruise(1) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Details", result.ActionName);
            Assert.AreEqual(1, result.RouteValues["id"]);
            Assert.AreEqual("Prośba o dołączenie została wysłana.", _controller.TempData["Success"]);

            var requestExists = await _context.CruiseJoinRequest.AnyAsync(j => j.UserId == userId && j.CruiseId == 1);
            Assert.IsTrue(requestExists);
        }

        /*
         Użytkownik niezalogowany – kiedy nie ma userId lub nie jest poprawnym Guid, 
        użytkownik powinien zostać przekierowany do strony logowania.

        Użytkownik zarejestrowany jako uczestnik rejsu – 
        kiedy użytkownik jest zapisany jako uczestnik rejsu, powinien zostać usunięty z bazy danych, 
        a odpowiedni komunikat o sukcesie wyświetlony.

        Użytkownik niezapisany na rejs – 
        kiedy użytkownik nie jest zapisany jako uczestnik rejsu, 
        powinien zobaczyć komunikat o błędzie.
        */

        [TestMethod]
        public async Task LeaveCruise_ShouldRedirectToLogin_WhenUserIsNotLoggedIn()
        {
            // Arrange
            var controller = new CruisesController(_context);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
            };

            // Act
            var result = await controller.LeaveCruise(1) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Login", result.ActionName);
            Assert.AreEqual("Account", result.ControllerName);
        }

        [TestMethod]
        public async Task LeaveCruise_ShouldRemoveUserFromCruise_WhenUserIsParticipant()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var cruise = new Cruises
            {
                Id = 1,
                name = "Test Cruise",
                currency = "USD",  // Dodajemy brakujące pole
                description = "Testowy rejs",  // Dodajemy brakujące pole
                destination = "Bahamy"  // Dodajemy brakujące pole
            };
            var participation = new CruisesParticipants { UsersId = userId, CruisesId = 1 };

            _context.Cruises.Add(cruise);
            _context.CruisesParticipants.Add(participation);
            await _context.SaveChangesAsync();
            // Tworzymy użytkownika
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()) // Dodajemy Claims z userId
            }, "mock"));

            // Tworzymy kontroler z zalogowanym użytkownikiem
            var controllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            _controller.ControllerContext = controllerContext;
            // Act
            var result = await _controller.LeaveCruise(1) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Details", result.ActionName);
            Assert.AreEqual(1, result.RouteValues["id"]);
            Assert.AreEqual("Zrezygnowałeś z rejsu.", _controller.TempData["Success"]);

            var participantExists = await _context.CruisesParticipants
                .AnyAsync(cp => cp.UsersId == userId && cp.CruisesId == 1);
            Assert.IsFalse(participantExists);
        }

        [TestMethod]
        public async Task LeaveCruise_ShouldReturnError_WhenUserIsNotParticipant()
        {
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(c => c.User).Returns(new ClaimsPrincipal()); // Ustaw pusty ClaimsPrincipal
            _controller.ControllerContext = new ControllerContext() { HttpContext = mockHttpContext.Object };

            // Arrange
            var userId = Guid.NewGuid();
            var cruise = new Cruises
            {
                Id = 1,
                name = "Test Cruise",
                currency = "PLN", // Dodaj wymaganą walutę
                description = "Opis testowego rejsu", // Dodaj opis
                destination = "Testowa destynacja" // Dodaj miejsce docelowe
            };

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()) // Dodajemy Claims z userId
            }, "mock"));

            // Tworzymy kontroler z zalogowanym użytkownikiem
            var controllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
            _controller.ControllerContext = controllerContext;
            _context.Cruises.Add(cruise);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.LeaveCruise(1) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Details", result.ActionName);
            Assert.AreEqual(1, result.RouteValues["id"]);
            Assert.AreEqual("Nie jesteś uczestnikiem tego rejsu.", _controller.TempData["Error"]);
        }

    }

}

