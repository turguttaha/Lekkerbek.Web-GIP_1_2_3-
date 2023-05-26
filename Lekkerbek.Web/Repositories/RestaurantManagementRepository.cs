using Lekkerbek.Web.Data;
using Lekkerbek.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Lekkerbek.Web.Repositories
{
    public class RestaurantManagementRepository
    {
        private readonly LekkerbekContext _context;

        public RestaurantManagementRepository(LekkerbekContext context)
        {
            _context = context;
        }

        public void AddToDatabaseOpeningsHour(RestaurantOpeninghours entity)
        {
            _context.Add(entity);
            _context.SaveChanges();
        }

        public List<RestaurantOpeninghours> GetRestaurantOpeninghours()
        {
            return _context.RestaurantOpeningHours.ToList();
        }

        public void DeleteFromDatabaseOpeningsHour(RestaurantOpeninghours entity) {
            _context.RestaurantOpeningHours.Remove(entity);
            _context.SaveChanges();
        }
        public void UpdateDatabaseOpeningsHour(RestaurantOpeninghours entity)
        {
            _context.Update(entity);
            _context.SaveChanges();
        }



        public List<WorkerHoliday> GetWorkerHoliday()
        {
            return _context.WorkerHolidays.ToList();
        }
        public List<WorkerHoliday> GetAllWorkerHoliday(int id)
        {
            return _context.WorkerHolidays.Where(c=>c.ChefId!=id).ToList();
        }
        public List<WorkerHoliday> GetWorkerHolidayFromWorker(int? id)
        {
            return _context.WorkerHolidays.Where(c=>c.ChefId==id).ToList();
        }
        public void AddWorkerHoliday(WorkerHoliday entity)
        {
            _context.Add(entity);
            _context.SaveChanges();
        }
        public void DeleteWorkerHolliday(WorkerHoliday entity)
        {
            _context.WorkerHolidays.Remove(entity);
            _context.SaveChanges();
        }
        public void UpdateWorkerHoliday(WorkerHoliday entity)
        {
            _context.WorkerHolidays.Update(entity);
            _context.SaveChanges();
        }
        public List<Chef> GetChefs()
        {
            return _context.Chefs.ToList();
        }
        public List<Order> GetAllOrders(DateTime startDate, DateTime endDate) 
        {
           
            return _context.Orders.Include("TimeSlot").Where(c => c.TimeSlot.StartTimeSlot.Date >= startDate.Date && c.TimeSlot.StartTimeSlot.Date <= endDate.Date).ToList();
        }
        public List<Order> GetAllOrders(DateTime startDate)
        {

            return _context.Orders.Include("TimeSlot").Where(c => c.TimeSlot.StartTimeSlot >= startDate).ToList();
        }

        //HOLIDAY/////
        public void AddToDatabaseHolidayDay(RestaurantHoliday entity)
        {
            _context.Add(entity);
            _context.SaveChanges();
        }

        public List<RestaurantHoliday> GetRestaurantHoliday()
        {
            return _context.RestaurantHolidays.ToList();
        }

        public void DeleteFromDatabaseHolidayDay(RestaurantHoliday entity)
        {
            _context.RestaurantHolidays.Remove(entity);
            _context.SaveChanges();
        }

        public void UpdateDatabaseHolidayDay(RestaurantHoliday entity)
        {
            _context.RestaurantHolidays.Update(entity);
            _context.SaveChanges();
        }






    }
}
