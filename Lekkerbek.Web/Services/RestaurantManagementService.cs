using Lekkerbek.Web.Models;
using Lekkerbek.Web.Repositories;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Lekkerbek.Web.Services
{
    public class RestaurantManagementService
    {
        private readonly RestaurantManagementRepository _repository;
        public RestaurantManagementService(RestaurantManagementRepository repository)
        {
            _repository = repository;
        }

        public void CreateOpeningsHour(RestaurantOpeninghours restaurantOpeninghours)
        {
            _repository.AddToDatabaseOpeningsHour(restaurantOpeninghours);
        }

        public List<RestaurantOpeninghours> GetAllOpeningsHours()
        {
            return _repository.GetRestaurantOpeninghours();
        }

        public void DestroyOpeningsHour(RestaurantOpeninghours openingHours)
        {
            _repository.DeleteFromDatabaseOpeningsHour(openingHours);
        }
        public RestaurantOpeninghours GetSpecificOpeningsHour(int id)
        {
            RestaurantOpeninghours entity = GetAllOpeningsHours().Find(x => x.RestaurantOpeninghoursId == id);
            return entity;
        }
        public void UpdateOpeningsHour(RestaurantOpeninghours openingHours)
        {
            _repository.UpdateDatabaseOpeningsHour(openingHours);
        }

        //chefs
        public List<WorkerHoliday> GetAllWorkerHolidays()
        {
            DateTime dateTimeNow = DateTime.Now;
            return _repository.GetWorkerHoliday().Where(c=>c.StartDate>=dateTimeNow && c.EndDate>=dateTimeNow).ToList();
        }
        public List<WorkerHoliday> GetAllHolidays()
        {
            DateTime dateTimeNow = DateTime.Now;
            return _repository.GetWorkerHoliday().ToList();
        }
        public List<WorkerHoliday> GetAllWokerHolidays(int id)
        {
            DateTime dateTimeNow = DateTime.Now;
            return _repository.GetAllWorkerHoliday(id).ToList();
        }
        public List<WorkerHoliday> GetAllHolidaysFromAWorker(int? id)
        {
            DateTime dateTimeNow = DateTime.Now;
            return _repository.GetWorkerHolidayFromWorker(id).ToList();
        }
        public void CreateWorkerHoliday(WorkerHoliday workerHoliday)
        {
            _repository.AddWorkerHoliday(workerHoliday);
        }
        public void DestroyWorkerHoliday(WorkerHoliday holidayDays)
        {
            _repository.DeleteWorkerHolliday(holidayDays);
        }
        public WorkerHoliday GetSpecificWorkerHoliday(int id)
        {
            WorkerHoliday entity = GetAllHolidays().Find(x => x.WorkerHolidayId == id);
            return entity;
        }
        public void UpdateWorkerHoliday(WorkerHoliday workerHoliday)
        {
            _repository.UpdateWorkerHoliday(workerHoliday);
        }
        public List<Chef> GetChefs() 
        { 
        
            return _repository.GetChefs();
        }
        public SelectList ChefsSelectList()
        {
            return new SelectList(_repository.GetChefs(), "ChefId", "ChefName");
        }
        public List<Order> GetAllOrders(DateTime startDate, DateTime endDate)
        {
            return _repository.GetAllOrders(startDate, endDate);
        }
        public List<DateTime> GetDateTimeRange(DateTime startDate, DateTime endDate)
        {
            List<DateTime> dateTimeList = new List<DateTime>();
            while (startDate<=endDate) 
            { 
                dateTimeList.Add(startDate);
                startDate = startDate.AddDays(1);
            }
                return dateTimeList;
        }
        //HOLIDAY///////////////////////////////////////

        public void CreateHolidayDay(RestaurantHoliday restaurantHoliday)
        {
            _repository.AddToDatabaseHolidayDay(restaurantHoliday);
        }

        public List<RestaurantHoliday> GetAllHolidayDays()
        {
            return _repository.GetRestaurantHoliday();
        }

        public void DestroyHolidayDay(RestaurantHoliday holidayDays)
        {
            _repository.DeleteFromDatabaseHolidayDay(holidayDays);
        }
        public RestaurantHoliday GetSpecificHolidayDay(int id)
        {
            RestaurantHoliday entity = GetAllHolidayDays().Find(x => x.RestaurantHolidayId == id);
            return entity;
        }
        public void UpdateHolidayDay(RestaurantHoliday holidayDays)
        {
            _repository.UpdateDatabaseHolidayDay(holidayDays);
        }

    }
}

