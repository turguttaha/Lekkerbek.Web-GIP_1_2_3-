using Lekkerbek.Web.Data;
using Lekkerbek.Web.Migrations;
using Lekkerbek.Web.Models;
using Lekkerbek.Web.ViewModel;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging.Signing;

namespace Lekkerbek.Web.Repositories
{
    public class OrdersChefRepository
    {
        private readonly LekkerbekContext _context;
        public OrdersChefRepository(LekkerbekContext context)
        {
            _context = context;
        }
        public List<Order> GetOrders()
        {
            //_context.Orders.Find(id);
            var a=  _context.Orders.Select(order => new Order
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
                    FName = order.Customer.FName,
                    LName = order.Customer.LName,
                    BtwNumber = order.Customer.BtwNumber,
                    Btw = order.Customer.Btw,
                    City = order.Customer.City,
                    ContactPerson = order.Customer.ContactPerson,
                    Birthday = order.Customer.Birthday,
                    LoyaltyScore = order.Customer.LoyaltyScore,
                    Email = order.Customer.Email,
                    PhoneNumber = order.Customer.PhoneNumber,
                    PreferredDishId = order.Customer.PreferredDishId,

                }

            }).Where(c => c.Finished == false && c.TimeSlot.ChefId == null).ToList();
            return a;
        }

        public List<OrderViewModel> GetOrderViewModels()
        {

            var result = _context.Orders.Select(order => new OrderViewModel
            //This is another way to make a new object
            {
                OrderID = order.OrderID,
                Finished = order.Finished,
                CustomerId = order.CustomerId,
                CustomerName = order.Customer.FName + " " + order.Customer.LName,
                Discount = order.Discount,
                TimeSlot = order.TimeSlot.StartTimeSlot,
                ChefId = order.TimeSlot.ChefId,

            }).Where(c => c.Finished == false && c.ChefId == null).ToList();
            return result;
        }

        public Order GetOrder(int? id)
        {
            return _context.Orders.Include("TimeSlot").Select(order => new Order
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
                

            }).Where(c => c.OrderID == id).ToList()[0];
        }
        public TimeSlot GetTimeSlot(int? id)
        {
            return _context.TimeSlots.Find(id);
        }
        public List<TimeSlot> GetTimeSlots()
        {
            return _context.TimeSlots.ToList();

        }
        public Order GetFirstTimeSlot()
        {
            //&& where finished is false
            var a = _context.Orders.Select(order => new Order
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
                    FName = order.Customer.FName,
                    LName = order.Customer.LName,
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

            }).Where(c => c.Finished == false && c.TimeSlot.ChefId == null).ToList().OrderBy(c=>c.TimeSlot.StartTimeSlot).FirstOrDefault();

            return a;

        }
        public List<Chef> GetChefs()
        {
            var a = _context.Chefs.ToList();
            return a;
        }
        public List<Chef> GetChefsList(List<int> ids)
        {
            return _context.Chefs.Where(r => ids.Contains(r.ChefId)==false).ToList();
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
                CustomerId = order.CustomerId,
                TimeSlot = new TimeSlot()
                {
                    StartTimeSlot = order.TimeSlot.StartTimeSlot,
                    Id = order.TimeSlot.Id
                }
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
                    Description = orderLine.MenuItem.Description
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
        public void UpdateTimeSlot(TimeSlot timeSlot) 
        {
            _context.Update(timeSlot);
            _context.SaveChanges();
        }
    }
}
