using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Lekkerbek.Web.Models;
using Lekkerbek.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lekkerbek.Web.Controllers
{
    public class OrderModuleController : Controller
    {

        private readonly ICustomerService _customerService;
        private readonly IOrderService _orderService;
        private readonly IMenuItemService _menuItemService;

        public OrderModuleController(ICustomerService customerService, IOrderService orderService, IMenuItemService menuItemService)
        {
            _customerService = customerService;
            _orderService = orderService;
            _menuItemService = menuItemService;
        }
        public ActionResult Hierarchy()
        {
            return View();
        }

        public ActionResult HierarchyBinding_Order([DataSourceRequest] DataSourceRequest request)
        {
            return Json(_orderService.Read().ToDataSourceResult(request));//sends order to view
        }

        public ActionResult HierarchyBinding_Orderline(int orderID, [DataSourceRequest] DataSourceRequest request)
        {
            return Json(_orderService.GetOrderLines() //sends orderlist to view
                .Where(orderline => orderline.OrderID == orderID)
                .ToDataSourceResult(request));
        }
        public ActionResult DetailTemplate()
        {
            return View();
        }

        public ActionResult DetailTemplate_HierarchyBinding_Order([DataSourceRequest] DataSourceRequest request)
        {
            return Json(_orderService.Read().ToDataSourceResult(request));
        }

        public ActionResult DetailTemplate_HierarchyBinding_Orderline(int orderID, [DataSourceRequest] DataSourceRequest request)
        {
            return Json(_orderService.GetOrderLines()
                .Where(orderline => orderline.OrderID == orderID)
                .ToDataSourceResult(request));
        }
        public IActionResult Index()
        {
            return View();
        }

        //public ActionResult EditingPopup_Read(int? id,[DataSourceRequest] DataSourceRequest request)
        //{
            
        //    return Json(_orderService.FilterOrdersForCustomer(id).ToDataSourceResult(request));
        //}

        //// GET: Customers/Create
        //public IActionResult Create()
        //{

        //    ViewData["PreferredDishId"] = _customerService.GetPreferredDishes();
        //    return View();
        //}

        //// POST: Customers/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("FName,LName,Email,PhoneNumber,Address,Birthday,PreferredDishId")] Customer customer)
        //{
        //    //I put this in the comment. Because ModelState.IsValid is checking if all values are populated. But we do not fill the id value, it is added in dabase.
        //    //if (ModelState.IsValid)
        //    //{
        //    if (customer != null)
        //    {
        //        _customerService.Create(customer);
        //        return RedirectToAction(nameof(Index));
        //    }
        //    //}
        //    ViewData["PreferredDishId"] = _customerService.GetPreferredDishes(customer);
        //    return View(customer);
        //}

        // GET: Customers/Edit/5

        public async Task<IActionResult> EditCustomer(int? id)
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
        public async Task<IActionResult> EditCustomer(int id, [Bind("CustomerId,FName,LName,Email,PhoneNumber,Address,Birthday,PreferredDishId")] Customer customer)
        {
            if (id != customer.CustomerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
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
            ViewData["PreferredDishId"] = _customerService.GetPreferredDishes(customer);
            return View(customer);
        }

        public IActionResult MenuItemList()
        {

            return View();
        }
        public IActionResult ReadMenuItems([DataSourceRequest] DataSourceRequest request)
        {
            var menuItems = _menuItemService.Read();
            return Json(menuItems.ToDataSourceResult(request));
        }

        // POST: OrderLines/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

       public async Task<JsonResult> AddOrderLine(string menuItemId, string menuItemAmount, string extraDetails)
        {
            OrderLine orderLine = new OrderLine();
            orderLine.MenuItemId = int.Parse(menuItemId);
            orderLine.DishAmount = int.Parse(menuItemAmount);
            orderLine.ExtraDetails = extraDetails;

            orderLine.MenuItem = _orderService.GetSpecificMenuItem(orderLine.MenuItemId);

            Order.TemproraryCart.Add(orderLine);
            

            ViewBag.TemproraryCart = Order.TemproraryCart;

            //ViewData["DishID"] = _orderService.MenuItemSelectList();
            //ModelState.Clear();
            return Json(new { status = "Your Menu Item is Added!"});
            // go to database and get dish name via id
            //orderLine.MenuItem = _orderService.GetSpecificMenuItem(orderLine.MenuItemId);
            //Order.TemproraryCart.Add(orderLine);

            //ViewData["Message"] = "Your Dish is added";

        }

        public IActionResult CompleteOrder(int? id)
        {
            // here we can add temporary Cart list to send it to view

            ViewBag.TimeSlotsSelectList = _orderService.GetTimeDropDownList();
            var customer = _customerService.GetSpecificCustomer(id);
            TempData["SelectedCustomerId"] = customer.CustomerId;
            return View(customer);
        }
        public async Task<IActionResult> CompleteOrder(string date, string time)
        {
            //it might be IFormCollection???
            string x = time;
            String selectedDate = date + " " + x;
            DateTime timeSlotDateAndTime = Convert.ToDateTime(selectedDate);
            //TimeSlot Object aanmaken

            TimeSlot timeSlot = new TimeSlot();
            timeSlot.StartTimeSlot = (DateTime)TempData["SelectedDateTime"]; // add from view via ajax
            //timeSlot.ChefId =(int)TempData["SelectedChef"];

            Order order = new Order();
            order.CustomerId = (int)TempData["SelectedCustomerId"];

            _orderService.CreateOrder(timeSlot, order);
            return RedirectToAction(nameof(Index));
        }

    }
}
