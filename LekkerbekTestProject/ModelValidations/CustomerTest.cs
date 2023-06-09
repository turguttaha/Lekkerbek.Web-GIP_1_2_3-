using Lekkerbek.Web.Data;
using Lekkerbek.Web.Models;
using Lekkerbek.Web.NewFolder;
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
    internal class CustomerTest
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
            public void ValidateCustomerModel_NameIsRequired()
            {
                // Arrange
                var customer = new Customer
                {
                    CustomerId = 1,
                    FName = null,
                    PhoneNumber = "12345678",
                    //EmailAddress = "test@example.com",
                    Birthday = DateTime.Today,
                };
                // Act & Assert
                Assert.ThrowsExceptionAsync<ValidationException>(() =>
                _context.SaveChangesAsync());
            }
        }
    }
}
