using Lekkerbek.Web.Data;
using Lekkerbek.Web.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace LekkerbekTestProject.ModelValidations
{
    [TestClass]
    public class TimeSlotTests
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
        public void ValidateTimeSlotModel_NameIsRequired()
        {
            // Arrange
            var timeSlot = new TimeSlot
            {
                Id = 1,
                StartTimeSlot = default

            };
            // Act & Assert
            Assert.ThrowsExceptionAsync<ValidationException>(() =>
           _context.SaveChangesAsync());
        }

        [TestMethod]
        public void StartTimeSlot_HasCorrectDisplayName()
            {
                // Arrange
                var propInfo = typeof(TimeSlot).GetProperty("StartTimeSlot");
                var attribute = propInfo.GetCustomAttribute<DisplayAttribute>();

                // Act
                var displayName = attribute?.Name;

                // Assert
                Assert.AreEqual("Tijdslot", displayName);
            }

        [TestMethod]
        public void StartTimeSlot_HasRequiredAttribute()
            {
                // Arrange
                var propInfo = typeof(TimeSlot).GetProperty("StartTimeSlot");
                var attribute = propInfo.GetCustomAttribute<RequiredAttribute>();

                // Assert
                Assert.IsNotNull(attribute);
                Assert.AreEqual("Tijdslot moet ingevuld zijn", attribute.ErrorMessage);
            }

        [TestMethod]
        public void StartTimeSlot_HasDateTimeDataTypeAttribute()
            {
                // Arrange
                var propInfo = typeof(TimeSlot).GetProperty("StartTimeSlot");
                var attribute = propInfo.GetCustomAttribute<DataTypeAttribute>();

                // Assert
                Assert.IsNotNull(attribute);
                Assert.AreEqual(DataType.DateTime, attribute.DataType);
            }

        [TestMethod]
        public void ChefId_AllowsNullValue()
            {
                // Arrange
                var propInfo = typeof(TimeSlot).GetProperty("ChefId");

                // Assert
                Assert.IsTrue(propInfo.PropertyType == typeof(int?));
            }
        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}