using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Inzynierka.Models;
using System;
using System.Web.Mvc;
using RedirectResult = Microsoft.AspNetCore.Mvc.RedirectResult;
using ControllerContext = Microsoft.AspNetCore.Mvc.ControllerContext;

namespace Tasty
{
    [TestClass]
    public class ChartersTest
    {
        [TestMethod]
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
        }

    }
}
