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

namespace LekkerbekTestProject.Integraties
{
    [TestClass]
    public class RestaurantHollidayIntegrationTests
    {
        private OrdersController _controller;
        private RestaurantManagementController _restaurantManagementController;
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


            // Configure any additional dependencies for the controller (e.g.,services, repositories)
            var mockOrderService = new Mock<IOrderService>();
            var orderNr1 = new Order { OrderID = 1, CustomerId = 1, TimeSlotID = 1, Finished = false };
            //in the created mockservice, which acts like the normal service
            //we are making a setup so we can tell the service what to do when a function gets called
            //for example, here we are saying, when we want a specific customer, return the object, this wwill always if succeeded give back the same object
            mockOrderService.Setup(x => x.GetSpecificOrder(orderNr1.OrderID)).Returns(orderNr1);
            var orderNr2 = new Order { OrderID = 2, CustomerId = 2, TimeSlotID = 2, Finished = false };
            mockOrderService.Setup(x => x.Read()).Returns(
             new List<Order> { orderNr1, orderNr2 });

            

            _restaurantManagementController = new RestaurantManagementController(new RestaurantManagementService(new RestaurantManagementRepository(_context)))
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext {  }
                }
            };

            _controller = new OrdersController(mockOrderService.Object);

        }
        [TestMethod]
        public async Task AddRestaurantHolliday_ValidData_Success()
        {
            // Arrange
            
            var newHolliday = new RestaurantHoliday
            {
                StartDate = Convert.ToDateTime("11/11/2000 00:00"),
                EndDate = Convert.ToDateTime("12/12/2000 00:00"),
                Description = "desc"
            };
            // Act
            var result = _restaurantManagementController.CreateHolidayDay(newHolliday);
            
            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirectResult = (RedirectToActionResult)result;
            Assert.AreEqual("HolidayDays", redirectResult.ActionName);
            
            // Verify that the newCustomer object is created in the DbContext
            var savedCustomer = await _context.RestaurantHolidays.FirstOrDefaultAsync(c =>
            c.Description == "desc");
            Assert.IsNotNull(savedCustomer);
            Assert.AreEqual(Convert.ToDateTime("11/11/2000 00:00"), savedCustomer.StartDate);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _controller.Dispose();
            _context.Dispose();
        }

    }
}
