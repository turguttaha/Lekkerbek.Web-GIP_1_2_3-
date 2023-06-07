using Lekkerbek.Web.Controllers;
using Lekkerbek.Web.Data;
using Lekkerbek.Web.Models;
using Lekkerbek.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LekkerbekTestProject.Controllers
{
    [TestClass]
    public class OrdersCashierControllerTest
    {
        private OrdersCashierController _controller;
        private Mock<IOrderCashierService> _orderCashierService;
        private Mock<ICustomerService> _customerService;
        private LekkerbekContext _context;


        [TestInitialize]
        public void TestInitialize()
        {
            _orderCashierService = new Mock<IOrderCashierService>();
            _customerService = new Mock<ICustomerService>();
            _controller = new OrdersCashierController(_orderCashierService.Object, _customerService.Object);
        }

        [TestMethod]
        //Return a ViewResult when the Index method is called
        public void Index_ReturnsViewResult()
        {
            // Act
            var result = _controller.Index();
            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        //in case id is null.
        public async Task EditCustomer_ReturnsNotFound_ForNullId()
        {
            // Arrange
            int? id = null;

            // Act
            var result = await _controller.EditCustomer(id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        //The case where the customer ID is not the same as the id received as a parameter.
        public async Task EditCustomer_IdMismatch_ReturnsNotFound()
        {
            // Arrange
            var customer = new Customer { CustomerId = 1 };
            // Act
            var result = await _controller.EditCustomer(2, customer);
            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestCleanup]
        public void TestCleanup()
        {
            //_context.Database.EnsureDeleted();
            _controller.Dispose();

        }
    }
}
