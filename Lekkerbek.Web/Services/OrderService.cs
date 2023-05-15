using Lekkerbek.Web.Models;
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
        public OrderService(OrdersRepository repository)
        {
            _repository = repository;
        }
       
        // it is list of the Time Slot for dropdown list
        private readonly List<SelectListItem> TimeSlotsSelectList = new List<SelectListItem>() {
                new SelectListItem {
                    Text = "12:00", Value = "12:00"
                },
                new SelectListItem {
                    Text = "12:15", Value = "12:15"
                },
                new SelectListItem {
                    Text = "12:30", Value = "12:30"
                },
                new SelectListItem {
                    Text = "12:45", Value = "12:45"
                },
                new SelectListItem {
                    Text = "13:00", Value = "13:00"
                },
                new SelectListItem {
                    Text = "13:15", Value = "13:15"
                },
                new SelectListItem {
                    Text = "13:30", Value = "13:30"
                },
                new SelectListItem {
                    Text = "13:45", Value = "13:45"
                },
                new SelectListItem {
                    Text = "18:00", Value = "18:00"
                },
                new SelectListItem {
                    Text = "18:15", Value = "18:15"
                },
                new SelectListItem {
                    Text = "18:30", Value = "18:30"
                },
                new SelectListItem {
                    Text = "18:45", Value = "18:45"
                },
                new SelectListItem {
                    Text = "19:00", Value = "19:00"
                },
                new SelectListItem {
                    Text = "19:15", Value = "19:15"
                },
                new SelectListItem {
                    Text = "19:30", Value = "19:30"
                },
                new SelectListItem {
                    Text = "19:45", Value = "19:45"
                },
                new SelectListItem {
                    Text = "20:00", Value = "20:00"
                },
                new SelectListItem {
                    Text = "20:15", Value = "20:15"
                },
                new SelectListItem {
                    Text = "20:30", Value = "20:30"
                },
                new SelectListItem {
                    Text = "20:45", Value = "20:45"
                },
                new SelectListItem {
                    Text = "21:00", Value = "21:00"
                },
                new SelectListItem {
                    Text = "21:15", Value = "21:15"
                },
                new SelectListItem {
                    Text = "21:30", Value = "21:30"
                },
                new SelectListItem {
                    Text = "21:45", Value = "21:45"
                },
            };
        public List<SelectListItem> GetTimeDropDownList(DateTime askDateTime)
        {
           /*
           Check if restaurant is open
           var getHolidays = repos.getHolidays
           ->check here if any record contains values startDate<=askDate<=endDate
           --->return error if TRUE
            */
            
            
            DateTime startTime = Convert.ToDateTime("01/01/2020 12:00");
            DateTime endTime = Convert.ToDateTime("01/01/2020 14:00");


            List<SelectListItem> timeSlotSelectListNew = new List<SelectListItem>();
                        
            /*
             * This will return the openinghours themselves of the restaurant
             * 
            string selectedDayOfWeek = askDateTime.DayOfWeek;
            var object = _repos.GetOpeningsHours(selectedDayOfWeek);
            foreach(var item in object)
            {
                while (item.startTime < item.endTime) 
                {
                    SelectListItem newSelectListItem = new SelectListItem {Text = item.startTime.TimeOfDay.ToString("HH:mm"), Value=itemstartTime.TimeOfDay.ToString() };
                    timeSlotSelectListNew.Add(newSelectListItem);
                    item.startTime = startTime.AddMinutes(15);
                }
            }
             
             */
            while (startTime < endTime) 
            {
                SelectListItem newSelectListItem = new SelectListItem {Text = startTime.TimeOfDay.ToString("HH:mm"), Value=startTime.TimeOfDay.ToString() };
                timeSlotSelectListNew.Add(newSelectListItem);
                startTime = startTime.AddMinutes(15);
            }
            Console.WriteLine(startTime.DayOfWeek);
            //filter out the timeslots that are already fully booked

            //gets the timeslot of a specific day
            List<TimeSlot> timeSlotOfADay = _repository.GetUsedTimeSlots(askDateTime);
            List<SelectListItem> timeSlotSelectList = TimeSlotsSelectList;
            List<SelectListItem> tempTimeSlotSelectList = TimeSlotsSelectList;
            
            //the chefcount will change depending on if they work on that day or not
            //var allWorkersSchedule = getWorkersSchedules(dayOfWeek)
            //var chefHollidays = getChefHollidays(askdate)
            //->check if any chefs are on holliday on this day, if so remove from original list(workerschedules)
            /*
             foreach(var item in chefHolliday)
            {
                allWorkerSchedules.remove(item);
            }
             */

            int chefCount = _repository.GetChefs().Count();
            //new list based on db data
            foreach (var item in timeSlotSelectList.ToList())
            {
                /*
                 * this will replace the chefCount int, this will get for the time that its looping through, the amount of chefs availible, we can then
                 * Check how many orders are being prepared, 

                int workingChefsOnTime = allWorkersSchedule.Where(c=>c.startTime<=item &&c.endTime>=item).Count();
                if (timeSlotOfADay.Where(c => c.StartTimeSlot.ToString("HH:mm") == item.Value).Count() == workingChefsOnTime)
                {
                    tempTimeSlotSelectList.Remove(item);
                }
                 */


                if (timeSlotOfADay.Where(c => c.StartTimeSlot.ToString("HH:mm") == item.Value).Count() == chefCount)
                {
                    tempTimeSlotSelectList.Remove(item);
                }
            }

            return tempTimeSlotSelectList;
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
            return new SelectList(_repository.GetCustomers(), "CustomerId", "FName");
        }
        public SelectList CustomerSelectList(object? selectedValue)
        {
                return new SelectList(_repository.GetCustomers(), "CustomerId", "FName", selectedValue);
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
