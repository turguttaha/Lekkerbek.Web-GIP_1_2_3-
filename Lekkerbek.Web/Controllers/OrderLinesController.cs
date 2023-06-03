using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lekkerbek.Web.Data;
using Lekkerbek.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Lekkerbek.Web.Services;
using Lekkerbek.Web.NewFolder;
using Microsoft.AspNetCore.Identity;

namespace Lekkerbek.Web.Controllers
{
    [Authorize(Roles = "Administrator, Customer")]

    public class OrderLinesController : Controller
    {
        
        private readonly OrderLineService _orderLineService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ICustomerService _customerService;
        private readonly IOrderService _orderService;

        public OrderLinesController(OrderLineService orderLineService, UserManager<IdentityUser> userManager, ICustomerService customerService, IOrderService orderService)
        {

            _orderLineService = orderLineService;
            _userManager = userManager;
            _customerService = customerService;
            _orderService = orderService;
        }
        private async Task<Customer> GetCustomerAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            var customer = _customerService.Read().Where(c => c.IdentityUser == user).FirstOrDefault();
            return customer;
        }

        // GET: OrderLines/Create
        public async Task<IActionResult> Create(int id)
        {
            if (User.IsInRole("Customer"))
            {
                Order order = _orderService.GetSpecificOrder(id);
                Customer customer = await GetCustomerAsync();
                if (order.CustomerId != customer.CustomerId) { return NotFound(); }
            }

            ViewData["DishID"] = new SelectList(_orderLineService.GetMenuItems(), "MenuItemId", "Name");
            TempData["Orderid"] = id;
            ViewBag.Id = id; 
            return View();
        }

        // POST: OrderLines/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderLineID,ExtraDetails,DishAmount,MenuItemId")] OrderLine orderLine)
        {
            // if (ModelState.IsValid)
            //{
            orderLine.OrderID = (int)TempData["Orderid"];
            _orderLineService.AddOrderLine(orderLine);
                
            if(User.IsInRole("Administrator"))
            return RedirectToAction("EditOrder", "Orders", new { id = orderLine.OrderID });
            if (User.IsInRole("Customer"))
            return RedirectToAction("EditOrder", "OrderModule", new { id = orderLine.OrderID });
            //}
            ViewData["DishID"] = new SelectList(_orderLineService.GetMenuItems(), "DishId", "DishId", orderLine.MenuItemId);
            ViewData["OrderID"] = new SelectList(_orderLineService.GetOrders(), "OrderID", "OrderID", orderLine.OrderID);
            return View(orderLine);
        }

        // GET: OrderLines/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _orderLineService.GetOrdersLines() == null)
            {
                return NotFound();
            }
            var orderLine = _orderLineService.GetSpecificOrderLineDetailed(id);

            if (User.IsInRole("Customer"))
            {
                Customer customer = await GetCustomerAsync();
                if (orderLine == null || orderLine.Order.CustomerId != customer.CustomerId)
                {
                    return NotFound();
                }
            }        
                
            if (orderLine == null)
            {
                return NotFound();
            }

            return View(orderLine);
        }

        // POST: OrderLines/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_orderLineService.GetOrdersLines() == null)
            {
                return Problem("Entity set 'LekkerbekContext.OrderLines'  is null.");
            }
            var orderLine = _orderLineService.GetSpecificOrderLineDetailed(id);
            if (User.IsInRole("Customer"))
            {
                Customer customer = await GetCustomerAsync();
                if (orderLine == null || orderLine.Order.CustomerId != customer.CustomerId)
                {
                    return NotFound();
                }
                if (orderLine != null)
                {
                    _orderLineService.RemoveOrderLine(orderLine);
                }
                return RedirectToAction("EditOrder", "OrderModule", new { id = orderLine.OrderID });
            }
            else if (User.IsInRole("Administrator"))
            {
                if (orderLine != null)
                {
                    _orderLineService.RemoveOrderLine(orderLine);
                }
                return RedirectToAction("EditOrder", "Orders", new { id = orderLine.OrderID });
            }
                           
            return View(orderLine);
        }

        
    }
}
