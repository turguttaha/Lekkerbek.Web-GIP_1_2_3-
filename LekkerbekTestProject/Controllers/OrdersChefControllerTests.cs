using Lekkerbek.Web.Controllers;
using Lekkerbek.Web.Data;
using Lekkerbek.Web.Models;
using Lekkerbek.Web.Services;
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

namespace LekkerbekTestProject.Controllers
{
    [TestClass]
    public class OrdersChefControllerTests
    {
        private OrdersChefController _controller;
        [TestInitialize]
        public void Initialize()
        {
            // Setup an in-memory SQLite database
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            var options = new DbContextOptionsBuilder<LekkerbekContext>()
            .UseSqlite(connection)
            .Options;
            // Configure any additional dependencies for the controller (e.g., services, repositories)
            var mockOrderChefService = new Mock<IOrderChefService>();
            var mockCustomerService = new Mock<ICustomerService>();
            var mockOrderService = new Mock<IOrderService>();
            var mockMenuItemService = new Mock<IMenuItemService>();

            // Configure mockServices as needed
            var customerNr1 = new Customer { CustomerId = 1, FName = "John " };
            mockCustomerService.Setup(x =>
           x.GetSpecificCustomer(customerNr1.CustomerId)).Returns(customerNr1);
            var customerNr2 = new Customer { CustomerId = 2, FName = "Jane " };
            mockCustomerService.Setup(x => x.Read()).Returns(
            new List<Customer> { customerNr1, customerNr2 });

            var order = new Order
            {
                OrderID = 1,
                CustomerId = 2,
                Finished = false,
                TimeSlotID = 3,
            };
            mockOrderChefService.Setup(a=>a.GetChefsOrders(1)).Returns(order);

            var userManagerMock = new Mock<UserManager<IdentityUser>>(new
           Mock<IUserStore<IdentityUser>>().Object,
            null, null, null, null, null, null, null, null);

            userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync((string userId) => new IdentityUser { Id = userId });
           
            var user = new Mock<ClaimsPrincipal>();
            user.Setup(x => x.Identity.IsAuthenticated).Returns(true);
            user.Setup(x => x.Identity.Name).Returns("testuser");
            user.Setup(x => x.FindFirst(ClaimTypes.Role)).Returns(new
           Claim(ClaimTypes.Role, "Chef"));
            _controller = new OrdersChefController(mockOrderChefService.Object, mockCustomerService.Object,
           mockOrderService.Object, mockMenuItemService.Object, userManagerMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = user.Object }
                }
            };
        }
        [TestCleanup]
        public void Cleanup()
        {
            _controller.Dispose();
        }
        [TestMethod]
        public async Task Details_ReturnsRedirectToAction()
        {
            // Arrange
            // service objects setup in the Initialize method
            try
            {
                // Act
                var result = await _controller.OrderDetails(1);
                // Assert
                Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
                var viewResult = (RedirectToActionResult)result;
                // check the viewresult model
                Assert.AreEqual("Index", viewResult.ActionName);

            }
            catch (AssertFailedException)
            {
                Assert.Fail("The Details result is no ViewResult!");
            }
        }

    }
}
    