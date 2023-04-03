using Lekkerbek.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Lekkerbek.Web.Services
{
    public class TestService : ICustomerService
    {
        public void Create(Customer customer)
        {
            throw new NotImplementedException();
        }

        public bool CustomerExists(int id)
        {
            throw new NotImplementedException();
        }

        public void Destroy(Customer customer)
        {
            Console.WriteLine( "Test Service");
        }

        public SelectList GetPreferredDishes()
        {
            throw new NotImplementedException();
        }

        public SelectList GetPreferredDishes(Customer customer)
        {
            throw new NotImplementedException();
        }

        public Customer GetSpecificCustomer(int? id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Customer> Read()
        {
            throw new NotImplementedException();
        }

        public void Update(Customer customer)
        {
            throw new NotImplementedException();
        }
    }
}
