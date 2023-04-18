using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Lekkerbek.Web.Models;
using Lekkerbek.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lekkerbek.Web.Controllers
{
	public class HierarchyController : Controller
	{
		private readonly ICustomerService _customerService;
		private readonly IOrderService _orderService;
		private readonly IMenuItemService _menuItemService;

		public HierarchyController(ICustomerService customerService, IOrderService orderService, IMenuItemService menuItemService)
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
	}
}
