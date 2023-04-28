using Lekkerbek.Web.Models;
using Lekkerbek.Web.ViewModel;

namespace Lekkerbek.Web.Services
{
    public interface IOrderCashierService
    {
        public IEnumerable<Order> Read();
        public void Update(Order order);
        public void UpdateCustomer(Customer customer);
        public Order GetSpecificOrder(int? id);
        public List<OrderLine> OrderLineRead(int? id);
        public List<Order> GetOrders(int? id);
        public List<Customer> GetAllCustomers();
        public Customer GetSpecificCustomer(int? id);
        public List<PreferredDish> GetAllPrefferedDishes();
        public IEnumerable<PreferredDish> ReadPrefferedDish();
        public bool OrderExists(int id);
        public List<OrderViewModel> GetOrderViewModels();
    }
}
