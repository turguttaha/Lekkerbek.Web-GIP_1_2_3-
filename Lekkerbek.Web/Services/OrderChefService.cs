using Lekkerbek.Web.Models;
using Lekkerbek.Web.Repositories;
using Lekkerbek.Web.ViewModel;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Lekkerbek.Web.Services
{
    public class OrderChefService : IOrderChefService
    {
        private static bool UpdateDatabase = true;
        private readonly OrdersChefRepository _repository;
        public OrderChefService(OrdersChefRepository ordersChefRepository) 
        { 
            _repository = ordersChefRepository;
        }
        private IList<ChefOrdersViewModel> GetAll() 
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
        public IEnumerable<ChefOrdersViewModel> Read()
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
        public TimeSlot GetTimeSlot(int? id)
        {
            var timeSlot = _repository.GetTimeSlot(id);

            if (timeSlot == null)
            {
                return null;
            }
            else
            {
                return timeSlot;
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

        public ChefOrdersViewModel GetChefOrders(int? id)
        {
            return _repository.GetOrders().Find(c => c.OrderId == id);
             
        }
        public List<Chef> GetAllChefs() 
        {
            return _repository.GetChefs();
        }

        public string ChefAssignOrder(DateTime startTimeSlot, Chef chef)
        {
            //check if there is a timeslot this chef is already working on
            //check if it is the first timeslot
            var allChefId = _repository.GetChefs();
            int chefId = chef.ChefId;


            var usedTimeSlots = _repository.GetTimeSlots().FindAll(t => t.StartTimeSlot == startTimeSlot && t.ChefId==chefId);
            
            List<int> ids = new List<int>();


            

            var firstTimeSlot = _repository.GetFirstTimeSlot();

            if (startTimeSlot != firstTimeSlot.TimeSlot.StartTimeSlot)
            {

                return "Er is een bestelling vroeger die eerst moet klaargemaakt worden!";
                

            }
            else
            {
                if (usedTimeSlots.Count() == 0)
                {
                    return "";
                }
                return "U hebt al een order op dit tijdslot dat u moet klaarmaken!";
            }

            
        }
        public Order GetChefsOrders(int? id) 
        {
            return _repository.GetAllOrders(id);
        }
        public void UpdateTimeSlot(Order order, Chef chef)
        {
            order.TimeSlot.ChefId = chef.ChefId;
            _repository.UpdateOrder(order);
        }

        public bool GetChefTimeSlot(int? id, DateTime startTime)
        {
            throw new NotImplementedException();
            
        }

        public List<OrderViewModel> GetOrderViewModels()
        {
            return _repository.GetOrderViewModels();
        }

        public Order GetFirstTimeSlot()
        {
            var timeSlot = _repository.GetFirstTimeSlot();

            if (timeSlot == null)
            {
                return null;
            }
            else
            {
                return timeSlot;
            }
        }
    }
}
