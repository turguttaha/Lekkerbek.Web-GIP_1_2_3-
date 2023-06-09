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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LekkerbekTestProject.Integraties
{
    [TestClass]
    public class RestaurantOpeninghoursTests
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
            var options = new DbContextOptionsBuilder<LekkerbekContext>()
            .UseSqlite(connection)
            .Options;
            _context = new LekkerbekContext(options);

            // Create the database schema
            _context.Database.EnsureCreated();

            // User manager
            var userManagerMock = new Mock<UserManager<IdentityUser>>(new
            Mock<IUserStore<IdentityUser>>().Object,
            null, null, null, null, null, null, null, null);

            userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync((string userId) => new IdentityUser { Id = userId });

            // Create the controller instance with the required dependencies
            var user = new Mock<ClaimsPrincipal>();
            user.Setup(x => x.Identity.IsAuthenticated).Returns(true);
            user.Setup(x => x.Identity.Name).Returns("testuser");
            user.Setup(x => x.FindFirst(ClaimTypes.Role)).Returns(new Claim(ClaimTypes.Role,
            "Administrator"));

            _userManager = userManagerMock.Object;
            _user = user.Object;


        }
        [TestCleanup]
        public void Cleanup()
        {
            _context.Dispose();
        }
        [TestMethod]
        public async Task AddOpeningHour_ValidData_Success()
        {
            // Arrange
            var restaurantManagementController = new RestaurantManagementController(new
RestaurantManagementService(new RestaurantManagementRepository(_context)))
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = _user }
                }
            };


            var newOpeningsHour = new RestaurantOpeninghours
            {
                StartTime = Convert.ToDateTime("11/11/2000 00:00"),
                EndTime = Convert.ToDateTime("12/12/2000 00:00"),
                DayOfWeek = DayOfWeekEnum.Monday,
            };
            // Act
            var result = restaurantManagementController.CreateOpeningsHour(newOpeningsHour);
            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirectResult = (RedirectToActionResult)result;
            Assert.AreEqual("OpeningsHours", redirectResult.ActionName);
            // Verify that the newCustomer object is created in the DbContext
            var savedOpeningsHour = await _context.RestaurantOpeningHours.FirstOrDefaultAsync(c =>
           c.DayOfWeek == DayOfWeekEnum.Monday);
            Assert.IsNotNull(savedOpeningsHour);
            Assert.AreEqual(Convert.ToDateTime("11/11/2000 00:00"), savedOpeningsHour.StartTime);
        }
    }

}

