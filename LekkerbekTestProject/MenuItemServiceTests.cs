using Lekkerbek.Web.Data;
using Lekkerbek.Web.Models;
using Lekkerbek.Web.NewFolder;
using Lekkerbek.Web.Services;
using Lekkerbek.Web.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz.Xml;

namespace LekkerbekTestProject
{
    [TestClass]
    public class MenuItemServiceTests
    {
        private LekkerbekContext _context;
        private MenuItemService _menuItemService;
        [TestInitialize]
        public void Setup()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            var options = new DbContextOptionsBuilder<LekkerbekContext>()
            .UseSqlite(connection)
            .Options;
            _context = new LekkerbekContext(options);
            _context.Database.EnsureCreated();
            _menuItemService = new MenuItemService(new MenuItemRepository(_context));
        }
        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
        [TestMethod]
        public async Task CreateMenuItem_Success()
        {
            // Arrange
            var menuItem = new MenuItem { MenuItemId = 1, Name = "Test" };
            // Act
            _menuItemService.Create(menuItem);
            // Assert
            
            var createdMenuItem = await _context.MenuItems.FirstOrDefaultAsync(M => M.MenuItemId == menuItem.MenuItemId);
            Assert.IsNotNull(createdMenuItem);
            // Additional assertions on the created customer properties
        }
        
        [TestMethod]
        public async Task GetSpecificMenuItem_Succes()
        {
            // Arrange
            var menuItem = new MenuItem { MenuItemId = 1, Name = "Test" };
            _context.MenuItems.Add(menuItem);
            await _context.SaveChangesAsync();
            // Act
            var retrievedMenuItem = _menuItemService.GetSpecificMenuItem(menuItem.MenuItemId);
            // Assert
            Assert.IsNotNull(retrievedMenuItem);
            Assert.AreEqual(menuItem.MenuItemId, retrievedMenuItem.MenuItemId);
        }
        [TestMethod]
        public async Task GetSpecificMenuItem_MenuItemNotFound()
        {
            // Arrange
            var nonExistentMenuItemId = 123;
            // Act
            var retrievedMenuItem = _menuItemService.GetSpecificMenuItem(nonExistentMenuItemId);
            // Assert
            Assert.IsNull(retrievedMenuItem);
        }
        [TestMethod]
        public async Task Update_Success()
        {
            // Arrange
            var menuItem = new MenuItem { MenuItemId = 1, Name = "No name??" };
            _context.MenuItems.Add(menuItem);
            await _context.SaveChangesAsync();
            menuItem.Name = "Name updated!";
            // Act
            
            _menuItemService.Update(menuItem);
            // Assert
            var retrievedMenuItem = await _context.MenuItems.FirstOrDefaultAsync(M => M.MenuItemId == menuItem.MenuItemId);
            Assert.IsNotNull(retrievedMenuItem);
            Assert.AreEqual(menuItem.Name, "Name updated!");
        }
    }
}
