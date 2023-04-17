using Lekkerbek.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using NuGet.Protocol.Core.Types;

namespace Lekkerbek.Web.Services
{
    public interface IOrderService
    {
        public List<SelectListItem> GetTimeDropDownList(DateTime askDateTime);
        public IEnumerable<Order> Read();
        public Order GetSpecificOrder(int? id);
        public List<OrderLine> FilterOrderLines(int? orderId);
        public SelectList CustomerSelectList();
        public SelectList CustomerSelectList(object selectedValue);
        public IQueryable<Chef> CheckChefs(DateTime? startTimeSlot);
        public SelectList MenuItemSelectList();
        public MenuItem GetSpecificMenuItem(int? id);
        public void CreateOrder(TimeSlot timeSlot, Order order);
        public TimeSlot GetSpecificTimeSlot(int? id);
        public void UpdateOrder(TimeSlot timeSlot, Order order);
        public bool OrderExists(int id);
        public List<OrderLine> GetOrderLines();
        public OrderLine GetSpecificOrderLine(int? id);
        public void UpdateOrderLine(OrderLine orderLine);
        public bool OrderLineExists(int id);
        public bool DeleteOrder(Order order);
        public Customer GetSpecificCustomer(int? id);
    }
}