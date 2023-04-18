using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Lekkerbek.Web.Models;
using Lekkerbek.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lekkerbek.Web.Controllers
{
	public class DetailTemplateController : Controller
	{
		private readonly ICustomerService _customerService;
		private readonly IOrderService _orderService;
		private readonly IMenuItemService _menuItemService;

		public DetailTemplateController(ICustomerService customerService, IOrderService orderService, IMenuItemService menuItemService)
		{
			_customerService = customerService;
			_orderService = orderService;
			_menuItemService = menuItemService;
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
	}
}
