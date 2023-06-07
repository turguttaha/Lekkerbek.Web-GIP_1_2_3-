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
    public class ServiceMockTest
    {
        private OrdersController _controller;
        private RestaurantManagementController _restaurantManagementController;
        private LekkerbekContext _context;
        private RestaurantManagementRepository _restaurantManagementRepository;

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

            // Create the controller instance with the required dependencies
            var userManagerMock = new Mock<UserManager<IdentityUser>>(new
            Mock<IUserStore<IdentityUser>>().Object,
            null, null, null, null, null, null, null, null);
            userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync((string userId) => new IdentityUser { Id = userId });
            // Create test users
            var adminUser = new IdentityUser
            {
                UserName = "adminuser@ucll.be",
                Email
           = "adminuser@ucll.be"
            };
            var customerUser = new IdentityUser
            {
                UserName = "customeruser@ucll.be",
                Email = "customeruser@ucll.be"
            };
            // Create test role
            var adminRole = new IdentityRole { Name = "Administrator" };
            var custRole = new IdentityRole { Name = "Customer" };
            // Add the users to the UserManager
            var users = new List<IdentityUser> { adminUser, customerUser }.AsQueryable();
            userManagerMock.Setup(u => u.Users).Returns(users);
            // Assign the users to the role
            userManagerMock.Setup(u => u.IsInRoleAsync(adminUser,
           adminRole.Name)).ReturnsAsync(true);
            userManagerMock.Setup(u => u.IsInRoleAsync(customerUser,
           custRole.Name)).ReturnsAsync(true);
            var adminUsers = new List<IdentityUser> { adminUser };
            userManagerMock.Setup(u =>
           u.GetUsersInRoleAsync(adminRole.Name)).ReturnsAsync(adminUsers);

            var customerUsers = new List<IdentityUser> { customerUser };
            userManagerMock.Setup(u =>
           u.GetUsersInRoleAsync(custRole.Name)).ReturnsAsync(customerUsers);
            // Create the controller instance with the required dependencies
            var userMock = new Mock<ClaimsPrincipal>();
            userMock.Setup(x => x.Identity.IsAuthenticated).Returns(true);
            userMock.Setup(x => x.Identity.Name).Returns("customerUser");
            userMock.Setup(x => x.IsInRole("Administrator")).Returns(false);
            userMock.Setup(x => x.IsInRole("CookTest")).Returns(true);

            _restaurantManagementController = new RestaurantManagementController(new RestaurantManagementService(new RestaurantManagementRepository(_context)))
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = userMock.Object }
                }
            };

            _controller = new OrdersController(mockOrderService.Object);

            _restaurantManagementRepository = new RestaurantManagementRepository(_context);


        }
        

        [TestMethod]
        public async Task CreateCustomer_Success()
        {
            //2.2


            // Arrange

            var timeSlot = new RestaurantHoliday { RestaurantHolidayId = 1, StartDate = Convert.ToDateTime("12/12/2000 00:00"), EndDate = Convert.ToDateTime("12/12/2000 00:00"), Description = ":D" };
            // Act
            _restaurantManagementRepository.AddToDatabaseHolidayDay(timeSlot);
            // Assert

            var createdHolliday = await _context.RestaurantHolidays.FirstOrDefaultAsync(o => o.RestaurantHolidayId == timeSlot.RestaurantHolidayId);
            Assert.IsNotNull(createdHolliday);
            // Additional assertions on the created customer properties
        }

        [TestMethod]
        public async Task TestMethod1()
        {

            //2.3
            // Arrange
            // service objects setup in the Initialize method
            try
            {
                // Act
                Mock<IOrderService> mockOrderService = new Mock<IOrderService>();

                var results = _controller.Index();
                // Assert
                Assert.IsInstanceOfType(results, typeof(IActionResult));



            }
            catch (AssertFailedException)
            {
                Assert.Fail("The Index is no IActionResult!");
            }


        }
        [TestMethod]
        public void ValidateOrderModel_NameIsRequired()
        {
            //2.1

            //ModelValidationTest

            // Arrange
            var customer = new Customer
            {
                CustomerId = 1,
                FName = null,
                Email = "test@example.com"
            };
            // Act & Assert
            Assert.ThrowsExceptionAsync<ValidationException>(() =>
           _context.SaveChangesAsync());
        }
        [TestMethod]
        public async Task AddCustomer_ValidData_Success()
        {
            // Arrange
            var restaurantManagementController = new RestaurantManagementController(new
           RestaurantManagementService(new RestaurantManagementRepository(_context)))
            {
                ControllerContext = new ControllerContext
                {

                }
            };
            var newHolliday = new RestaurantHoliday
            {
                StartDate = Convert.ToDateTime("11/11/2000 00:00"),
                EndDate = Convert.ToDateTime("12/12/2000 00:00"),
                Description = ":D"
            };
            // Act
            var result = restaurantManagementController.CreateHolidayDay(newHolliday);
            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirectResult = (RedirectToActionResult)result;
            Assert.AreEqual("HolidayDays", redirectResult.ActionName);
            // Verify that the newCustomer object is created in the DbContext
            var savedCustomer = await _context.RestaurantHolidays.FirstOrDefaultAsync(c =>
           c.Description == ":D");
            Assert.IsNotNull(savedCustomer);
            Assert.AreEqual(Convert.ToDateTime("11/11/2000 00:00"), savedCustomer.StartDate);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _controller.Dispose();
            _restaurantManagementController.Dispose();
            _context.Dispose();
        }

    }
}
