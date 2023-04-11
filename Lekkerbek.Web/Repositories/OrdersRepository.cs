using Lekkerbek.Web.Data;
using Lekkerbek.Web.Migrations;
using Lekkerbek.Web.Models;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging.Signing;
using NuGet.Protocol.Core.Types;

namespace Lekkerbek.Web.Repositories
{
    public class OrdersRepository
    {
        private readonly LekkerbekContext _context;

        public OrdersRepository(LekkerbekContext context)
        {
            _context = context;
        }

        public List<Order> GetOrders()
        {
            var result = _context.Orders.Select(order => new Order
            //This is another way to make a new object
            {
                OrderID = order.OrderID,
                Finished = order.Finished,
                CustomerId = order.CustomerId,
                Discount = order.Discount,
                TimeSlotID = order.TimeSlotID,
                TimeSlot = new TimeSlot()
                {
                    StartTimeSlot = order.TimeSlot.StartTimeSlot,
                    ChefId = order.TimeSlot.ChefId,
                    Id = order.TimeSlot.Id
                },
                Customer = new Customer()
                {
                    CustomerId = order.Customer.CustomerId,
                    FName = order.Customer.Name,
                    LName = order.Customer.Name,
                    BtwNumber = order.Customer.BtwNumber,
                    Btw = order.Customer.Btw,
                    City = order.Customer.City,
                    ContactPerson = order.Customer.ContactPerson,
                    //Address = order.Customer.Address,
                    Birthday = order.Customer.Birthday,
                    LoyaltyScore = order.Customer.LoyaltyScore,
                    Email = order.Customer.Email,
                    PhoneNumber = order.Customer.PhoneNumber,
                    PreferredDishId = order.Customer.PreferredDishId,

                }


            }).ToList();

            return result;
            // return _context.Orders.Include(o => o.Customer).Include(o => o.TimeSlot).ToList();

        }
        public List<OrderLine> GetOrderLines()
        {
            return _context.OrderLines.Include(o=>o.MenuItem).ToList();

        }
        public List<Customer> GetCustomers()
        {
            return _context.Customers.Include(o=>o.PreferredDish).ToList();
        }

        public List<TimeSlot> GetTimeSlots()
        {
            return _context.TimeSlots.Include(o=>o.Chef).ToList();
        }

        public DbSet<Chef> GetChefs()
        {
            var a = _context.Chefs;



            return a;
        }

        public List<MenuItem> GetMenuItems()
        {
            return _context.MenuItems.ToList();
        }

        public void CreateOrder(TimeSlot timeSlot, Order order) 
        {
            _context.Add(timeSlot);
            _context.SaveChanges();

            var lastTimeSlot = _context.TimeSlots.OrderByDescending(t => t.Id).FirstOrDefault();
            order.TimeSlotID = lastTimeSlot.Id;
            _context.Add(order);
            _context.SaveChanges();

            var lastOrder = _context.Orders.OrderByDescending(t => t.OrderID).FirstOrDefault();
            foreach (OrderLine item in Order.TemproraryCart.ToList())
            {
                item.MenuItem = null;
                item.OrderID = lastOrder.OrderID;
                _context.Add(item);
                _context.SaveChanges();
                Order.TemproraryCart.Remove(item);
            }

        }
        public void UpdateOrder(TimeSlot timeSlot, Order order)
        {
            _context.Update(timeSlot);
            _context.SaveChanges();

            var lastTimeSlot = _context.TimeSlots.OrderByDescending(t => t.Id).FirstOrDefault();

            order.TimeSlotID = lastTimeSlot.Id;
            _context.Update(order);
            _context.SaveChanges();
        }
        public void DeleteOrder(Order order, TimeSlot timeSlot, List<OrderLine> filteredOrderLines)
        {
            _context.Orders.Attach(order);
            _context.Orders.Remove(order);
            _context.TimeSlots.Remove(timeSlot);
            foreach (var orderLine in filteredOrderLines) { _context.OrderLines.Remove(orderLine); }
            _context.SaveChanges();
        }
        public void UpdateOrderLine(OrderLine orderLine)
        {
            _context.Update(orderLine);
            _context.SaveChanges();
        }

    }
}
