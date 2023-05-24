using Lekkerbek.Web.Areas.Identity.Pages.Account;
using Lekkerbek.Web.Models;
using Lekkerbek.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Encodings.Web;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using Azure.Core;
using Telerik.SvgIcons;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.Collections;
using Lekkerbek.Web.ViewModel;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace Lekkerbek.Web.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminsController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly ICustomerService _customerService;

        public AdminsController(
            UserManager<IdentityUser> userManager,
            IUserStore<IdentityUser> userStore,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            ICustomerService customerService)
        {
            _userManager = userManager;
            _userStore = userStore;
            _signInManager = signInManager;
            _logger = logger;
            _customerService = customerService;
        }



        public IActionResult AdminList()
        {
            
            return View();
        }

        public async Task<IActionResult> Admin_read([DataSourceRequest] DataSourceRequest request)
        {


            IList<IdentityUser> users = await _userManager.GetUsersInRoleAsync("Administrator");

            //ViewData["IdentityUsers"] = new SelectList(users, "Id", "UserName");
            return Json(users.ToDataSourceResult(request));
            //return View(users);
        }
        public IActionResult AssignAdminList()
        {
            return View();
        }
        public async Task<IActionResult> AssignAdmin_read([DataSourceRequest] DataSourceRequest request)
        {

            
            IList<IdentityUser> users = await _userManager.GetUsersInRoleAsync("Customer");
            IList<IdentityUser> newList = new List<IdentityUser>();
            foreach (IdentityUser user in users)
            {
                if(!await _userManager.IsInRoleAsync(user, "Administrator")) { newList.Add(user); }
            }

            //ViewData["IdentityUsers"] = new SelectList(users, "Id", "UserName");
            return Json(newList.ToDataSourceResult(request));
            //return View(users);
        }

        public async Task<IActionResult> AssignAdmin(string id)
        {
            var userAddRole = await _userManager.FindByIdAsync(id);
            await _userManager.AddToRoleAsync(userAddRole, "Administrator");
            //await _userManager.RemoveFromRoleAsync(userAddRole, "Customer");
            return RedirectToAction("AdminList");
        }

        public async Task<ActionResult> Delete_Admin([DataSourceRequest] DataSourceRequest request, IdentityUser admin)
        {

                var user = await _userManager.FindByIdAsync(admin.Id);
                await _userManager.RemoveFromRoleAsync(user, "Administrator");


            return Json(new[] { user }.ToDataSourceResult(request, ModelState));
        }

    }
}
