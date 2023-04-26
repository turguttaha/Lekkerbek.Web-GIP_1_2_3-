﻿using System;
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

namespace Lekkerbek.Web.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class ChefsController : Controller
    {
       
        private readonly ChefService _chefService;
        public ChefsController(ChefService chefService)
        {
            _chefService = chefService;
        }

        // GET: Chefs
        public async Task<IActionResult> Index()
        {
              return View();
        }
        public IActionResult ReadChefs([DataSourceRequest] DataSourceRequest request)
        {
            var chefs = _chefService.Read();
            return Json(chefs.ToDataSourceResult(request));
        }
        public ActionResult DeleteChef([DataSourceRequest] DataSourceRequest request, Chef chef)

        {
            //can't delete
            if (_chefService.Read().Any(ol => ol.ChefId == chef.ChefId))
            {
                ModelState.AddModelError("Model", "Unable to delete (present in (an) order(s))!");
            }
            else if (chef != null)
            {
                _chefService.Destroy(chef);
            }

            return Json(new[] { chef }.ToDataSourceResult(request, ModelState));
        }
        // GET: Chefs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ChefId,ChefName")] Chef chef)
        {
            //I put this in the comment. Because ModelState.IsValid is checking if all values are populated. But we do not fill the id value, it is added in dabase.
            //if (ModelState.IsValid)
            //{
            if (chef != null)
            {
                _chefService.Create(chef);
                return RedirectToAction(nameof(Index));
            }
            //}
            return View(chef);
        }

    }
}
