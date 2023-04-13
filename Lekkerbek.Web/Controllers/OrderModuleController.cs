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

        public OrderModuleController(ICustomerService customerService, IOrderService orderService)
        {
            _customerService = customerService;
            _orderService = orderService;
        }

        public IActionResult Index()
        {
            return View();
        }

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

        // POST: OrderLines/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
       
       public async Task<JsonResult> AddOrderLine(string id)
        {
            OrderLine orderLine = new OrderLine();
            orderLine.MenuItemId = int.Parse(id);
            orderLine.DishAmount = 
            orderLine.ExtraDetails=

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
    }
}
