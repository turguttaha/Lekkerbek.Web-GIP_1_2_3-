using Lekkerbek.Web.Models;
using Lekkerbek.Web.ViewModel;

namespace Lekkerbek.Web.Services
{
    public interface IMenuItemService
    {
        public IEnumerable<MenuItemViewModel> Read();
        public void Destroy(MenuItem menuItem);
        public void Create(MenuItem menuItem);
        public MenuItemViewModel GetSpecificMenuItem(int? id);
        public void Update(MenuItem menuItem);
        public bool MenuItemExists(int id);
    }
}
