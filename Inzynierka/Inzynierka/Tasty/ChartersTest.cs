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
namespace Tasty
{
    [TestClass]
    public class ChartersTest
    {
        
        [TestMethod]
        public async Task Create_ShouldRedirectToIndex_WhenCharterIsCreatedSuccessfully()
        {// Arrange
            var options = new DbContextOptionsBuilder<AhoyDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            // Ustawienie kontekstu i kontrolera
            using var context = new AhoyDbContext(options);
            var controller = new ChartersController(context);

            // Symulacja zalogowanego użytkownika
            var user = new Users { Id = Guid.NewGuid(), banned = false };
            context.Users.Add(user);
            await context.SaveChangesAsync(); // Użycie async w SaveChanges

            var claimsIdentity = new ClaimsIdentity(new[]
            {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        }, "test");

            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = claimsPrincipal }
            };


            // Symulacja TempData
            controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            // Przygotowanie obiektu Charters
            var charter = new Charters
            {
                startDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                endDate = DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
                price = 1000,
                location = "Test location",
                description = "Test description",
                currency = "USD",
                YachtId = 1,
                OwnerId = user.Id
            };

            // Act
            var result = await controller.Create(charter);

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual("Index", redirectResult.ActionName);
            Assert.AreEqual("Charter został pomyślnie dodany!", controller.TempData["Message"]);
        }
        [TestMethod]
        public async Task Edit_ShouldRedirectToIndex_WhenCharterIsEditedSuccessfully()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AhoyDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            // Ustawienie kontekstu i kontrolera
            using var context = new AhoyDbContext(options);
            var controller = new ChartersController(context);

            // Symulacja zalogowanego użytkownika
            var user = new Users { Id = Guid.NewGuid(), banned = false };
            context.Users.Add(user);
            await context.SaveChangesAsync(); // Użycie async w SaveChanges

            var claimsIdentity = new ClaimsIdentity(new[]
            {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
    }, "test");

            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = claimsPrincipal }
            };

            // Symulacja TempData
            controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            // Przygotowanie obiektu Charters
            var charter = new Charters
            {
                startDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                endDate = DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
                price = 1000,
                location = "Test location",
                description = "Test description",
                currency = "USD",
                YachtId = 1,
                OwnerId = user.Id
            };
            context.Charters.Add(charter);
            await context.SaveChangesAsync(); // Zapisanie początkowego czarteru

            var charterToEdit = await context.Charters.FindAsync(charter.Id);
            charterToEdit.price = 1500;  // Zmiana ceny na potrzeby edycji


            // Act
            var result = await controller.Edit(charterToEdit.Id, charterToEdit);
            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual("Index", redirectResult.ActionName);
            //Assert.AreEqual("Czarter został pomyślnie edytowany!", controller.TempData["Message"]);
        }

        /*        [TestMethod]
        public async Task Edit_ShouldReturnToIndex_WhenYachtHasPlannedCruise()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AhoyDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using var context = new AhoyDbContext(options);
            var controller = new ChartersController(context);

            var user = new Users { Id = Guid.NewGuid(), banned = false };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var yacht = new Yachts { Id = 1, name = "Test Yacht" };
            context.Yachts.Add(yacht);
            await context.SaveChangesAsync();

            // Symulacja czarteru i rejsu
            var charter = new Charters
            {
                startDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                endDate = DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
                price = 1000,
                location = "Test location",
                description = "Test description",
                currency = "USD",
                YachtId = yacht.Id,
                OwnerId = user.Id
            };
            context.Charters.Add(charter);
            await context.SaveChangesAsync();

            var cruise = new Cruises
            {
                YachtId = yacht.Id,
                start_date = DateTime.Now.AddDays(1),
                end_date = DateTime.Now.AddDays(2)
            };
            context.Cruises.Add(cruise);
            await context.SaveChangesAsync();

            // Przygotowanie edytowanego czarteru
            var editedCharter = new Charters
            {
                Id = charter.Id,
                startDate = DateOnly.FromDateTime(DateTime.Now.AddDays(3)),
                endDate = DateOnly.FromDateTime(DateTime.Now.AddDays(4)),
                price = 1500,
                location = "Updated location",
                description = "Updated description",
                currency = "EUR",
                YachtId = yacht.Id,
                OwnerId = user.Id
            };

            // Act
            var result = await controller.Edit(editedCharter.Id, editedCharter);

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual("Index", redirectResult.ActionName);
            Assert.AreEqual("Nie można edytować czarteru, ponieważ jacht ma zaplanowany rejs, którego daty pokrywają się z datami czarteru.", controller.TempData["Message"]);
        }*/
        /*[TestMethod]
         public async Task Create_ShouldRedirectWithMessage_WhenUserIsBanned()
         {
             // Arrange
             var options = new DbContextOptionsBuilder<AhoyDbContext>()
                 .UseInMemoryDatabase(databaseName: "TestDatabase")
                 .Options;

             using var context = new AhoyDbContext(options);
             var controller = new ChartersController(context);

             var bannedUser = new Users { Id = Guid.NewGuid(), banned = true };
             context.Users.Add(bannedUser);
             context.SaveChanges();

             var charter = new Charters
             {
                 startDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                 endDate = DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
                 price = 1000,
                 location = "Test location",
                 description = "Test description",
                 currency = "USD",
                 YachtId = 1,
                 OwnerId = bannedUser.Id
             };

             // Act
             var result = await controller.Create(charter);

             // Assert
             var redirectResult = result as RedirectToActionResult;
             Assert.IsNotNull(redirectResult);
             Assert.AreEqual("Index", redirectResult.ActionName);
             Assert.AreEqual("Twoje konto jest zbanowane. Nie możesz dodawać nowych czarterów.", controller.TempData["Message"]);
         }*/
        /* [TestMethod]
         public async Task Create_ShouldRedirectToLogin_WhenUserIsNotLoggedIn()
         {
             // Arrange
             var options = new DbContextOptionsBuilder<AhoyDbContext>()
                 .UseInMemoryDatabase(databaseName: "TestDatabase")
                 .Options;

             using var context = new AhoyDbContext(options);
             var controller = new ChartersController(context);

             var charter = new Charters
             {
                 startDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                 endDate = DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
                 price = 1000,
                 location = "Test location",
                 description = "Test description",
                 currency = "USD",
                 YachtId = 1
             };

             // Act
             var result = await controller.Create(charter);

             // Assert
             var redirectResult = result as RedirectToActionResult;
             Assert.IsNotNull(redirectResult);
             Assert.AreEqual("Index", redirectResult.ActionName);
             Assert.AreEqual("Aby dodać czarter, musisz się zalogować.", controller.TempData["Message"]);
         }*/
        /*[TestMethod]
        public async Task Create_ShouldRedirectWithMessage_WhenYachtHasPlannedCruise()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AhoyDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using var context = new AhoyDbContext(options);
            var controller = new ChartersController(context);

            // Dodanie jachtu i zaplanowanego rejsu
            var yacht = new Yachts { Id = 1, name = "Test Yacht" };
            context.Yachts.Add(yacht);
            context.Cruises.Add(new Cruises
            {
                YachtId = yacht.Id,
                start_date = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                end_date = DateOnly.FromDateTime(DateTime.Now.AddDays(5))
            });
            context.SaveChanges();

            var charter = new Charters
            {
                startDate = DateOnly.FromDateTime(DateTime.Now.AddDays(2)),  // Data rejsu pokrywa się z czarterem
                endDate = DateOnly.FromDateTime(DateTime.Now.AddDays(3)),
                price = 1000,
                location = "Test location",
                description = "Test description",
                currency = "USD",
                YachtId = yacht.Id
            };

            // Act
            var result = await controller.Create(charter);

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual("Index", redirectResult.ActionName);
            Assert.AreEqual("Nie można dodać czarteru, ponieważ jacht ma zaplanowany rejs.", controller.TempData["Message"]);
        }*/
        /*[TestMethod]
        public async Task Create_ShouldRedirectWithMessage_WhenYachtIsForSale()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AhoyDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using var context = new AhoyDbContext(options);
            var controller = new ChartersController(context);

            // Dodanie jachtu i wystawienie go na sprzedaż
            var yacht = new Yachts { Id = 1, name = "Test Yacht" };
            context.Yachts.Add(yacht);
            context.YachtSale.Add(new YachtSale { YachtId = yacht.Id, status = TransactionStatus.Pending });
            context.SaveChanges();

            var charter = new Charters
            {
                startDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                endDate = DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
                price = 1000,
                location = "Test location",
                description = "Test description",
                currency = "USD",
                YachtId = yacht.Id
            };

            // Act
            var result = await controller.Create(charter);

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual("Index", redirectResult.ActionName);
            Assert.AreEqual("Nie można dodać czarteru, ponieważ jacht jest wystawiony na sprzedaż.", controller.TempData["Message"]);
        }*/
    }
}
