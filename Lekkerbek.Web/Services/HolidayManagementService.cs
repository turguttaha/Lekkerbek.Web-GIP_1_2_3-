using Lekkerbek.Web.Models;
using Lekkerbek.Web.Repositories;

namespace Lekkerbek.Web.Services
{
    public class HolidayManagementService
    {
        private readonly HolidayManagementRepository _repository;
        public HolidayManagementService(HolidayManagementRepository repository) 
        { 
        _repository = repository;
        }

        public void CreateHolidayDay(RestaurantHolidayDays restaurantHolidayDays)
        {
            _repository.AddToDatabaseRestaurantHolidayDay(restaurantHolidayDays);
        }
    }
}
