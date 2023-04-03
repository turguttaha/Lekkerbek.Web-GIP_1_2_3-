using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lekkerbek.Web.Data;
using Lekkerbek.Web.Models;
using Azure.Core;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace Lekkerbek.Web.Controllers
{
    public class MenuItemsController : Controller
    {
        private readonly LekkerbekContext _context;
        public MenuItemsController(LekkerbekContext context)
        {
            _context = context;
        }
        public IActionResult ReadMenuItems([DataSourceRequest] DataSourceRequest request)
        {
            //var MenuItems = _context.MenuItems.ToList();
            var menuItems = _context.MenuItems.Select(item => new Models.MenuItem
            //This is another way to make a new object
            {
                MenuItemId = item.MenuItemId,
                Price = item.Price,
                Description = item.Description,
                Name = item.Name,
            }).ToList();
            return Json(menuItems.ToDataSourceResult(request));
        }

        public IActionResult DeleteMenuItem([DataSourceRequest] DataSourceRequest request, Models.MenuItem menuItem)
        {
            //if (_context.OrderLines.Any(ol => ol.MenuItemId == menuItem.MenuItemId))
            //{
            //    return RedirectToAction("NoDelete", "Orders", menuItem);
            //}

            // Delete the item in the data base or follow with the dummy data.
            _context.MenuItems.Remove(menuItem);
            _context.SaveChanges();

            // Return a collection which contains only the destroyed item.
            return Json(new[] { menuItem }.ToDataSourceResult(request));
        }

    // GET: Dishes
    public async Task<IActionResult> Index()
        {
            var list = await _context.MenuItems.ToListAsync();
            return View(await _context.MenuItems.ToListAsync());
        }

        // GET: Dishes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.MenuItems == null)
            {
                return NotFound();
            }

            var dish = await _context.MenuItems
                .FirstOrDefaultAsync(m => m.MenuItemId == id);
            if (dish == null)
            {
                return NotFound();
            }

            return View(dish);
        }

        // GET: Dishes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Dishes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DishId,Name,Description,Price")] Models.MenuItem dish)
        {
            //if (ModelState.IsValid)
            //{
                _context.Add(dish);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            //}
            return View(dish);
        }

        // GET: Dishes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.MenuItems == null)
            {
                return NotFound();
            }

            var dish = await _context.MenuItems.FindAsync(id);
            if (dish == null)
            {
                return NotFound();
            }
            return View(dish);
        }

        // POST: Dishes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MenuItemId,Name,Description,Price")] Models.MenuItem menuItem)
        {
            if (id != menuItem.MenuItemId)
            {
                return NotFound();
            }

           // if (ModelState.IsValid)
          //  {
                try
                {
                    _context.Update(menuItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MenuItemExists(menuItem.MenuItemId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
          //  }
            return View(menuItem);
        }

        //// GET: Dishes/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null || _context.MenuItems == null)
        //    {
        //        return NotFound();
        //    }

        //    var menuItem = await _context.MenuItems
        //        .FirstOrDefaultAsync(m => m.MenuItemId == id);
        //    if (_context.OrderLines.Any(ol => ol.MenuItemId == menuItem.MenuItemId))
        //    {
        //        return RedirectToAction("NoDelete", "Orders", menuItem);
        //    }
        //    if (menuItem == null)
        //    {
        //        return NotFound();
        //    }


        //    return View(menuItem);
        //}

        // POST: Dishes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.MenuItems == null)
            {
                return Problem("Entity set 'LekkerbekContext.Dishes'  is null.");
            }

            var dish = await _context.MenuItems.FindAsync(id);
            if (dish != null)
            {
                _context.MenuItems.Remove(dish);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MenuItemExists(int id)
        {
          return _context.MenuItems.Any(e => e.MenuItemId == id);
        }
    }
}
