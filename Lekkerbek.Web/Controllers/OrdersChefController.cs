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

namespace Lekkerbek.Web.Controllers
{
    [Authorize(Roles = "Chef")]
    public class OrdersChefController : Controller
    {
        private readonly IOrderChefService _orderChefService;

        public OrdersChefController(IOrderChefService orderChefService)
        {
            
            _orderChefService = orderChefService;
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
        public ActionResult EditingPopup_Read([DataSourceRequest] DataSourceRequest request)
        {
            var a = Json(_orderChefService.Read().ToDataSourceResult(request));
            return a;//this was the read, to call all orders
        }
        
        // Pay Off page Get: order to pay
        public async Task<IActionResult> OrderDetails(int? id)
        {
            if (id == null || _orderChefService.Read() == null)
            {
                return NotFound();
            }
            //gets the selected order
            var order = _orderChefService.GetChefOrders(id);
            
            //gets the chefs that are able to prepare the selected meal (that don't have to prepare another one at that time)
            //In this function we can later on add looking up if a chef has vacation periods and then remove them if they aren't working
            ViewBag.ChefSelectList = _orderChefService.ChefSelectList(order.StartTimeSlot);
            TempData["OrderIdFromBill"] = id;
            int x = (int)id;
            TempData["OrderID"] = x;


            //filtering orderlines according to orderId
            List<OrderLine> allOrderLines = _orderChefService.OrderLineRead(id);

            List <OrderLine> filteredOrderLines = new List<OrderLine>();

            foreach (var orderLine in allOrderLines.Where(c => c.OrderID == id))
            {
                if (!filteredOrderLines.Contains(orderLine))
                    filteredOrderLines.Add(orderLine);

            }
           
            ViewBag.listOfTheOrder = filteredOrderLines;


            if (order == null)
            {
                return NotFound();
            }
        

            //var orderCount = _context.Orders.Where(c => c.CustomerID == order.CustomerID).ToList();
            
            return View(order);
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
                _orderChefService.UpdateTimeSlot(timeSlot);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ViewData["TimeSlotError"] = "There is an order that needs to be finnished sooner than this one!";
                var order = _orderChefService.GetChefOrders(id);
                ViewBag.ChefSelectList = _orderChefService.ChefSelectList(order.StartTimeSlot);
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


