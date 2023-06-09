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
        public class CustomersControllerTests
        {
            private CustomersController _controller;
            [TestInitialize]
            public void Initialize()
            {
                // Setup an in-memory SQLite database
                var connection = new SqliteConnection("DataSource=:memory:");
                connection.Open();
                var options = new DbContextOptionsBuilder<LekkerbekContext>()
                .UseSqlite(connection)
                .Options;
                // Configure any additional dependencies for the controller (e.g.,services, repositories)
var mockCustomerService = new Mock<ICustomerService>();
                var mockOrderService = new Mock<IOrderService>();
                // Configure mockServices as needed
                var customerNr1 = new Customer { CustomerId = 1, FName = "John Doe" };
                mockCustomerService.Setup(x => x.GetSpecificCustomer(customerNr1.CustomerId)).Returns(customerNr1);
                var customerNr2 = new Customer { CustomerId = 2, FName = "Jane Doe" };
                mockCustomerService.Setup(x => x.Read()).Returns(
                new List<Customer> { customerNr1, customerNr2 });
                var userManagerMock = new Mock<UserManager<IdentityUser>>(new
                Mock<IUserStore<IdentityUser>>().Object,
                null, null, null, null, null, null, null, null);
                userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((string userId) => new IdentityUser { Id = userId });
                // Create the controller instance with the required dependencies
            var user = new Mock<ClaimsPrincipal>();
                user.Setup(x => x.Identity.IsAuthenticated).Returns(true);
                user.Setup(x => x.Identity.Name).Returns("testuser");
                user.Setup(x => x.FindFirst(ClaimTypes.Role)).Returns(new
                Claim(ClaimTypes.Role, "Administrator"));
                _controller = new CustomersController(mockCustomerService.Object,
                mockOrderService.Object, userManagerMock.Object)
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
            public async Task Details_ReturnsViewResultWithCustomer()
            {
                // Arrange
                // service objects setup in the Initialize method
                try
                {
                    // Act
                    var result = _controller.Create();
                    // Assert
                    Assert.IsInstanceOfType(result, typeof(ViewResult));
                }
                catch (AssertFailedException)
                {
                    Assert.Fail("The Details result is no ViewResult!");
                }
            }
        }
    }


