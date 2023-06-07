using Lekkerbek.Web.Data;
using Lekkerbek.Web.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LekkerbekTestProject.ModelValidations
{
    [TestClass]
    public class ChefTests
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

        [TestMethod]
        public void ValidateCustomerModel_NameIsRequired()
        {
            // Arrange
            var chef = new Chef
            {
                ChefId = 1,
                ChefName = null,

            };
            // Act & Assert
            Assert.ThrowsExceptionAsync<ValidationException>(() =>
           _context.SaveChangesAsync());
        }
        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

    }
}
