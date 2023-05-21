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

namespace Lekkerbek.Web.Controllers
{
    [Authorize(Roles = "Administrator, Customer")]

    public class OrderLinesController : Controller
    {
        
        private readonly OrderLineService _orderLineService;

        public OrderLinesController(OrderLineService orderLineService)
        {
           
            _orderLineService = orderLineService;
        }


        // GET: OrderLines/Create
        public IActionResult Create(int id)
        {
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
            var orderLine = _orderLineService.GetSpecificOrderLine(id);
            if (orderLine != null)
            {
                _orderLineService.RemoveOrderLine(orderLine);
            }
            
           
            
            if (User.IsInRole("Administrator"))
                return RedirectToAction("EditOrder", "Orders",new { id = orderLine.OrderID });
            if (User.IsInRole("Customer"))
                return RedirectToAction("EditOrder", "OrderModule", new { id = orderLine.OrderID });
            return View(orderLine);
        }

        
    }
}
