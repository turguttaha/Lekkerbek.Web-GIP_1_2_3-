using Lekkerbek.Web.Controllers;
using Lekkerbek.Web.Data;
using Lekkerbek.Web.Models;
using Lekkerbek.Web.NewFolder;
using Lekkerbek.Web.Repositories;
using Lekkerbek.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using Quartz.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Telerik.SvgIcons;

namespace LekkerbekTestProject.ModelValidations
{
    [TestClass]
    public class RestaurantHollidayTest
    {
        private LekkerbekContext _context;

        [TestInitialize]
        public void Initialize()
        {
            // Setup an in-memory SQLite database
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            var options = new DbContextOptionsBuilder<LekkerbekContext>()
            .UseSqlite(connection)
            .Options;
            _context = new LekkerbekContext(options);
            _context.Database.EnsureCreated();


        }
        
        [TestMethod]
        public void ValidateRestaurantHolliday_DateRangeRequirement()
        {
            // Arrange
            var customer = new RestaurantHoliday
            {
                StartDate = default,
                EndDate = default
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
