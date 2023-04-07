using Lekkerbek.Web.Data;
using Lekkerbek.Web.Models;

namespace Lekkerbek.Web.Repository
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
            return _context.Orders.Select(order => new Order
            //This is another way to make a new object
            {
                OrderID = order.OrderID,
                Finished = order.Finished,
                CustomerID = order.CustomerID,
                Discount = order.Discount,
                Customer = order.Customer,
                OrderLines = order.OrderLines,
                TimeSlot = new TimeSlot()
                {
                    Id = order.TimeSlot.Id,
                    StartTimeSlot = order.TimeSlot.StartTimeSlot,
                    Chef = new Chef() 
                    { 
                        ChefId = order.TimeSlot.Chef.ChefId,
                        ChefName = order.TimeSlot.Chef.ChefName
                        
                    }
                }

            }).Where(c=>c.Finished==false).ToList();

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
                CustomerID = order.CustomerID
            }).Where(c=> c.CustomerID == id).ToList();
        }

        public List<OrderLine> getAllOrderLines(int id) 
        {
            return _context.OrderLines.Select(orderLine => new OrderLine
            //This is another way to make a new object
            {
                OrderLineID = orderLine.OrderLineID,
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

            }).Where(c => c.OrderID==id).ToList();
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
