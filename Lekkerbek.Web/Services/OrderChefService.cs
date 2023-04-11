using Lekkerbek.Web.Models;
using Lekkerbek.Web.Repositories;

namespace Lekkerbek.Web.Services
{
    public class OrderChefService : IOrderChefService
    {
        private static bool UpdateDatabase = true;
        private readonly OrdersCashierRepository _repository;
        public OrderChefService(OrdersCashierRepository ordersCashierRepository) 
        { 
            _repository = ordersCashierRepository;
        }
        private IList<Order> GetAll() 
        { 
            var result = _repository.GetOrders();
            return result;
        }
        public IEnumerable<Customer> ReadCustomers() 
        { 
            return GetAllCustomers();
        }
        private IList<Customer> GetAllCustomers()
        {
            var result = _repository.GetAllCustomers();
            return result;
        }
        public IEnumerable<Order> Read()
        {
            return GetAll();
        }
        public List<PreferredDish> GetAllPrefferedDishes()
        {
            var result = _repository.GetAllPrefferedDishes();
            return result;
        }
        public IEnumerable<PreferredDish> ReadPrefferedDish() 
        {
            return GetAllPrefferedDishes();
        }
      

        private List<OrderLine> getOrderLines(int? id) 
        {
            var result = _repository.getAllOrderLines(id);
            return result;
        }
        public List<OrderLine> OrderLineRead(int? id)
        {
            return getOrderLines(id);
        }
        
        List<Order> IOrderChefService.GetOrders(int? id)
        {
            var result = _repository.getOrders(id);
            return result;
        }

        public void Update(Order order) 
        {
            _repository.UpdateOrder(order);
        }
        public void UpdateCustomer(Customer customer)
        {
            _repository.UpdateCustomer(customer);
        }
        public Customer GetSpecificCustomer(int? id)
        {
            var customer = _repository.GetAllCustomers().Find(x => x.CustomerId == id);
            if (customer == null)
            {
                return null;
            }
            else
            {
                return customer;
            }
        }

        public Order GetSpecificOrder(int? id)
        {
            //var order = _repository.GetOrders().Find(x => x.OrderID == id);
            var order = _repository.GetOrder(id);

            if (order == null)
            {
                return null;
            }
            else 
            {
                return order;
            }
        }

        public bool OrderExists(int id)
        {
            return _repository.GetOrders().Any(e=>e.CustomerId == id);
        }

        
        

        List<Customer> IOrderChefService.GetAllCustomers()
        {
            throw new NotImplementedException();
        }

        public Order GetChefOrders(int? id)
        {
            return _repository.GetOrders().Find(c => c.OrderID == id);
             
        }
    }
}
