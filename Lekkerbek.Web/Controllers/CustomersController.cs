using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lekkerbek.Web.Data;
using Lekkerbek.Web.Models;
using System.ComponentModel.DataAnnotations;
using Lekkerbek.Web.NewFolder;
using Lekkerbek.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Telerik.SvgIcons;
using Lekkerbek.Web.ViewModel;
using System.Globalization;

namespace Lekkerbek.Web.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class CustomersController : Controller
    {
        private UserManager<IdentityUser> _userManager;

        private readonly ICustomerService _customerService;
        private readonly IOrderService _orderService;

        public CustomersController(ICustomerService customerService, IOrderService orderService, UserManager<IdentityUser> userManager)
        {
            _customerService = customerService;
            _orderService = orderService;
            _userManager = userManager;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DetailTemplate_HierarchyBinding_Customers([DataSourceRequest] DataSourceRequest request)
        {
            return Json(_customerService.GetAllViews().ToDataSourceResult(request));
        }

        public ActionResult DetailTemplate_HierarchyBinding_Details(int customerId, [DataSourceRequest] DataSourceRequest request)
        {
            return Json(_customerService.GetAllViews()
                .Where(customer => customer.CustomerId == customerId)
                .ToDataSourceResult(request));
        }

        public async Task<ActionResult> EditingPopup_DestroyAsync([DataSourceRequest] DataSourceRequest request, CustomerViewModel customer)
        {

            if (_orderService.Read().Any(ol => ol.CustomerId == customer.CustomerId))
            {
                ModelState.AddModelError("Model", "Kan niet verwijderen (aanwezig in (een) bestelling(en))!");
            }
            else if (customer != null)
            {
                var user = await _userManager.FindByEmailAsync(customer.Email);
                _customerService.Destroy(customer);
                if (user!=null)
                {
                    var result = await _userManager.DeleteAsync(user);
                }
                
            }

            return Json(new[] { customer }.ToDataSourceResult(request, ModelState));
        }


        // GET: Customers/Create
        public IActionResult Create()
        {

            ViewData["PreferredDishId"] = _customerService.GetPreferredDishes();
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FName,LName,Email,PhoneNumber,FirmName,ContactPerson,StreetName,City,PostalCode,Btw,BtwNumber,Birthday,PreferredDishId")] Customer customer)
        {
            if (ModelState.IsValid)
            {
            if (customer != null)
            {
                _customerService.Create(customer);
                return RedirectToAction(nameof(Index));
            }
            }
            ViewData["PreferredDishId"] = _customerService.GetPreferredDishes(customer);
            return View(customer);
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _customerService.Read() == null)
            {
                return NotFound();
            }

            var customer = _customerService.GetSpecificCustomer(id);

            if (customer == null)
            {
                return NotFound();
            }
            ViewData["PreferredDishId"] = _customerService.GetPreferredDishes(customer);
            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CustomerId,FName,LName,Email,PhoneNumber,FirmName,ContactPerson,StreetName,City,PostalCode,Btw,BtwNumber,Birthday,PreferredDishId")] Customer customer)
        {
            if (id != customer.CustomerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
             {
            if (customer != null)
            {
                try
                {
                    _customerService.Update(customer);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_customerService.CustomerExists(customer.CustomerId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            }
            ViewData["PreferredDishId"] = _customerService.GetPreferredDishes(customer);
            return View(customer);
        }


    }
}
