using Lekkerbek.Web.Data;
using Lekkerbek.Web.Models;
using Lekkerbek.Web.Repositories;

namespace Lekkerbek.Web.Services
{
    public class MenuItemService : IMenuItemService
    {
        private readonly MenuItemRepository _repository;

        public MenuItemService(MenuItemRepository repository)
        {
            _repository = repository;
        }

        private IList<MenuItem> GetAll()
        {

            var result = _repository.GetMenuItems();

            return result;
        }

        public IEnumerable<MenuItem> Read()
        {
            return GetAll();
        }

        public void Destroy(MenuItem menuItem)
        {


                var entity = new MenuItem();

                entity.MenuItemId = menuItem.MenuItemId;

                _repository.DeleteFromDataBase(entity);

        }

        public void Create(MenuItem menuItem) 
        { 
            _repository.AddToDataBase(menuItem);
        }

        public MenuItem GetSpecificMenuItem(int? id)
        {
            var menuItem = _repository.GetMenuItems().Find(x => x.MenuItemId == id);
            if (menuItem == null)
                return null;
            else
                return menuItem;
        }

        public void Update(MenuItem menuItem)
        {
        _repository.UpdateIntoDataBase(menuItem);
        }

        public bool MenuItemExists(int id)
        {
            return GetAll().Any(e => e.MenuItemId == id);
        }
    }
}
