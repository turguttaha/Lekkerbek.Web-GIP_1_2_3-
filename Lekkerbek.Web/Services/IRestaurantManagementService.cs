using Lekkerbek.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Lekkerbek.Web.Services
{
    public interface IRestaurantManagementService
    {
        public void CreateOpeningsHour(RestaurantOpeninghours restaurantOpeninghours);
        public List<RestaurantOpeninghours> GetAllOpeningsHours();
        public void DestroyOpeningsHour(RestaurantOpeninghours openingHours);
        public RestaurantOpeninghours GetSpecificOpeningsHour(int id);
        public void UpdateOpeningsHour(RestaurantOpeninghours openingHours);
        public List<WorkerHoliday> GetAllWorkerHolidays();
        public List<WorkerHoliday> GetAllHolidays();
        public List<WorkerHoliday> GetAllWokerHolidays(int id);
        public List<WorkerHoliday> GetAllHolidaysFromAWorker(int? id);
        public void CreateWorkerHoliday(WorkerHoliday workerHoliday);
        public void DestroyWorkerHoliday(WorkerHoliday holidayDays);
        public WorkerHoliday GetSpecificWorkerHoliday(int id);
        public void UpdateWorkerHoliday(WorkerHoliday workerHoliday);
        public List<Chef> GetChefs();
        public SelectList ChefsSelectList();
        public List<Order> GetAllOrders(DateTime startDate, DateTime endDate);
        public List<Order> GetAllOrders(DateTime startDate);
        public List<DateTime> GetDateTimeRange(DateTime startDate, DateTime endDate);
        public void CreateHolidayDay(RestaurantHoliday restaurantHoliday);
        public List<RestaurantHoliday> GetAllHolidayDays();
        public void DestroyHolidayDay(RestaurantHoliday holidayDays);
        public RestaurantHoliday GetSpecificHolidayDay(int id);
        public void UpdateHolidayDay(RestaurantHoliday holidayDays);
    }
}
