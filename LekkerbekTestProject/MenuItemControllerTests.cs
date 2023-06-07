using Lekkerbek.Web.Controllers;
using Lekkerbek.Web.Data;
using Lekkerbek.Web.Models;
using Lekkerbek.Web.ViewModel;
using Lekkerbek.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Lekkerbek.Web.Services;
using Lekkerbek.Web.Repositories;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LekkerbekTestProject
{
    [TestClass]
    public class MenuItemControllerTests
    {
        private MenuItemsController _controller;
        [TestInitialize]
        public void Initialize()
        {
            // Setup an in-memory SQLite database
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            var options = new DbContextOptionsBuilder<LekkerbekContext>()
            .UseSqlite(connection)
            .Options;
            // Configure any additional dependencies for the controller (e.g.,services, repositories)
            var mockMenuItemService = new Mock<IMenuItemService>();
            var mockOrderService = new Mock<IOrderService>();
            // Configure mockServices as needed
            var menuItemNr1 = new MenuItemViewModel { MenuItemId = 1, Name = "Cake" };
            var menuItemNr3 = new MenuItem { MenuItemId = 1, Name = "Cake" };
            mockMenuItemService.Setup(M => M.GetSpecificMenuItem(menuItemNr1.MenuItemId)).Returns(menuItemNr3);
            var menuItemNr2 = new MenuItemViewModel { MenuItemId = 2, Name = "Cake 2" };
            mockMenuItemService.Setup(x => x.Read()).Returns(new List<MenuItemViewModel> { menuItemNr1, menuItemNr2 });

            var userManagerMock = new Mock<UserManager<IdentityUser>>(new Mock<IUserStore<IdentityUser>>().Object,
            null, null, null, null, null, null, null, null);
            userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync((string userId) => new IdentityUser { Id = userId });
            // Create the controller instance with the required dependencies
            
            var user = new Mock<ClaimsPrincipal>();
            user.Setup(x => x.Identity.IsAuthenticated).Returns(true);
            user.Setup(x => x.Identity.Name).Returns("testuser");
            user.Setup(x => x.FindFirst(ClaimTypes.Role)).Returns(new
            Claim(ClaimTypes.Role, "Administrator"));
            _controller = new MenuItemsController(mockMenuItemService.Object, mockOrderService.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = user.Object }
                }
            };
        }
        [TestCleanup]
        public void Cleanup()
        {
            _controller.Dispose();
        }
        [TestMethod]
        public async Task Create_ReturnsViewResultWithMenuItem()
        {
            // Arrange
            // service objects setup in the Initialize method
            try
            {
                // Act
                var result = _controller.Create();
                // Assert
                Assert.IsInstanceOfType(result, typeof(ViewResult));
                
                
            }
            catch (AssertFailedException)
            {
                Assert.Fail("The Details result is no ViewResult!");
            }
        }
    }
}


