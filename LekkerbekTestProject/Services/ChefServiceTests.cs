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
    public class ChefServiceTests
    {
        private LekkerbekContext _context;
        private ChefService _chefService;

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

            //var mockChefRepository = new Mock<ChefRepository>();

            _chefService = new ChefService(new ChefRepository(_context), userManagerMock.Object);
        }
        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [TestMethod]
        public async Task CreateChef_Success()
        {
            // Arrange
            var chef = new ChefViewModel { ChefId = 1, ChefName = "Test" };
            // Act
             _chefService.Create(chef);
            // Assert
            var createdCustomer = await _context.Chefs.FirstOrDefaultAsync(o =>
            o.ChefId == chef.ChefId);
            Assert.IsNotNull(createdCustomer);
            // Additional assertions on the created customer properties
        }
    }
}
