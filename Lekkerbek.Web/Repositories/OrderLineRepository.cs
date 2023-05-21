using Lekkerbek.Web.Data;
using Lekkerbek.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Lekkerbek.Web.Repositories
{
    public class OrderLineRepository
    {
        private readonly LekkerbekContext _context;

        public OrderLineRepository(LekkerbekContext context)
        {
            _context = context;
        }

        public void AddToDataBase(OrderLine orderLine) 
        {
            _context.Add(orderLine);
            _context.SaveChanges();
        }
        public OrderLine GetOrderLineDetailed(int? id) 
        { 
            return _context.OrderLines.Include(o => o.MenuItem).Include(o => o.Order).FirstOrDefault(m => m.OrderLineID == id);
        }
        public OrderLine GetOrderLine(int? id)
        {
            return _context.OrderLines.FirstOrDefault(m => m.OrderLineID == id);
        }
        public List<Order> GetOrders()
        {
            return _context.Orders.ToList();
        }
        public List<OrderLine> GetOrdersLines()
        {
            return _context.OrderLines.ToList();
        }
        public List<MenuItem> GetMenuItems()
        {
            return _context.MenuItems.ToList();
        }
        public void DeleteFromDataBase(OrderLine entity)
        {

            _context.OrderLines.Remove(entity);
            _context.SaveChanges();
        }
    }
}
