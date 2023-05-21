using Lekkerbek.Web.Models;
using Lekkerbek.Web.Repositories;

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

