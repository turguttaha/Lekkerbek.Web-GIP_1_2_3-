using Lekkerbek.Web.Data;
using Lekkerbek.Web.Models;
using Lekkerbek.Web.ViewModel;
using System.Globalization;

namespace Lekkerbek.Web.Repositories
{
    public class MenuItemRepository
    {
        private readonly LekkerbekContext _context;

        public MenuItemRepository(LekkerbekContext context)
        {
            _context = context;
        }

        public List<MenuItemViewModel> GetMenuItems()
        {
            //var MenuItems = _context.MenuItems.ToList();

            //return _context.MenuItems.Select(item => new Models.MenuItem
            return _context.MenuItems.Select(item => new MenuItemViewModel
            //This is another way to make a new object
            {
                MenuItemId = item.MenuItemId,
                Price = Convert.ToDouble(item.Price, new CultureInfo("en-EN")),
                Sort = item.Sort,
                BtwNumber = item.BtwNumber,
                Description = item.Description,
                Name = item.Name,
            }).ToList();
        }

        //public void DeleteFromDataBase(MenuItem entity)
        public void DeleteFromDataBase(MenuItem entity)
        {
            _context.MenuItems.Remove(entity);
            _context.SaveChanges();
        }

        //public void AddToDataBase(MenuItem menuItem) 
        public void AddToDataBase(MenuItem menuItem)
        {
            _context.MenuItems.Add(menuItem);
             _context.SaveChangesAsync();
        }

        //public void UpdateIntoDataBase(MenuItem menuItem)
        public void UpdateIntoDataBase(MenuItem menuItem)
        {
            _context.Update(menuItem);
            _context.SaveChanges();
        }

    }
}
