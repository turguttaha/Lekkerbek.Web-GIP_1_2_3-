using Lekkerbek.Web.Data;
using Lekkerbek.Web.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Quartz.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LekkerbekTestProject.ModelValidations
{
    public class CustomerTests
    {
        private LekkerbekContext _context;
        [TestInitialize]
        public void Initialize()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            var options = new DbContextOptionsBuilder<LekkerbekContext>()
            .UseSqlite(connection)
            .Options;
            _context = new LekkerbekContext(options);
            _context.Database.EnsureCreated();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
        [TestMethod]
        public void ValidateMenuItemModel_NameIsRequired()
        {
            // Arrange
            var menuItem = new MenuItem
            {
                MenuItemId = 1,
                Name = null,
                Description = "Cake",
                Price = 10,
                BtwNumber = 1,
                Sort = "Dessert",
            };
            // Act & Assert
            Assert.ThrowsExceptionAsync<ValidationException>(() =>
           _context.SaveChangesAsync());
        }
    }
}
