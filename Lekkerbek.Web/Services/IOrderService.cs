using Lekkerbek.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Lekkerbek.Web.Services
{
    public interface IOrderService
    {
        public List<SelectListItem> GetTimeDropDownList();
        public IEnumerable<Order> Read();
        public Order GetSpecificOrder(int? id);
        public List<OrderLine> FilterOrderLines(int? orderId);
        public SelectList CustomerSelectList(object selectedValue);
        public List<Chef> CheckChefs(DateTime? startTimeSlot);
        public SelectList MenuItemSelectList();
        public MenuItem GetSpecificMenuItem(int? id);
        public void CreateOrder(TimeSlot timeSlot, Order order);
        public TimeSlot GetSpecificTimeSlot(int? id);
    }
}