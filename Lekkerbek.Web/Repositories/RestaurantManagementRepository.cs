using Lekkerbek.Web.Data;
using Lekkerbek.Web.Models;

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

    }
}
