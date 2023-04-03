using Lekkerbek.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Lekkerbek.Web.Services
{
    public interface ICustomerService
    {
        public IEnumerable<Customer> Read();
        public void Create(Customer customer);
        public void Update(Customer customer);
        public void Destroy(Customer customer);
        public SelectList GetPreferredDishes();
        public SelectList GetPreferredDishes(Customer customer);
        public Customer GetSpecificCustomer(int? id);
        public bool CustomerExists(int id);
    }
}
