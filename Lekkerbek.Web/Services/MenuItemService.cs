using Kendo.Mvc.UI;
using Lekkerbek.Web.Data;
using Lekkerbek.Web.Models;
using Lekkerbek.Web.Repositories;
using Lekkerbek.Web.ViewModel;

namespace Lekkerbek.Web.Services
{
    public class MenuItemService : IMenuItemService
    {
        private readonly MenuItemRepository _repository;

        public MenuItemService(MenuItemRepository repository)
        {
            _repository = repository;
        }

        //private IList<MenuItem> GetAll()
            private IList<MenuItemViewModel> GetAll()
        {

            var result = _repository.GetMenuItems();

            return result;
        }

        //public IEnumerable<MenuItem> Read()
            public IEnumerable<MenuItemViewModel> Read()
        {
            return GetAll();
        }

            public void Destroy(Models.MenuItem menuItem)
        {


                var entity = new Models.MenuItem();

                entity.MenuItemId = menuItem.MenuItemId;
            

            _repository.DeleteFromDataBase(entity);

        }

        //public void Create(MenuItem menuItem) 
        public void Create(Models.MenuItem menuItem)
        { 
            //_repository.AddToDataBase(menuItem);
            _repository.AddToDataBase(menuItem);
        }

        //public MenuItem GetSpecificMenuItem(int? id)
        public MenuItemViewModel GetSpecificMenuItem(int? id)
        {
            var menuItem = _repository.GetMenuItems().Find(x => x.MenuItemId == id);
            if (menuItem == null)
                return null;
            else
                return menuItem;
        }

        //public void Update(MenuItem menuItem)
        public void Update(Models.MenuItem menuItem)
        {
            //_repository.UpdateIntoDataBase(menuItem);
            _repository.UpdateIntoDataBase(menuItem);
        }

        public bool MenuItemExists(int id)
        {
            return GetAll().Any(e => e.MenuItemId == id);
        }
    }
}
