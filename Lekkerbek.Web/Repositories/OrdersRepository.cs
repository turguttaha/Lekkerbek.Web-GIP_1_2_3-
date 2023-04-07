﻿using Lekkerbek.Web.Data;
using Lekkerbek.Web.Migrations;
using Lekkerbek.Web.Models;
using Microsoft.EntityFrameworkCore;
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
            return _context.Orders.Include(o => o.Customer).Include(o => o.TimeSlot).ToList();

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

        public List<Chef> GetChefs()
        {
            return _context.Chefs.ToList();
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
