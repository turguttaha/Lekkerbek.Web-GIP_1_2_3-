using Lekkerbek.Web.Data;
using Lekkerbek.Web.Models;

namespace Lekkerbek.Web.Repositories
{
    public class HolidayManagementRepository
    {
        private readonly LekkerbekContext _context;

        public HolidayManagementRepository(LekkerbekContext context)
        {
            _context = context;
        }

        public void AddToDatabaseRestaurantHolidayDay(RestaurantHolidayDays entity)
        {
            _context.Add(entity);
            _context.SaveChanges();
        }
    }
}
