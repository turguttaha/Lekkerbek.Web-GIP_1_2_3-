using Lekkerbek.Web.Models;
using Lekkerbek.Web.ViewModel;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Lekkerbek.Web.Services
{
    public interface ICustomerService
    {
        public IEnumerable<Customer> Read();
        public IList<CustomerViewModel> GetAllViews();
        public void Create(Customer customer);
        public void Update(Customer customer);
        public void Destroy(CustomerViewModel customer);
        public SelectList GetPreferredDishes();
        public SelectList GetPreferredDishes(Customer customer);
        public Customer GetSpecificCustomer(int? id);
        public bool CustomerExists(int id);
    }
}
