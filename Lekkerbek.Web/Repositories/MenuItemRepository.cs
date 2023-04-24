using Lekkerbek.Web.Data;
using Lekkerbek.Web.Models;

namespace Lekkerbek.Web.Repositories
{
    public class MenuItemRepository
    {
        private readonly LekkerbekContext _context;

        public MenuItemRepository(LekkerbekContext context)
        {
            _context = context;
        }

        public List<MenuItem> GetMenuItems()
        {
            //var MenuItems = _context.MenuItems.ToList();

            return _context.MenuItems.Select(item => new Models.MenuItem
            //This is another way to make a new object
            {
                MenuItemId = item.MenuItemId,
                Price = item.Price,
                Sort = item.Sort,
                BtwNumber = item.BtwNumber,
                Description = item.Description,
                Name = item.Name,
                BtwNumber = item.BtwNumber,
                Sort = item.Sort,
            }).ToList();
        }

        public void DeleteFromDataBase(MenuItem entity)
        {
            _context.MenuItems.Remove(entity);
            _context.SaveChanges();
        }

        public void AddToDataBase(MenuItem menuItem) 
        {
            _context.MenuItems.Add(menuItem);
             _context.SaveChangesAsync();
        }

        public void UpdateIntoDataBase(MenuItem menuItem)
        {
            _context.Update(menuItem);
            _context.SaveChanges();
        }

    }
}
