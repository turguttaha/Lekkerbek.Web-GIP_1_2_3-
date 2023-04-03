using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lekkerbek.Web.Data;
using Lekkerbek.Web.Models;

namespace Lekkerbek.Web.Controllers
{
    public class OrderLinesController : Controller
    {
        private readonly LekkerbekContext _context;

        public OrderLinesController(LekkerbekContext context)
        {
            _context = context;
        }


        // GET: OrderLines/Create
        public IActionResult Create(int id)
        {
            ViewData["DishID"] = new SelectList(_context.MenuItems, "DishId", "Name");
            TempData["Orderid"] = id;
            ViewBag.Id = id; 
            return View();
        }

        // POST: OrderLines/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderLineID,ExtraDetails,DishAmount,DishID")] OrderLine orderLine)
        {
            // if (ModelState.IsValid)
            //{
            orderLine.OrderID = (int)TempData["Orderid"];
                _context.Add(orderLine);
                await _context.SaveChangesAsync();
            return RedirectToAction("EditOrder", "Orders", new { id = orderLine.OrderID });
            //}
            ViewData["DishID"] = new SelectList(_context.MenuItems, "DishId", "DishId", orderLine.MenuItemId);
            ViewData["OrderID"] = new SelectList(_context.Orders, "OrderID", "OrderID", orderLine.OrderID);
            return View(orderLine);
        }

        // GET: OrderLines/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.OrderLines == null)
            {
                return NotFound();
            }

            var orderLine = await _context.OrderLines
                .Include(o => o.MenuItem)
                .Include(o => o.Order)
                .FirstOrDefaultAsync(m => m.OrderLineID == id);
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
            if (_context.OrderLines == null)
            {
                return Problem("Entity set 'LekkerbekContext.OrderLines'  is null.");
            }
            var orderLine = await _context.OrderLines.FindAsync(id);
            if (orderLine != null)
            {
                _context.OrderLines.Remove(orderLine);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction("EditOrder", "Orders",new { id = orderLine.OrderID });
        }

        private bool OrderLineExists(int id)
        {
          return _context.OrderLines.Any(e => e.OrderLineID == id);
        }
    }
}
