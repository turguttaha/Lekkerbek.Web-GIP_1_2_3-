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
    public class ChefHolidaysTests
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
        public async Task AddHolidayDay_ValidData_Success()
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


            var newHolidayDay = new RestaurantHoliday
            {
                StartDate = Convert.ToDateTime("11/11/2000 00:00"),
                EndDate = Convert.ToDateTime("12/12/2000 00:00"),
                Description = "xyz"
            };
            // Act
            var result = restaurantManagementController.CreateHolidayDay(newHolidayDay);
            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirectResult = (RedirectToActionResult)result;
            Assert.AreEqual("HolidayDays", redirectResult.ActionName);
            // Verify that the newCustomer object is created in the DbContext
            var savedHolidayDay = await _context.RestaurantHolidays.FirstOrDefaultAsync(c =>
           c.Description == "xyz");
            Assert.IsNotNull(savedHolidayDay);
            Assert.AreEqual(Convert.ToDateTime("11/11/2000 00:00"), savedHolidayDay.StartDate);
        }
    } 
}