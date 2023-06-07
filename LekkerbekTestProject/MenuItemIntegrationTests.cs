using Lekkerbek.Web.Controllers;
using Lekkerbek.Web.Data;
using Lekkerbek.Web.Models;
using Lekkerbek.Web.NewFolder;
using Lekkerbek.Web.Services;
using Lekkerbek.Web.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
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
    public class MenuItemIntegrationTests
    {
        private LekkerbekContext _context;
        private UserManager<IdentityUser> _userManager;
        private ClaimsPrincipal _user;
        
 [TestInitialize]
        public void Initialize()
        {
            // Create an in-memory SQLite database
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            // Initialize the DbContext using the SQLite connection
            _context = new LekkerbekContext(new DbContextOptionsBuilder<LekkerbekContext>()
            .UseSqlite(connection)
            .Options);
            // Create the database schema
            _context.Database.EnsureCreated();
            // User manager
            var userManagerMock = new Mock<UserManager<IdentityUser>>(new
           Mock<IUserStore<IdentityUser>>().Object,
            null, null, null, null, null, null, null, null);
            userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync((string userId) => new IdentityUser { Id = userId });
            // Create the controller instance with the required dependencies
            var userMock = new Mock<ClaimsPrincipal>();
            userMock.Setup(x => x.Identity.IsAuthenticated).Returns(true);
            userMock.Setup(x => x.Identity.Name).Returns("testuser");
            userMock.Setup(x => x.FindFirst(ClaimTypes.Role)).Returns(new
           Claim(ClaimTypes.Role, "Administrator"));
            _userManager = userManagerMock.Object;
            _user = userMock.Object;
        }
        [TestCleanup]
        public void Cleanup()
        {
            _context.Dispose();
        }
        
 [TestMethod]
        public async Task AddMenuItem_ValidData_Success()
        {
            // Arrange
            var menuItemsController = new MenuItemsController(new MenuItemService(new MenuItemRepository(_context)), new OrderService(new OrdersRepository(_context),new CustomerService(new CustomersRepository (_context))))
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = _user }
                }
            };
            var newMenuItem = new MenuItem
            {
                Name = "Cake",
                Description = "Dessert"
            };
            // Act
            var result = await menuItemsController.Create(newMenuItem);
            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirectResult = (RedirectToActionResult)result;
            Assert.AreEqual("Index", redirectResult.ActionName);
            // Verify that the newCustomer object is created in the DbContext
            var savedMenuItem = await _context.MenuItems.FirstOrDefaultAsync(m => m.Name == "Cake");
            Assert.IsNotNull(savedMenuItem);
            Assert.AreEqual("Dessert", savedMenuItem.Description);
        }
    }
}
