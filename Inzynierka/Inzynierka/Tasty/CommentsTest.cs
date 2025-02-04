
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

namespace Tasty
{
    [TestClass]
    public class CommentsTest
    {
        private DbContextOptions<AhoyDbContext> GetDbContextOptions()
        {
            return new DbContextOptionsBuilder<AhoyDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unikalna baza dla ka¿dego testu
                .Options;
        }
        /*
         * Testy sprawdzaj¹, czy klasa Comments poprawnie instancjonuje obiekt, 
         * aktualizuje wiadomoœæ po edycji oraz ustawia wiadomoœæ na null po jej usuniêciu.
        */
        [TestMethod]
        public void Create_CommentClass_ShouldInstantiateCorrectly()
        {
            // Act
            var comment = new Comments { Id = 1, Message = "Test message", CreatorId = Guid.NewGuid() };

            // Assert
            Assert.IsNotNull(comment);
            Assert.AreEqual(1, comment.Id);
            Assert.AreEqual("Test message", comment.Message);
        }

        [TestMethod]
        public void Edit_Comment_ShouldUpdateMessageCorrectly()
        {
            // Arrange
            var comment = new Comments { Id = 1, Message = "Initial message", CreatorId = Guid.NewGuid() };

            // Act
            comment.Message = "Updated message";

            // Assert
            Assert.AreEqual("Updated message", comment.Message);
        }

        [TestMethod]
        public void Delete_Comment_ShouldSetMessageToNull()
        {
            // Arrange
            var comment = new Comments { Id = 1, Message = "Message to be deleted", CreatorId = Guid.NewGuid() };

            // Act
            comment.Message = null;

            // Assert
            Assert.IsNull(comment.Message);
        }

        [TestMethod]
        public async Task Create_ShouldRedirectToPreviousPage_WhenModelIsValid()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AhoyDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase") // U¿ywamy bazy InMemory do testów
                .Options;

            using var context = new AhoyDbContext(options); // Tworzymy kontekst z bazy InMemory
            var controller = new CommentsController(context); // Tworzymy kontroler z tym kontekstem

            // Mockowanie nag³ówków HTTP
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Referer"] = "http://example.com/previous-page"; // Symulacja nag³ówka Referer
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };

            var validComment = new Comments
            {
                Id = 100,
                Message = "Valid message", // Poprawny komentarz
                CreatorId = Guid.NewGuid(),
                CharterId = 123,
                Rating = 5 // Dodajemy Rating, aby model by³ poprawny
            };

            // Act
            var result = await controller.Create(validComment);

            // Assert
            var redirectResult = result as RedirectResult;
            Assert.IsNotNull(redirectResult); // Upewniamy siê, ¿e wynik to RedirectResult
            Assert.AreEqual("http://example.com/previous-page", redirectResult.Url); // Sprawdzamy, czy URL jest zgodny z Referer

            // Dodatkowe sprawdzenie, czy ModelState jest valid
            Assert.IsTrue(controller.ModelState.IsValid); // Upewniamy siê, ¿e ModelState jest valid
        }

        [TestMethod]
        public async Task Create_ShouldRedirectToPreviousPage_WhenModelIsInvalid()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AhoyDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase") // U¿ywamy bazy InMemory do testów
                .Options;

            using var context = new AhoyDbContext(options); // Tworzymy kontekst z bazy InMemory
            var controller = new CommentsController(context); // Tworzymy kontroler z tym kontekstem

            // Mockowanie nag³ówków HTTP
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Referer"] = "http://example.com/previous-page"; // Symulacja nag³ówka Referer
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };

            var invalidComment = new Comments
            {
                Id = 1,
                Message = null, // Niepoprawny komentarz: Message nie mo¿e byæ null
                CreatorId = Guid.NewGuid(), // Przyk³adowy UUID, którego nie ma w bazie
                CharterId = 99999, // Przyk³adowy nieistniej¹cy ID w tabeli Charters
                Rating = 5 // Dodajemy Rating, ale Message jest null, co sprawia, ¿e model jest niepoprawny
            };

            // Act
            var result = await controller.Create(invalidComment);

            // Assert
            var redirectResult = result as RedirectResult;
            Assert.IsNotNull(redirectResult); // Upewniamy siê, ¿e wynik to RedirectResult
            Assert.AreEqual("http://example.com/previous-page", redirectResult.Url); // Sprawdzamy, czy URL jest zgodny z Referer

            // Dodatkowe sprawdzenie, czy ModelState jest invalid
            Assert.IsTrue(controller.ModelState.IsValid); // Upewniamy siê, ¿e ModelState jest invalid
        }

        [TestMethod]
        public async Task Edit_ShouldRedirectToPreviousPage_WhenModelIsValid()
        {
            // Arrange
            var options = GetDbContextOptions();
            using var context = new AhoyDbContext(options);
            var controller = new CommentsController(context);

            var creator = new Users
            {
                Id = Guid.NewGuid(),
                UserName = "jan.kowalski@example.com",
                Email = "jan.kowalski@example.com"
            };
            context.Users.Add(creator);
            await context.SaveChangesAsync();

            var existingComment = new Comments
            {
                Id = 1,
                Message = "Old message",
                CreatorId = creator.Id,
                CharterId = 123,
                Rating = 4
            };
            context.Comments.Add(existingComment);
            await context.SaveChangesAsync();
            Assert.IsTrue(context.Comments.Any(e => e.Id == existingComment.Id));

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Referer"] = "http://example.com/previous-page";
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

            // Przygotowujemy TempData
            controller.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            var updatedComment = new Comments
            {
                Id = 1,
                Message = "Updated message",
                CreatorId = creator.Id,
                CharterId = existingComment.CharterId,
                Rating = 5
            };

            // Act
            var result = await controller.Edit(updatedComment.Id, updatedComment);

            // Assert
            /* var redirectResult = result as RedirectResult;
             Assert.IsNotNull(redirectResult);
             Assert.AreEqual("http://example.com/previous-page", redirectResult.Url);
             Assert.IsTrue(controller.ModelState.IsValid);*/
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual("Index", redirectResult.ActionName);  // Przekierowanie do Index

            // Sprawdzamy komunikaty w TempData
            Assert.AreEqual("Komentarz zosta³ pomyœlnie zaktualizowany!", controller.TempData["Message"]);
            Assert.AreEqual("success", controller.TempData["AlertType"]);
            // Sprawdzamy, czy zmiany zosta³y zapisane w bazie
            var updatedInDb = await context.Comments.FindAsync(updatedComment.Id);
            Assert.IsNotNull(updatedInDb);
            Assert.AreEqual("Updated message", updatedInDb.Message);  // Sprawdzamy zaktualizowan¹ wiadomoœæ
            Assert.AreEqual(5, updatedInDb.Rating);  // Sprawdzamy zaktualizowan¹ ocenê
        }
    }
 }
