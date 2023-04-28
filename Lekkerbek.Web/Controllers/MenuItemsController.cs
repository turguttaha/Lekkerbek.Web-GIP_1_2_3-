using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lekkerbek.Web.Data;
using Lekkerbek.Web.Models;
using Lekkerbek.Web.ViewModel;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Lekkerbek.Web.Services;
using Microsoft.AspNetCore.Authorization;
using System.Globalization;

namespace Lekkerbek.Web.Controllers
{
    [Authorize(Roles = "Administrator")]

    public class MenuItemsController : Controller
    {
        private readonly IMenuItemService _menuItemService;
        private readonly IOrderService _orderService;
        public MenuItemsController(IMenuItemService menuItemService, IOrderService orderService)
        {
            _menuItemService = menuItemService;
            _orderService = orderService;
        }
        public IActionResult Index()
        {

            return View();
         }
    public IActionResult ReadMenuItems([DataSourceRequest] DataSourceRequest request)
        {
            var menuItems = _menuItemService.Read();
            return Json(menuItems.ToDataSourceResult(request));
        }

        public IActionResult DeleteMenuItem([DataSourceRequest] DataSourceRequest request, Models.MenuItem menuItem)
        {
            ModelState.Remove("Sort");
            if (_orderService.GetOrderLines().Any(ol => ol.MenuItemId == menuItem.MenuItemId))
            {
                ModelState.AddModelError("Model", "Unable to delete (present in (an) order(s))!");
            }
            //else if(menuItem != null)
                else if (menuItem != null)
                    {
                // Delete the item in the data base or follow with the dummy data.

                //_menuItemService.Destroy(menuItem);
                _menuItemService.Destroy(menuItem);
            }


            // Return a collection which contains only the destroyed item.
            //return Json(new[] { menuItem }.ToDataSourceResult(request, ModelState));
            return Json(new[] { menuItem }.ToDataSourceResult(request, ModelState));
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
        public async Task<IActionResult> Create([Bind("MenuItemId,Name,Description,Price,Sort,BtwNumber")] Models.MenuItem menuItem, IFormCollection formDetails)

        {
            ////if (ModelState.IsValid)
            ////{

            //menuItem.Price = double.Parse(formDetails["Price"].ToString().Replace('.',','));//i LOVE when one side of the app works with , as a seperator, and the other with a . :D

            //_menuItemService.Create(menuItem);

            //    return RedirectToAction(nameof(Index));
            ////}
            //return View(menuItem);

            //if (ModelState.IsValid)
            //{

            menuItem.Price = double.Parse(formDetails["Price"].ToString().Replace('.', ','));//i LOVE when one side of the app works with , as a seperator, and the other with a . :D

            _menuItemService.Create(menuItem);

            return RedirectToAction(nameof(Index));
            //}
            return View(menuItem);
        }

        // GET: Dishes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _menuItemService.Read() == null)
            {
                return NotFound();
            }

            var dish = _menuItemService.GetSpecificMenuItem(id);
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
        //public async Task<IActionResult> Edit(int id, [Bind("MenuItemId,Name,Description,Price,Sort,BtwNumber")] Models.MenuItem menuItem, IFormCollection formDetails)
        public async Task<IActionResult> Edit(int id, [Bind("MenuItemId,Name,Description,Price,Sort,BtwNumber")] Models.MenuItem menuItem, IFormCollection formDetails)

        {
            //if (id != menuItem.MenuItemId)
            if (id != menuItem.MenuItemId)
            {
                return NotFound();
            }

           // if (ModelState.IsValid)
          //  {
                try
                {
                menuItem.Price = double.Parse(formDetails["Price"].ToString().Replace('.', ','));
                _menuItemService.Update(menuItem);

            }
                catch (DbUpdateConcurrencyException)
                {
                //if (!_menuItemService.MenuItemExists(menuItem.MenuItemId))
                if (!_menuItemService.MenuItemExists(menuItem.MenuItemId))
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
            //return View(menuItem);
            return View(menuItem);
        }

       
    }
}
