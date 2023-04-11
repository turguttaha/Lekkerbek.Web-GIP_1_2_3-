﻿using Lekkerbek.Web.Models;

namespace Lekkerbek.Web.Services
{
    public interface IOrderChefService
    {
        public IEnumerable<Order> Read();
        public void Update(Order order);
        public void UpdateCustomer(Customer customer);
        public Order GetSpecificOrder(int? id);
        public List<OrderLine> OrderLineRead(int? id);
        public List<Order> GetOrders(int? id);
        public Order GetChefOrders(int? id);
        public List<Customer> GetAllCustomers();
        public Customer GetSpecificCustomer(int? id);
        public List<PreferredDish> GetAllPrefferedDishes();
        public IEnumerable<PreferredDish> ReadPrefferedDish();
        public bool OrderExists(int id);
    }
}
