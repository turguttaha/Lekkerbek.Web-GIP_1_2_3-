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
using Lekkerbek.Web.Areas.Identity.Pages.Account;

namespace Lekkerbek.Web.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class ChefsController : Controller
    {
       
        private readonly ChefService _chefService;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly ILogger<RegisterModel> _logger;
        public ChefsController(ChefService chefService, 
            SignInManager<IdentityUser> signInManager, 
            UserManager<IdentityUser> userManager,
            IUserStore<IdentityUser> userStore,
            ILogger<RegisterModel> logger
            )
        {
            _chefService = chefService;
            _signInManager = signInManager;
            _userManager = userManager;
            _userStore = userStore;
            _logger = logger;
        }

        // GET: Chefs
        public async Task<IActionResult> Index()
        {
              return View();
        }
        public IActionResult ReadChefs([DataSourceRequest] DataSourceRequest request)
        {
            var chefs = _chefService.GetChefsWithIdentity();
            return Json(chefs.ToDataSourceResult(request));
        }
        public async Task<ActionResult> DeleteChefAsync([DataSourceRequest] DataSourceRequest request, ChefViewModel chef)
        {
            //can't delete
            if (_chefService.GetTimeSlots().Any(ol => ol.ChefId == chef.ChefId))
            {
                ModelState.AddModelError("Model", "Unable to delete (present in (an) order(s))!");
            }
            else if (chef != null)
            {
                var user = await _userManager.FindByIdAsync(chef.IdentityId);
                await _userManager.RemoveFromRoleAsync(user, "Chef");
                Chef chef1 = new Chef()
                {
                    ChefId = chef.ChefId,
                    ChefName = chef.ChefName,
                    IdentityUser = user,
                };
                _chefService.Destroy(chef1);
            }

            return Json(new[] { chef }.ToDataSourceResult(request, ModelState));
        }

        public IActionResult AssignChefList()
        {
            return View();
        }
        public async Task<IActionResult> AssignChef_read([DataSourceRequest] DataSourceRequest request)
        {

            IList<IdentityUser> users = await _userManager.GetUsersInRoleAsync("Customer");
            IList<IdentityUser> newList = new List<IdentityUser>();
            foreach (IdentityUser user in users)
            {
                if (!await _userManager.IsInRoleAsync(user, "Chef")) { newList.Add(user); }
            }
  
            return Json(newList.ToDataSourceResult(request));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignChef([Bind("IdentityId,ChefName,Email")] ChefViewModel chef)
        {
           
            if (ModelState.IsValid)
            {
                var userAddRole = await _userManager.FindByIdAsync(chef.IdentityId);
                await _userManager.AddToRoleAsync(userAddRole, "Chef");
                //await _userManager.RemoveFromRoleAsync(userAddRole, "Customer");
                _chefService.Create(chef);
                return RedirectToAction("Index");
            }
            return RedirectToAction("AssignChefList");
        }
        
        public async Task<ActionResult> EditingPopup_Update([DataSourceRequest] DataSourceRequest request, ChefViewModel product)
        {
            if (product != null && ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(product.IdentityId);
                var entity = new Chef();

                entity.ChefId = product.ChefId;
                entity.ChefName = product.ChefName;
                entity.IdentityUser = user;

                _chefService.Update(entity);
            }

            return Json(new[] { product }.ToDataSourceResult(request, ModelState));
        }

    }
}
