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

namespace LekkerbekTestProject.Services
{
    [TestClass]
    public class RestaurantManagementServiceTest
    {
        
        private LekkerbekContext _context;
        private RestaurantManagementService _restaurantManagementService;

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

            // Configure any additional dependencies for the controller (e.g.,services, repositories)
            var mockOrderService = new Mock<IOrderService>();
            var orderNr1 = new Order { OrderID = 1, CustomerId = 1, TimeSlotID = 1, Finished = false };
            //in the created mockservice, which acts like the normal service
            //we are making a setup so we can tell the service what to do when a function gets called
            //for example, here we are saying, when we want a specific customer, return the object, this will always if succeeded give back the same object
            mockOrderService.Setup(x => x.GetSpecificOrder(orderNr1.OrderID)).Returns(orderNr1);
            var orderNr2 = new Order { OrderID = 2, CustomerId = 2, TimeSlotID = 2, Finished = false };
            mockOrderService.Setup(x => x.Read()).Returns(
             new List<Order> { orderNr1, orderNr2 });

            _restaurantManagementService = new RestaurantManagementService(new RestaurantManagementRepository(_context));


        }
        

        [TestMethod]
        public async Task CreateRestaurantHolliday_Success()
        {
            // Arrange
            var timeSlot = new RestaurantHoliday { RestaurantHolidayId = 1, StartDate = Convert.ToDateTime("12/12/2000 00:00"), EndDate = Convert.ToDateTime("12/12/2000 00:00"), Description = "desc" };
            
            // Act
            _restaurantManagementService.CreateHolidayDay(timeSlot);
           
            // Assert
            var createdHolliday = await _context.RestaurantHolidays.FirstOrDefaultAsync(o => o.RestaurantHolidayId == timeSlot.RestaurantHolidayId);
            Assert.IsNotNull(createdHolliday);
            
        }

        

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

    }
}
