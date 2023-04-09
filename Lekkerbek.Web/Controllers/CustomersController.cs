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

namespace Lekkerbek.Web.Controllers
{
    public class CustomersController : Controller
    {
        private readonly ICustomerService _customerService;

        public CustomersController( ICustomerService customerService)
        {
            _customerService = customerService;
        }

        public ActionResult Index()
        {
          
            return View();
        }

        public ActionResult EditingPopup_Read([DataSourceRequest] DataSourceRequest request)
        {
            return Json(_customerService.Read().ToDataSourceResult(request));
        }
        // pop-up Create - update
        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult EditingPopup_Create([DataSourceRequest] DataSourceRequest request, Customer customer)
        //{
        //    if (customer != null && ModelState.IsValid)
        //    {
        //        _productService.Create(customer);
        //    }

        //    return Json(new[] { customer }.ToDataSourceResult(request, ModelState));
        //}

        // [AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult EditingPopup_Update([DataSourceRequest] DataSourceRequest request, Customer customer)
        //{
        //    if (customer != null && ModelState.IsValid)
        //    {
        //       // _productService.Update(product);
        //    }

        //    return Json(new[] { customer }.ToDataSourceResult(request, ModelState));
        //}

        // [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EditingPopup_Destroy([DataSourceRequest] DataSourceRequest request, Customer customer)

        {
            //if (_context.Orders.Any(ol => ol.CustomerId == customer.CustomerId))
            //{
            //    return View("NoDelete", customer);
            //}
            if (customer != null)
            {
                _customerService.Destroy(customer);
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
        public async Task<IActionResult> Create([Bind("FName,LName,Email,PhoneNumber,Address,Birthday,PreferredDishId")] Customer customer)
        {
            //I put this in the comment. Because ModelState.IsValid is checking if all values are populated. But we do not fill the id value, it is added in dabase.
            //if (ModelState.IsValid)
            //{
            if(customer != null)
            {
                _customerService.Create(customer);
                return RedirectToAction(nameof(Index));
            }
            //}
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
        public async Task<IActionResult> Edit(int id, [Bind("CustomerId,FName,LName,Email,PhoneNumber,Address,Birthday,PreferredDishId")] Customer customer)
        {
            if (id != customer.CustomerId)
            {
                return NotFound();
            }

            //if (ModelState.IsValid)
            // {
            //I put this in the comment. Because ModelState.IsValid is checking if all values are populated. But we do not fill the id value, it is added in dabase.
            if(customer != null)
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
            
            //}
            ViewData["PreferredDishId"] = _customerService.GetPreferredDishes(customer);
            return View(customer);
        }

        
    }
}
