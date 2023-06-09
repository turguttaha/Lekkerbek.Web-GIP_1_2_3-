using Lekkerbek.Web.Controllers;
using Lekkerbek.Web.Data;
using Lekkerbek.Web.Models;
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

namespace LekkerbekTestProject.Controllers
{
    [TestClass]
    public class RestaurantManagementControllerTests
    {
        private RestaurantManagementController _controller;
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
            var mockRestaurantManagementService = new Mock<IRestaurantManagementService>();

            // Configure mockServices as needed
            var chef = new Chef { ChefId=1,ChefName="testChef" };
            var workerHoliday = new WorkerHoliday {WorkerHolidayId=1, ChefId = 1, StartDate = Convert.ToDateTime("11/11/2000 00:00"), EndDate = Convert.ToDateTime("13/11/2000 00:00") };
            mockRestaurantManagementService.Setup(m => m.GetSpecificWorkerHoliday(1)).Returns(workerHoliday);

            var userManagerMock = new Mock<UserManager<IdentityUser>>(new Mock<IUserStore<IdentityUser>>().Object,
            null, null, null, null, null, null, null, null);
            userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync((string userId) => new IdentityUser { Id = userId });
            // Create the controller instance with the required dependencies

            var user = new Mock<ClaimsPrincipal>();
            user.Setup(x => x.Identity.IsAuthenticated).Returns(true);
            user.Setup(x => x.Identity.Name).Returns("testuser");
            user.Setup(x => x.FindFirst(ClaimTypes.Role)).Returns(new
            Claim(ClaimTypes.Role, "Administrator"));
            _controller = new RestaurantManagementController(mockRestaurantManagementService.Object)
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
        public async Task EditWorkerHoliday_ReturnsViewResultWithWorkerHoliday()
        {
            // Arrange
            // service objects setup in the Initialize method
            try
            {
                // Act
                var result = _controller.EditWorkerHoliday(1);
                // Assert
                Assert.IsInstanceOfType(result, typeof(ViewResult));
                var viewResult = (ViewResult)result;
                // check the viewresult model                
                Assert.IsTrue(viewResult.Model is WorkerHoliday holiday );
                holiday = (WorkerHoliday)viewResult.Model;
                Assert.IsTrue(holiday.WorkerHolidayId == 1&&holiday.ChefId==1);

            }
            catch (AssertFailedException)
            {
                Assert.Fail("The Details result is no ViewResult!");
            }
        }
    }
}
