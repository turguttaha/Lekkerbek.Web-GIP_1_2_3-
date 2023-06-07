using Lekkerbek.Web.Models;
using Lekkerbek.Web.NewFolder;
using Lekkerbek.Web.Repositories;
using Lekkerbek.Web.ViewModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Telerik.SvgIcons;

namespace Lekkerbek.Web.Services
{
    public class OrderService : IOrderService
    {
        


        private readonly OrdersRepository _repository;
        private readonly ICustomerService _customerService;
        public OrderService(OrdersRepository repository, ICustomerService customerService)
        {
            _repository = repository;
            _customerService = customerService;
        }
        public bool IsRestaurantClosed(DateTime askDateTime) 
        {
            var closingDays = _repository.GetRestaurantHolliday();
            foreach (var item in closingDays)
            {
                if (item.StartDate <= askDateTime && askDateTime <= item.EndDate)
                {
                    
                    return true;

                }

            }
            int selectedDayOfWeek = (int)((DayOfWeek)Enum.Parse(typeof(DayOfWeekEnum), askDateTime.DayOfWeek.ToString()));
            var openingsHours = _repository.GetOpeningsHours(selectedDayOfWeek);
            if (openingsHours == null||openingsHours.Count()==0) 
            {
                return true;
            }
            return false;
        }


       
        public List<SelectListItem> GetTimeDropDownList(DateTime askDateTime)
        {
            List<SelectListItem> timeSlotSelectListNew = new List<SelectListItem>();
                        
            //gets the day of the week from the day we want to look at
            //after, it will get the restaurants schedule for this day of the week
            int selectedDayOfWeek = (int)((DayOfWeek)Enum.Parse(typeof(DayOfWeekEnum), askDateTime.DayOfWeek.ToString())); 
            var openingsHours = _repository.GetOpeningsHours(selectedDayOfWeek);
            
            //for every entry, meaning from and till a certain time, on this day that the restaurant is open, this loop will create
            //timeslots that will be shown to the customer when creating their order
            foreach(var item in openingsHours)
            {
                while (item.StartTime < item.EndTime) 
                {
                    SelectListItem newSelectListItem = new SelectListItem {Text = item.StartTime.TimeOfDay.ToString(), Value=item.StartTime.TimeOfDay.ToString() };
                    timeSlotSelectListNew.Add(newSelectListItem);
                    item.StartTime = item.StartTime.AddMinutes(15);
                }
            }
            
            
            //filter out the timeslots that are already fully booked
            //gets the timeslot of a specific day
            List<TimeSlot> timeSlotOfADay = _repository.GetUsedTimeSlots(askDateTime);
            //We make a copy of the timeslotslist so we can remove the used timeslots
            List<SelectListItem> tempTimeSlotSelectList = timeSlotSelectListNew;
            
            //getting the chefHollidays
            var chefHollidays = _repository.GetChefsHollidays();
            //gets the amount of chefs of the restaurant
            int chefCount = _repository.GetChefs().Count();
            //this loop will subtract the amount of chefs from the chefcount(meaning only the chefs that are working are accounted for)
            foreach (var item in chefHollidays)
            {
                if (item.StartDate <= askDateTime && askDateTime <= item.EndDate) 
                {
                    chefCount--;
                    
                }
                
            }
            DateTime dateTime= DateTime.Now;
            //we loop through all of the timeslots
            foreach (var item in timeSlotSelectListNew.ToList())
            {
                //if the amount of orders on this time is equal to the amount of chefs, then its fully booked, remove from the timeslotlist             
                if (timeSlotOfADay.Where(c => c.StartTimeSlot.ToString("HH:mm:ss") == item.Value).Count() == chefCount)
                {
                    tempTimeSlotSelectList.Remove(item);
                }
                else if (Convert.ToDateTime(askDateTime.Date.ToString("dd/MM/yyyy")+" " + item.Value) < dateTime) 
                {
                    tempTimeSlotSelectList.Remove(item);
                }
                               
            }
            
            return tempTimeSlotSelectList;
        }

        public IList<OrderViewModel> GetAllOrderViews()
        {
            return _repository.GetOrdersViews();
        }
        private IList<Order> GetAll()
        {

            var result = _repository.GetOrders();

            return result;
        }
        public IEnumerable<Order> Read()
        {
            return GetAll();
        }
        public Order GetSpecificOrder(int? id)
        {
            var order = _repository.GetOrders().Find(x => x.OrderID == id);
            if (order == null)
                return null;
            else
                return order;
        }
        public IEnumerable<OrderViewModel> FilterOrdersForCustomer(int? customerId)
        {
            var orders = _repository.GetOrderViewModels().FindAll(x=>x.CustomerId == customerId);
            return orders;
        }
        public List<OrderLine> GetOrderLines()
        {
            return _repository.GetOrderLines();
        }
        public List<OrderLine> GetOrderLinesMenuItem()
        {
            return _repository.GetOrderLinesMenuItem();
        }
        public OrderLine GetSpecificOrderLine(int? id)
        {
            return _repository.GetOrderLines().Find(o=>o.OrderLineID == id);
        }
        public List<OrderLine> FilterOrderLines(int? orderId)
        {
            if (orderId == null)
            {
                return new List<OrderLine>();
            }
            //filtering orderlines occording to orderId
            List<OrderLine> allOrderLines = _repository.GetOrderLinesMenuItem();
            List<OrderLine> filteredOrderLines = new List<OrderLine>();
            foreach (var orderLine in allOrderLines.Where(c => c.OrderID == orderId))
            {
                if (!filteredOrderLines.Contains(orderLine))
                {
                    //orderLine.MenuItem = _context.MenuItems.Find(orderLine.MenuItemId);
                    filteredOrderLines.Add(orderLine);
                }

            }
            return filteredOrderLines;
        }
        public SelectList CustomerSelectList()
        {
            var customerListWithView = _customerService.GetAllViews();
            return new SelectList(customerListWithView, "CustomerId", "Name");
        }
        public SelectList CustomerSelectList(object? selectedValue)
        {
            var customerListWithView = _customerService.GetAllViews();
            return new SelectList(customerListWithView, "CustomerId", "Name", selectedValue);
        }
        public SelectList MenuItemSelectList()
        {
            return new SelectList(_repository.GetMenuItems(), "MenuItemId", "Name");
        }
        public MenuItem GetSpecificMenuItem(int? id)
        {
            return _repository.GetMenuItems().Find(o => o.MenuItemId == id);

        }
        public TimeSlot GetSpecificTimeSlot(int? id)
        {
            return _repository.GetTimeSlots().Find(o => o.Id == id);

        }
        ////public IQueryable<Chef> CheckChefs(DateTime? startTimeSlot)
        ////{
            
        ////    //return _repository.GetTimeSlots().FindAll(t => t.StartTimeSlot == startTimeSlot);


        ////    var usedTimeSlots = _repository.GetTimeSlots().FindAll(t => t.StartTimeSlot == startTimeSlot);

        ////    var allChefId = _repository.GetChefs();

        ////    /*
        ////    Well this is the code, the check is just "is the count of the timeslots on this specific day and time lower than the amount of chefs"
        ////    Depending on how we want it, we can or keep this button with a "check feature" so the person who is looking for a timeslot can 
        ////    we can write functions where we look for the amount of chefs that have vacation and subtract that number from allchefId, because that number is the "free chefs"
            
        ////     I will keep the rest of the code up so the rest can still be tested, orders right now can be made if you just comment out "TempData["SelectedChef"] = int.Parse(collection["ChefId"]);"
        ////    this line in selectTimeSlots on line 166
        ////     */
        ////    if (usedTimeSlots.Count() < allChefId.Count())
        ////    {
        ////        Console.WriteLine("Timeslots can be used");
        ////    }


        ////    List<int> ids = new List<int>();


        ////    if (usedTimeSlots.Count() != 0)
        ////    {
        ////        foreach (var test in usedTimeSlots)
        ////        {
        ////            if (test.ChefId != null)
        ////            {
        ////                ids.Add((int)test.ChefId);
        ////            }

        ////        }
        ////    }
        ////    if (ids.Count() < allChefId.Count())
        ////    {

        ////        var a = _repository.GetChefs().Where(r => ids.Contains(r.ChefId) == false);
                   
        ////        return a;

        ////    }
        ////    else
        ////    {
        ////       return  null;
        ////    }
        ////}
        public void CreateOrder(TimeSlot timeSlot, Order order)
        {
            _repository.CreateOrder(timeSlot, order);
        }
        public void UpdateOrder(TimeSlot timeSlot, Order order)
        {
            _repository.UpdateOrder(timeSlot, order);
        }
        public bool OrderExists(int id)
        {
            return _repository.GetOrders().Any(e => e.OrderID == id);
        }
        public void UpdateOrderLine(OrderLine orderLine)
        {
            _repository.UpdateOrderLine(orderLine);
        }
        public bool OrderLineExists(int id)
        {
            return _repository.GetOrderLines().Any(e => e.OrderLineID == id);
        }
        public bool DeleteOrder(Order order)
        {

                int x = (int)order.CustomerId;
                order.Customer = GetSpecificCustomer(x);
                //var timeSlot = GetSpecificTimeSlot(order.TimeSlotID);
                var timeSlot = order.TimeSlot;
                DateTime startTimeSlot = timeSlot.StartTimeSlot;
                DateTime endTimeSlot = startTimeSlot.AddMinutes(15);
                DateTime now = DateTime.Now;
                DateTime twoHoursAgo = endTimeSlot.AddMinutes(-120);

                List<OrderLine> filteredOrderLines = FilterOrderLines(order.OrderID);

                if (twoHoursAgo > now)
                {

                    //deleting order timeslot orderlines from database
                    if (timeSlot != null && filteredOrderLines != null)
                    {
                        _repository.DeleteOrder(order, timeSlot, filteredOrderLines);
                        return true;
                    }
                   
                }
                return false;
                
        }

        public List<OrderViewModel> GetOrderViewModels()
        {
           return _repository.GetOrderViewModels();
        }

        public Customer GetSpecificCustomer(int? id)
        {
           return _repository.GetCustomers().Find(c=> c.CustomerId == id);
        }

    }
}
