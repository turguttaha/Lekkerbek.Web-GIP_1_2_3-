using Lekkerbek.Web.Data;
using Lekkerbek.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Lekkerbek.Web.Repositories
{
    public class OrdersCashierRepository
    {
        private readonly LekkerbekContext _context;
        public OrdersCashierRepository(LekkerbekContext context)
        {
            _context = context;
        }
        public List<Order> GetOrders()
        {
            //_context.Orders.Find(id);
            return _context.Orders.Select(order => new Order
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

            }).Where(c => c.Finished == false).ToList();

        }

        public Order GetOrder(int? id)
        {
           return _context.Orders.Include("OrderLines.MenuItem").Include("Customer").Where(c => c.OrderID == id).ToList()[0];
        }
        public List<Customer> GetAllCustomers()
        {
            return _context.Customers.Select(customer => new Customer
            //This is another way to make a new object
            {
                CustomerId = customer.CustomerId,
                FName = customer.FName,
                LName = customer.LName,
                PreferredDishId = customer.PreferredDishId,
                PreferredDish = new PreferredDish()
                {
                    PreferredDishId = customer.PreferredDish.PreferredDishId,
                    Name = customer.PreferredDish.Name
                }

            }).ToList();

        }
        public List<PreferredDish> GetAllPrefferedDishes()
        {
            return _context.PreferredDishes.Select(prefDish => new PreferredDish
            //This is another way to make a new object
            {

                PreferredDishId = prefDish.PreferredDishId,
                Name = prefDish.Name


            }).ToList();

        }
        public List<Order> getOrders(int? id)
        {
            return _context.Orders.Select(order => new Order
            {
                OrderID = order.OrderID,
                CustomerId = order.CustomerId
            }).Where(c => c.OrderID == id).ToList();
        }

        public List<OrderLine> getAllOrderLines(int? id)
        {
            return _context.OrderLines.Select(orderLine => new OrderLine
            //This is another way to make a new object
            {
                OrderLineID = orderLine.OrderLineID,
                OrderID = orderLine.OrderID,
                ExtraDetails = orderLine.ExtraDetails,
                DishAmount = orderLine.DishAmount,
                //we don't need order details themselves for this i think
                MenuItem = new MenuItem()
                {
                    MenuItemId = orderLine.MenuItem.MenuItemId,
                    Name = orderLine.MenuItem.Name,
                    Price = orderLine.MenuItem.Price,
                    Description = orderLine.MenuItem.Description,
                    BtwNumber = orderLine.MenuItem.BtwNumber,
                    Sort = orderLine.MenuItem.Sort,
                }

            }).Where(c => c.OrderID == id).ToList();
            
        }
        public void UpdateOrder(Order order)
        {

            _context.Update(order);
            _context.SaveChanges();
        }
        public void UpdateCustomer(Customer customer)
        {
            _context.Update(customer);
            _context.SaveChanges();
        }
    }
}
