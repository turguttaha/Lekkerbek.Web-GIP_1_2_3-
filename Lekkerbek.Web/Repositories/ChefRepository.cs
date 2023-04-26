using Lekkerbek.Web.Data;
using Lekkerbek.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Lekkerbek.Web.Repositories
{
    public class ChefRepository
    {
        private readonly LekkerbekContext _context;

        public ChefRepository(LekkerbekContext context)
        {
            _context = context;
        }
        public List<Chef> GetChef()
        {
            return _context.Chefs.ToList();

        }
        public void AddToDataBase(Chef chef)
        {
            _context.Add(chef);
            _context.SaveChanges();
        }
        public void DeleteFromDataBase(Chef entity)
        {

            _context.Chefs.Remove(entity);

            

            _context.SaveChanges();
        }
    }
}
