
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

namespace Tasty
{
    [TestClass]
    public class YachtSalesTest
    {
        private DbContextOptions<AhoyDbContext> GetDbContextOptions()
        {
            return new DbContextOptionsBuilder<AhoyDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unikalna baza dla ka¿dego testu
                .Options;
        }

        [TestMethod]
        public async Task Create_ShouldRedirectToIndex_WhenYachtSaleIsCreatedSuccessfully()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AhoyDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            // Ustawienie kontekstu i kontrolera
            using var context = new AhoyDbContext(options);
            var controller = new YachtSalesController(context);

            // Symulacja zalogowanego u¿ytkownika
            var user = new Users { Id = Guid.NewGuid(), banned = false };
            context.Users.Add(user);
            await context.SaveChangesAsync(); // U¿ycie async w SaveChanges

            var claimsIdentity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            }, "test");

            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = claimsPrincipal }
            };
            controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            // Przygotowanie obiektu YachtSale
            var yachtSale = new YachtSale
            {
                price = 1000,
                currency = "USD",
                location = "Test location",
                notes = "Test notes",
                YachtId = 1
            };

            // Symulacja dostêpnoœci jachtu
            var yacht = new Yachts
            {
                Id = 1,
                name = "Test Yacht",
                description = "Test description", // Wymagane
                location = "Test location", // Wymagane
                manufacturer = "Test manufacturer", // Wymagane
                model = "Test model", // Wymagane
                type = "Test type" // Wymagane
            };

            context.Yachts.Add(yacht);
            await context.SaveChangesAsync();

            // Act
            var result = await controller.Create(yachtSale);

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual("Index", redirectResult.ActionName);
            Assert.AreEqual("Jacht zosta³ pomyœlnie wystawiony na sprzeda¿!", controller.TempData["Message"]);
            Assert.AreEqual("success", controller.TempData["AlertType"]);
        }


        //================================================================================================
        /* 
          test Buy_ShouldRedirectToDetails_WhenYachtSaleIsBoughtSuccessfully, 
          który sprawdza, czy proces zakupu przebiega poprawnie i 
          czy odpowiedni komunikat jest ustawiany w TempData.

          test Buy_ShouldRedirectToLogin_WhenUserIsNotLoggedIn, który sprawdza, 
          czy u¿ytkownik niezalogowany jest przekierowywany do strony logowania.

          test Buy_ShouldRedirectToDetails_WhenYachtSaleAlreadySold, 
          który sprawdza, czy jeœli jacht zosta³ ju¿ sprzedany, 
          odpowiedni komunikat o b³êdzie pojawi siê w TempData.
        */
        [TestMethod]
        public async Task Buy_ShouldRedirectToDetails_WhenYachtSaleIsBoughtSuccessfully()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AhoyDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            // Ustawienie kontekstu i kontrolera
            using var context = new AhoyDbContext(options);
            var controller = new YachtSalesController(context);

            // Symulacja zalogowanego u¿ytkownika
            var user = new Users { Id = Guid.NewGuid(), banned = false };
            context.Users.Add(user);
            await context.SaveChangesAsync(); // U¿ycie async w SaveChanges

            var claimsIdentity = new ClaimsIdentity(new[]
            {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
    }, "test");

            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = claimsPrincipal }
            };
            controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            // Symulacja dostêpnoœci jachtu
            var yachtSale = new YachtSale
            {
                price = 1000,
                currency = "USD",
                location = "Test location",
                notes = "Test notes",
                YachtId = 1
            };

            var yacht = new Yachts
            {
                Id = 1,
                name = "Test Yacht",
                description = "Test description",
                location = "Test location",
                manufacturer = "Test manufacturer",
                model = "Test model",
                type = "Test type"
            };

            context.Yachts.Add(yacht);
            await context.SaveChangesAsync();

            // Dodajemy ofertê sprzeda¿y
            context.YachtSale.Add(yachtSale);
            await context.SaveChangesAsync();

            // Act
            var result = await controller.Buy(yachtSale.Id);

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual("Details", redirectResult.ActionName);
            Assert.AreEqual("Zakup zosta³ zainicjowany. Oczekuje na akceptacjê sprzedaj¹cego.", controller.TempData["Message"]);
            Assert.AreEqual("success", controller.TempData["AlertType"]);
        }

        [TestMethod]
        public async Task Buy_ShouldRedirectToLogin_WhenUserIsNotLoggedIn()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AhoyDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using var context = new AhoyDbContext(options);
            var controller = new YachtSalesController(context);
            controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            // Symulacja braku zalogowanego u¿ytkownika
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() // Brak zalogowanego u¿ytkownika
            };
            // Symulacja u¿ytkownika niezalogowanego
            var yachtSale = new YachtSale
            {
                price = 1000,
                currency = "USD",
                location = "Test location",
                notes = "Test notes",
                YachtId = 1
            };

            var yacht = new Yachts
            {
                Id = 1,
                name = "Test Yacht",
                description = "Test description",
                location = "Test location",
                manufacturer = "Test manufacturer",
                model = "Test model",
                type = "Test type"
            };

            context.Yachts.Add(yacht);
            await context.SaveChangesAsync();

            // Dodajemy ofertê sprzeda¿y
            context.YachtSale.Add(yachtSale);
            await context.SaveChangesAsync();

            // Act
            var result = await controller.Buy(yachtSale.Id);

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual("Index", redirectResult.ActionName);
            Assert.AreEqual("Musisz byæ zalogowany, aby dokonaæ zakupu.", controller.TempData["Message"]);
            Assert.AreEqual("warning", controller.TempData["AlertType"]);
        }

        [TestMethod]
        public async Task Buy_ShouldRedirectToDetails_WhenYachtSaleAlreadySold()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AhoyDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using var context = new AhoyDbContext(options);
            var controller = new YachtSalesController(context);

            // Symulacja zalogowanego u¿ytkownika
            var user = new Users { Id = Guid.NewGuid(), banned = false };
            context.Users.Add(user);
            await context.SaveChangesAsync(); // U¿ycie async w SaveChanges

            var claimsIdentity = new ClaimsIdentity(new[]
            {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
    }, "test");

            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = claimsPrincipal }
            };
            controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            // Symulacja dostêpnoœci jachtu
            var yachtSale = new YachtSale
            {
                price = 1000,
                currency = "USD",
                location = "Test location",
                notes = "Test notes",
                YachtId = 1,
                BuyerUserId = Guid.NewGuid() // Jacht ju¿ sprzedany
            };

            var yacht = new Yachts
            {
                Id = 1,
                name = "Test Yacht",
                description = "Test description",
                location = "Test location",
                manufacturer = "Test manufacturer",
                model = "Test model",
                type = "Test type"
            };

            context.Yachts.Add(yacht);
            await context.SaveChangesAsync();

            // Dodajemy ofertê sprzeda¿y
            context.YachtSale.Add(yachtSale);
            await context.SaveChangesAsync();

            // Act
            var result = await controller.Buy(yachtSale.Id);

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual("Details", redirectResult.ActionName);
            Assert.AreEqual("Ten jacht zosta³ ju¿ sprzedany.", controller.TempData["Message"]);
            Assert.AreEqual("danger", controller.TempData["AlertType"]);
        }
        /*
         * Jeœli transakcja nie istnieje (NotFound).
         * Jeœli transakcja nie jest w stanie oczekiwania (status != Pending).
         * Jeœli transakcja jest zaakceptowana poprawnie (zmiana w³aœciciela i statusu).
        */
        [TestMethod]
        public async Task Accept_ShouldReturnNotFound_WhenYachtSaleDoesNotExist()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AhoyDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using var context = new AhoyDbContext(options);
            var controller = new YachtSalesController(context);

            // Act
            var result = await controller.Accept(999); // ID, którego nie ma w bazie danych

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Accept_ShouldRedirectWithWarning_WhenTransactionIsNotPending()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AhoyDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using var context = new AhoyDbContext(options);
            var controller = new YachtSalesController(context);
            controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            var yachtSale = new YachtSale
            {
                Id = 1,
                status = TransactionStatus.Rejected,  // Status inny ni¿ Pending
                currency = "USD",  // Dodajemy brakuj¹c¹ wartoœæ
                location = "Test Location",  // Dodajemy brakuj¹c¹ wartoœæ
                notes = "Test notes"  // Dodajemy brakuj¹c¹ wartoœæ
            };
            context.YachtSale.Add(yachtSale);
            await context.SaveChangesAsync();

            // Act
            var result = await controller.Accept(1);

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual("Details", redirectResult.ActionName);
            Assert.AreEqual("Ta transakcja nie jest ju¿ w stanie oczekiwania.", controller.TempData["Message"]);
            Assert.AreEqual("warning", controller.TempData["AlertType"]);
        }

        [TestMethod]
        public async Task Accept_ShouldUpdateStatusAndOwner_WhenTransactionIsPending()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AhoyDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using var context = new AhoyDbContext(options);
            var controller = new YachtSalesController(context);
            controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            // Tworzenie instancji YachtSale z wszystkimi wymaganymi polami
            var yachtSale = new YachtSale
            {
                Id = 1,
                status = TransactionStatus.Pending,
                BuyerUserId = Guid.NewGuid(),
                Yacht = new Yachts
                {
                    Id = 1,
                    name = "Test Yacht",
                    description = "Test description",
                    location = "Test location",
                    manufacturer = "Test manufacturer",
                    model = "Test model",
                    type = "Test type"
                },
                currency = "USD", // Dodaj brakuj¹c¹ wartoœæ
                location = "Test Location", // Dodaj brakuj¹c¹ wartoœæ
                notes = "Test notes" // Dodaj brakuj¹c¹ wartoœæ
            };
            context.YachtSale.Add(yachtSale);
            await context.SaveChangesAsync();

            // Act
            var result = await controller.Accept(1);

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual("Details", redirectResult.ActionName);
            Assert.AreEqual("Zakup zosta³ zaakceptowany.", controller.TempData["Message"]);
            Assert.AreEqual("success", controller.TempData["AlertType"]);

            // Sprawdzenie, czy w³aœciciel jachtu zosta³ zaktualizowany
            var updatedYachtSale = await context.YachtSale.FindAsync(1);
            Assert.AreEqual(TransactionStatus.Accepted, updatedYachtSale.status);
            Assert.AreEqual(yachtSale.BuyerUserId, updatedYachtSale.Yacht.OwnerId); // Nowy w³aœciciel
        }
        /*
         * Cel: Testuje, czy metoda Reject prawid³owo przekierowuje i wyœwietla ostrze¿enie, 
         * gdy transakcja nie jest w stanie Pending (np. zosta³a ju¿ odrzucona).
         * 
         * Testuje, czy metoda Reject prawid³owo aktualizuje status transakcji na Pending 
         * oraz ustawia BuyerUserId na null (czyli usuwa kupuj¹cego).
         */
        [TestMethod]
        public async Task Reject_ShouldRedirectWithWarning_WhenTransactionIsNotPending()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AhoyDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using var context = new AhoyDbContext(options);
            var controller = new YachtSalesController(context);
            controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            // Tworzymy transakcjê z innym statusem ni¿ Pending (np. Rejected)
            var yachtSale = new YachtSale
            {
                price = 1000,
                currency = "USD",
                location = "Test location",
                notes = "Test notes",
                YachtId = 1,
                status = TransactionStatus.Rejected
            };

            var yacht = new Yachts
            {
                Id = 1,
                name = "Test Yacht",
                description = "Test description",
                location = "Test location",
                manufacturer = "Test manufacturer",
                model = "Test model",
                type = "Test type"
            };
            context.Yachts.Add(yacht);
            context.YachtSale.Add(yachtSale);
            await context.SaveChangesAsync();

            // Act
            var result = await controller.Reject(1);

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual("Details", redirectResult.ActionName);
            Assert.AreEqual("Ta transakcja nie jest ju¿ w stanie oczekiwania.", controller.TempData["Message"]);
            Assert.AreEqual("warning", controller.TempData["AlertType"]);
        }

        [TestMethod]
        public async Task Reject_ShouldUpdateStatusAndClearBuyer_WhenTransactionIsPending()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AhoyDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using var context = new AhoyDbContext(options);
            var controller = new YachtSalesController(context);
            controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            var yachtSale = new YachtSale
            {
                price = 1000,
                currency = "USD",
                location = "Test location",
                notes = "Test notes",
                YachtId = 1,
                status = TransactionStatus.Pending,
                BuyerUserId = Guid.NewGuid() // Zak³adaj¹c, ¿e kupuj¹cy jest przypisany
            };

            var yacht = new Yachts
            {
                Id = 1,
                name = "Test Yacht",
                description = "Test description",
                location = "Test location",
                manufacturer = "Test manufacturer",
                model = "Test model",
                type = "Test type"
            };
            context.Yachts.Add(yacht);
            context.YachtSale.Add(yachtSale);
       
            await context.SaveChangesAsync();

            // Act
            var result = await controller.Reject(1);

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual("Details", redirectResult.ActionName);
            Assert.AreEqual("Zakup zosta³ odrzucony.", controller.TempData["Message"]);
            Assert.AreEqual("danger", controller.TempData["AlertType"]);

            // Sprawdzamy, czy status zosta³ zaktualizowany na Pending oraz czy BuyerUserId jest ustawiony na null
            var updatedYachtSale = await context.YachtSale.FindAsync(1);
            Assert.AreEqual(TransactionStatus.Pending, updatedYachtSale.status);
            Assert.IsNull(updatedYachtSale.BuyerUserId); // Kupuj¹cy powinien zostaæ usuniêty
        }

    }
}
