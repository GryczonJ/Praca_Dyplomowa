
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
using System.Web.Mvc;
using RedirectResult = Microsoft.AspNetCore.Mvc.RedirectResult;
using ControllerContext = Microsoft.AspNetCore.Mvc.ControllerContext;

namespace Tasty
{
    [TestClass]
    public class UnitTest1
    {
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


        /*[TestMethod]
        public async Task Create_ShouldRedirectToPreviousPage_WhenModelIsValid()
        {
            // Arrange
            var mockContext = new Mock<AhoyDbContext>();
            var mockDbSet = new Mock<DbSet<Comments>>();
            mockContext.Setup(c => c.Comments).Returns(mockDbSet.Object);

            var controller = new CommentsController(mockContext.Object);

            // Mockowanie HttpContext i nag³ówków
            var mockHttpContext = new Mock<HttpContext>();
            var mockRequest = new Mock<HttpRequest>();
            mockRequest.Setup(r => r.Headers["Referer"]).Returns("http://example.com/previous-page");
            mockHttpContext.Setup(c => c.Request).Returns(mockRequest.Object);
            controller.ControllerContext.HttpContext = mockHttpContext.Object;

            var comments = new Comments
            {
                Id = 1,
                Message = "Test message",
                CreatorId = Guid.NewGuid(),
                CharterId = 123
            };

            // Act
            var result = await controller.Create(comments);

            // Assert
            var redirectResult = result as RedirectToActionResult;

            Assert.IsNotNull(redirectResult); // Upewniamy siê, ¿e wynik jest typu RedirectToActionResult
            // Jeœli wynik jest przekierowaniem, sprawdzamy jego URL
            Assert.IsNotNull(redirectResult?.UrlHelper); // Sprawdzamy, czy URL nie jest null
            //Assert.AreEqual("http://example.com/previous-page", redirectResult.UrlHelper); // Sprawdzamy, czy URL jest zgodny
                                                                                          
        }*/
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
                Id = 1,
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
      
    }
 }
