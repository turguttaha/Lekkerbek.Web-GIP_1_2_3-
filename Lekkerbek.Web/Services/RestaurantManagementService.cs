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
    }
}
