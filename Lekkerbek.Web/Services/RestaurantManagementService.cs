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
    }
}
