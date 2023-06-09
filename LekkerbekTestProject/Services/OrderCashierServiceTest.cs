using Lekkerbek.Web.Data;
using Lekkerbek.Web.Models;
using Lekkerbek.Web.NewFolder;
using Lekkerbek.Web.Repositories;
using Lekkerbek.Web.Services;
using Lekkerbek.Web.ViewModel;
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

namespace LekkerbekTestProject.Services
{
    [TestClass]
    public class OrderCahierServiceTests
    {
        private LekkerbekContext _context;
        private OrderCashierService _orderCahierService;

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

            var userManagerMock = new Mock<UserManager<IdentityUser>>(new
            Mock<IUserStore<IdentityUser>>().Object,
            null, null, null, null, null, null, null, null);

            userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync((string userId) => new IdentityUser { Id = userId });

            var user = new Mock<ClaimsPrincipal>();
            user.Setup(x => x.Identity.IsAuthenticated).Returns(true);
            user.Setup(x => x.Identity.Name).Returns("testuser");
            user.Setup(x => x.FindFirst(ClaimTypes.Role)).Returns(new Claim(ClaimTypes.Role,
            "Administrator"));


            _orderCahierService = new OrderCashierService(new OrdersCashierRepository(_context), userManagerMock.Object);
        }
        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
        [TestMethod]
        public async Task GetSpecificOrder_ValidOrderId_ReturnsOrder()
        {
            // Arrange
            var order = new Order { OrderID = 1, Finished = true };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Act
            var result = _orderCahierService.GetSpecificOrder(order.OrderID);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(order.OrderID, result.OrderID);
        }

    }
}
