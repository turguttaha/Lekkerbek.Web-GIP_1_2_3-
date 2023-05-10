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

        public List<SelectListItem> ChefSelectList(DateTime startTimeSlot)
        {

            var usedTimeSlots = _repository.GetTimeSlots().FindAll(t => t.StartTimeSlot == startTimeSlot);

            var allChefId = _repository.GetChefs();

            List<int> ids = new List<int>();


            if (usedTimeSlots.Count() != 0)
            {
                foreach (var currentTimeSlot in usedTimeSlots)
                {
                    if (currentTimeSlot.ChefId != null)
                    {
                        ids.Add((int)currentTimeSlot.ChefId);
                    }

                }
            }
            if (ids.Count() < allChefId.Count())
            {

                List<SelectListItem> item = _repository.GetChefsList(ids).ConvertAll(notWorkingChefs =>
                            {
                                return new SelectListItem()
                                {
                                    Text = notWorkingChefs.ChefName.ToString(),
                                    Value = notWorkingChefs.ChefId.ToString(),
                                    Selected = false
                                };
                            });
                //this will return all the chefs that are still free
                return item;

            }
            else
            {
                return null;
            }

            
        }

        public void UpdateTimeSlot(TimeSlot timeSlot)
        {
            _repository.UpdateTimeSlot(timeSlot);
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
