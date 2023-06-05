using Lekkerbek.Web.Data;
using Lekkerbek.Web.Models;
using Lekkerbek.Web.Repositories;
using Lekkerbek.Web.Services;
using Lekkerbek.Web.ViewModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;

namespace Lekkerbek.Web.NewFolder
{
    public class CustomerService : ICustomerService
    {
        private static bool UpdateDatabase = true;//If true, stores info in the database. If false, session.

        private readonly CustomersRepository _repository;

        public CustomerService(CustomersRepository customersRepository)
        {
            _repository = customersRepository;
        }

        public IList<CustomerViewModel> GetAllViews()
        {

            var result = _repository.GetCustomersViews();

            return result;
        }
        private IList<Customer> GetAll()
        {

            var result = _repository.GetCustomers();

            return result;
        }

        public IEnumerable<Customer> Read()
        {
            return GetAll();
        }
        public void Create(Customer customer)
        {
            _repository.AddToDataBase(customer);
        }
        public void Update(Customer customer)
        {
            _repository.UpdateIntoDataBase(customer);
        }

        public SelectList GetPreferredDishes()
        {
            var list = new SelectList(_repository.GetPreferredDishes(), "PreferredDishId", "Name");
            return list;
        }
        public SelectList GetPreferredDishes(Customer customer)
        {
            var list = new SelectList(_repository.GetPreferredDishes(), "PreferredDishId", "Name", customer.PreferredDishId);
            return list;
        }
        public Customer GetSpecificCustomer(int? id)
        {
            var customer = _repository.GetCustomers().Find(x=>x.CustomerId == id);
            if (customer == null)
                return null;
            else
            return customer;
        }
        public bool CustomerExists(int id)
        {
            return _repository.GetCustomers().Any(e => e.CustomerId == id);
        }
        
        //This func removes customer and related objects(orders orderlines)
        public void Destroy(CustomerViewModel customer)
        {
            if (!UpdateDatabase)
            {
                var target = GetAll().FirstOrDefault(p => p.CustomerId == customer.CustomerId);
                if (target != null)
                {
                    GetAll().Remove(target);
                }
            }
            else
            {
                var entity = new Customer();

                entity.CustomerId = customer.CustomerId;

                _repository.DeleteFromDataBase(entity);
            }
        }

    }
}

