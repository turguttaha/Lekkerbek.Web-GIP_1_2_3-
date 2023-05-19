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
            if (_chefService.GetTimeSlots().Any(ol => ol.ChefId == chef.ChefId))
            {
                ModelState.AddModelError("Model", "Unable to delete (present in (an) order(s))!");
            }
            else if (chef != null)
            {
                _chefService.Destroy(chef);
            }

            return Json(new[] { chef }.ToDataSourceResult(request, ModelState));
        }

        
        public ActionResult EditingPopup_Create([DataSourceRequest] DataSourceRequest request, ChefViewModel product)
        {
            //ModelState.Remove("TimeSlot");

            if (product != null && ModelState.IsValid)
            {
                _chefService.Create(product);
            }

            return Json(new[] { product }.ToDataSourceResult(request, ModelState));
        }

        
        public ActionResult EditingPopup_Update([DataSourceRequest] DataSourceRequest request, Chef product)
        {
            if (product != null && ModelState.IsValid)
            {
                _chefService.Update(product);
            }

            return Json(new[] { product }.ToDataSourceResult(request, ModelState));
        }

    }
}
