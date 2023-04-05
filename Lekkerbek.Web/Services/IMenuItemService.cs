using Lekkerbek.Web.Models;

namespace Lekkerbek.Web.Services
{
    public interface IMenuItemService
    {
        public IEnumerable<MenuItem> Read();
        public void Destroy(MenuItem menuItem);
        public void Create(MenuItem menuItem);
        public MenuItem GetSpecificMenuItem(int? id);
        public void Update(MenuItem menuItem);
        public bool MenuItemExists(int id);
    }
}
