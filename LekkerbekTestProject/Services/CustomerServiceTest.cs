using Lekkerbek.Web.Models;
using Lekkerbek.Web.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

namespace LekkerbekTestProject
{
        [TestClass]
        public class CustomerServiceTests
        {
            private LekkerbekContext _context;
            private CustomerService _customerService;
            [TestInitialize]
            public void Setup()
            {
                var connection = new SqliteConnection("DataSource=:memory:");
                connection.Open();
                var options = new DbContextOptionsBuilder<LekkerbekContext>()
                .UseSqlite(connection)
                .Options;
                _context = new LekkerbekContext(options);
                _context.Database.EnsureCreated();
                _customerService = new CustomerService(new CustomersRepository (_context));
            }
            [TestCleanup]
            public void Cleanup()
            {
                _context.Database.EnsureDeleted();
                _context.Dispose();
            }
            [TestMethod]
            public async Task CreateCustomer_Success()
            {
                // Arrange
                var customer = new Customer { CustomerId = 1, FName = "Test" };
                // Act
                 _customerService.Create(customer);
                // Assert
var createdCustomer =  _context.Customers.FirstOrDefault(o =>
o.CustomerId == customer.CustomerId);
                Assert.IsNotNull(createdCustomer);
                // Additional assertions on the created customer properties
            }
            //[TestMethod]
            //[ExpectedException(typeof(Exception))]
            ////public async Task CreateCustomer_InvalidData()
            //{
            //    // Arrange
            //    var invalidCustomer = new Customer { };
            //    // Act
            //   _customerService.Create(invalidCustomer);
            //    // Assert
            //    // Exception is expected to be thrown
            //}
            //[TestMethod]
            //public async Task GetCustomerById_Success()
            //{
            //    // Arrange
            //    var customer = new Customer { CustomerId = 1, FName = "Test" };
            //    _context.Customers.Add(customer);
            //    await _context.SaveChangesAsync();
            //    // Act
            //    var retrievedCustomer = 
            //    _customerService.GetSpecificCustomer(customer.CustomerId);
            //    // Assert
            //    Assert.IsNotNull(retrievedCustomer);
            //    Assert.AreEqual(customer.CustomerId, retrievedCustomer.CustomerId);
            //}
            [TestMethod]
            public async Task GetCustomerById_CustomerNotFound()
            {
                // Arrange
                var nonExistentCustomerId = 123;
                // Act
                var retrievedCustomer = 
                _customerService.GetSpecificCustomer(nonExistentCustomerId);
                // Assert
                Assert.IsNull(retrievedCustomer);
            }
            [TestMethod]
            public async Task UpdateCustomer_Success()
            {
                // Arrange
                var customer = new Customer { CustomerId = 1, FName = "No name??" };
                _context.Customers.Add(customer);
                 _context.SaveChangesAsync();
                customer.FName = "Name updated!";
                // Act
             _customerService.Update(customer);
                // Assert
                var retrievedCustomer = await _context.Customers.FirstOrDefaultAsync(o =>
                o.CustomerId == customer.CustomerId);
                Assert.IsNotNull(retrievedCustomer);
                Assert.AreEqual(customer.FName, "Name updated!");
            }
        }
    }
        
