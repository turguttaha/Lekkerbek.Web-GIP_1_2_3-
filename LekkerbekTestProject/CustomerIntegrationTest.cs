using Lekkerbek.Web.Controllers;
using Lekkerbek.Web.Data;
using Lekkerbek.Web.Models;
using Lekkerbek.Web.NewFolder;
using Lekkerbek.Web.Services;

using Lekkerbek.Web.Repositories;
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

namespace LekkerbekTestProject
{

        [TestClass]
        public class CustomerIntegrationTests
        {
            private LekkerbekContext _context;
            private UserManager<IdentityUser> _userManager;
            private ClaimsPrincipal _user;
        [TestInitialize]
            public void Initialize()
            {
                // Create an in-memory SQLite database
                var connection = new SqliteConnection("DataSource=:memory:");
                connection.Open();
                // Initialize the DbContext using the SQLite connection
                _context = new LekkerbekContext(new
                DbContextOptionsBuilder<LekkerbekContext>()
                .UseSqlite(connection)
                .Options);
                // Create the database schema
                _context.Database.EnsureCreated();
                // User manager
                var userManagerMock = new Mock<UserManager<IdentityUser>>(new
                Mock<IUserStore<IdentityUser>>().Object,
                null, null, null, null, null, null, null, null);
                userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((string userId) => new IdentityUser { Id = userId });
                // Create the controller instance with the required dependencies
                var userMock = new Mock<ClaimsPrincipal>();
                userMock.Setup(x => x.Identity.IsAuthenticated).Returns(true);
                userMock.Setup(x => x.Identity.Name).Returns("testuser");
                userMock.Setup(x => x.FindFirst(ClaimTypes.Role)).Returns(new
                Claim(ClaimTypes.Role, "Administrator"));
                _userManager = userManagerMock.Object;
                _user = userMock.Object;
            }
            [TestCleanup]
            public void Cleanup()
            {
                _context.Database.EnsureDeleted();
                _context.Dispose();
                _userManager.Dispose();
            }
            [TestMethod]
            
            public async Task AddCustomer_ValidData_Success()
            {
                // Arrange
                var customerController = new CustomersController(new
                CustomerService(new CustomersRepository(_context)), new OrderService(new OrdersRepository(_context), new CustomerService(new CustomersRepository(_context))), _userManager)
                {
                    ControllerContext = new ControllerContext
                    {
                        HttpContext = new DefaultHttpContext { User = _user }
                    }
                };
                var newCustomer = new Customer
                {
                    FName = "Customer1",
                    LName = "Customer2",
                    Email = "customer.nr1@ucll.be"
                };
                // Act
                var result = await customerController.Create(newCustomer);
                // Assert
                Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
                var redirectResult = (RedirectToActionResult)result;
                Assert.AreEqual("Index", redirectResult.ActionName);
                // Verify that the newCustomer object is created in the DbContext
                var savedCustomer = await _context.Customers.FirstOrDefaultAsync(c =>
                c.FName == "Customer1");
                Assert.IsNotNull(savedCustomer);
                Assert.AreEqual("customer.nr1@ucll.be", savedCustomer.Email);
            }
        }
    }

