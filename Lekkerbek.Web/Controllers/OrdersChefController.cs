using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lekkerbek.Web.Data;
using Lekkerbek.Web.Models;
using Microsoft.Data.SqlClient;
using System.Net.Mail;
using System.Net;
using System.IO;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Threading;
using Lekkerbek.Web.Services;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Microsoft.AspNetCore.Authorization;
using Lekkerbek.Web.NewFolder;
using Microsoft.AspNetCore.Identity;

namespace Lekkerbek.Web.Controllers
{
    [Authorize(Roles = "Chef")]
    public class OrdersChefController : Controller
    {
        private readonly IOrderChefService _orderChefService;

        private readonly UserManager<IdentityUser> _userManager;
        private readonly ICustomerService _customerService;
        private readonly IOrderService _orderService;
        private readonly IMenuItemService _menuItemService;

        public OrdersChefController(IOrderChefService orderChefService, ICustomerService customerService, IOrderService orderService, IMenuItemService menuItemService, UserManager<IdentityUser> userManager)
        {
            _orderChefService = orderChefService;
            _customerService = customerService;
            _orderService = orderService;   
            _menuItemService = menuItemService;
            _userManager = userManager;
        }
        // GET: Orders according to finished property
        public IActionResult Index()
        {

            //var orderCashier = _orderCashierService.Read();
            return View();
            //var lekkerBekContext = _orderCashierService.Read();
            //return View();
            //var lekkerbekContext = _context.Orders.Include(o => o.Customer).Include(o => o.TimeSlot).Where(c=>c.Finished==false);
            //return View(await lekkerbekContext.ToListAsync());
        }
        public async Task<ActionResult> DetailTemplate_HierarchyBinding_OrderAsync([DataSourceRequest] DataSourceRequest request)
        {
            //var user = await _userManager.GetUserAsync(User);
            //var customer = _customerService.Read().Where(c => c.IdentityUser == user).FirstOrDefault();
            //return Json(_orderService.FilterOrdersForCustomer(chef.ChefId).ToDataSourceResult(request));
            //return Json(_customerService.GetAllViews().ToDataSourceResult(request));
            return Json(_orderChefService.Read().ToDataSourceResult(request));
        }

        public ActionResult DetailTemplate_HierarchyBinding_Orderline(int orderID, [DataSourceRequest] DataSourceRequest request)
        {
            var a = _orderService.GetOrderLines();

            return Json(_orderService.GetOrderLines()
                .Where(orderline => orderline.OrderID == orderID)
                .ToDataSourceResult(request));
        }
        public ActionResult EditingPopup_Read([DataSourceRequest] DataSourceRequest request)
        {
            return Json(_orderChefService.Read().ToDataSourceResult(request));
        }
        
        // Pay Off page Get: order to pay
        public async Task<IActionResult> OrderDetails(int? id)
        {
            //gets the selected order
            var order = _orderChefService.GetChefsOrders(id);

            //gets the chefs that are able to prepare the selected meal (that don't have to prepare another one at that time)
            //In this function we can later on add looking up if a chef has vacation periods and then remove them if they aren't working
            var chefIdentity = await _userManager.GetUserAsync(User);
            var chef = _orderChefService.GetAllChefs().Where(c=>c.IdentityUser==chefIdentity).FirstOrDefault();

            var chefError = _orderChefService.ChefAssignOrder(order.TimeSlot.StartTimeSlot, chef);
            if (chefError!="")
            {
                TempData["ChefError"] = chefError;
            }
            else 
            {
                _orderChefService.UpdateTimeSlot(order, chef);
            }


            //stay on index page
            return RedirectToAction("Index");
        }
        
        // Pay off page discount func
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OrderDetails(int id, [Bind("OrderID,OrderFinishedTime,Finished,CustomerID,TimeSlotID")] Order orderInfo, IFormCollection collection)
        {
            var orderFinish = _orderChefService.GetSpecificOrder(id);
            var timeSlot = _orderChefService.GetTimeSlot(orderFinish.TimeSlotID);
            var firstTimeSlot = _orderChefService.GetFirstTimeSlot();
            
            
            if (timeSlot.StartTimeSlot == firstTimeSlot.TimeSlot.StartTimeSlot)
            {
                timeSlot.ChefId = int.Parse(Request.Form["ChefSelectList"]);
                //_orderChefService.UpdateTimeSlot(timeSlot);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                var chefIdentity = await _userManager.GetUserAsync(User);
                var chef = _orderChefService.GetAllChefs().Where(c => c.IdentityUser == chefIdentity).FirstOrDefault();
                ViewData["TimeSlotError"] = "There is an order that needs to be finnished sooner than this one!";
                var order = _orderChefService.GetChefOrders(id);
                
                TempData["OrderIdFromBill"] = id;
                int x = (int)id;
                TempData["OrderID"] = x;


                //filtering orderlines according to orderId
                List<OrderLine> allOrderLines = _orderChefService.OrderLineRead(id);

                List<OrderLine> filteredOrderLines = new List<OrderLine>();

                foreach (var orderLine in allOrderLines.Where(c => c.OrderID == id))
                {
                    if (!filteredOrderLines.Contains(orderLine))
                        filteredOrderLines.Add(orderLine);

                }

                ViewBag.listOfTheOrder = filteredOrderLines;
                return View(order);
            }
            //if (ModelState.IsValid)
            //{

            

            //_orderChefService.Update(orderFinish);

        }



        private bool CustomerExists(int id)
        {
            bool exist = false;
            if (_orderChefService.GetSpecificCustomer(id) != null) 
            {
                exist = true;
            }
            return exist;
            //return _context.Customers.Any(e => e.CustomerId == id);
        }
        private bool OrderExists(int id)
        {
            bool exist = false;
            if (_orderChefService.GetSpecificOrder(id) != null)
            {
                exist = true;
            }
            return exist;
            //return _context.Orders.Any(e => e.OrderID == id);
        }
    }
}


