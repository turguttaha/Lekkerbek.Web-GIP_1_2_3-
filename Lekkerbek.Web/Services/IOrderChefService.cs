using Lekkerbek.Web.Models;
using Lekkerbek.Web.ViewModel;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Lekkerbek.Web.Services
{
    public interface IOrderChefService
    {
        public IEnumerable<ChefOrdersViewModel> Read();
        public void Update(Order order);
        public void UpdateCustomer(Customer customer);
        public void UpdateTimeSlot(Order order, Chef chef);
        public Order GetSpecificOrder(int? id);
        public TimeSlot GetTimeSlot(int? id);
        public Order GetFirstTimeSlot();
        public bool GetChefTimeSlot(int? id, DateTime startTime);
        public List<OrderLine> OrderLineRead(int? id);
        public List<Order> GetOrders(int? id);
        public ChefOrdersViewModel GetChefOrders(int? id);
        public List<Customer> GetAllCustomers();
        public Customer GetSpecificCustomer(int? id);
        public List<Chef> GetAllChefs();
        public List<PreferredDish> GetAllPrefferedDishes();
        public IEnumerable<PreferredDish> ReadPrefferedDish();
        public bool OrderExists(int id);
        public string ChefAssignOrder(DateTime startTimeSlot, Chef chef);
        public List<OrderViewModel> GetOrderViewModels();
        public Order GetChefsOrders(int? id);
    }
}
