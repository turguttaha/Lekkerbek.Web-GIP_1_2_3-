﻿using Lekkerbek.Web.Data;
using Lekkerbek.Web.Models;
using Lekkerbek.Web.ViewModel;
using NuGet.Protocol.Core.Types;

namespace Lekkerbek.Web.Repositories
{
    public class CustomersRepository
    {
        private readonly LekkerbekContext _context;

        public CustomersRepository(LekkerbekContext context)
        {
            _context = context;
        }

        public List<CustomerViewModel> GetCustomersViews()
        {
            return _context.Customers.Select(customer => new CustomerViewModel
            //This is another way to make a new object
            {
                CustomerId = customer.CustomerId,
                Name = (customer.ContactPerson!=null? customer.FirmName : customer.FName + " " + customer.LName),
                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber,
                Btw = customer.Btw,
                BtwNumber = customer.BtwNumber,
                Address =  customer.StreetName+ " " + customer.City + " " + customer.PostalCode,
                ContactPerson = customer.ContactPerson,
                FirmName = customer.FirmName,
                Birthday = customer.Birthday,
                IdentityUser = customer.IdentityUser,
                PreferredDishName = customer.PreferredDish.Name




            }).ToList();

        }

        public List<Customer> GetCustomers()
        {
            return _context.Customers.Select(customer => new Customer
            //This is another way to make a new object
            {
                CustomerId = customer.CustomerId,
                FName = customer.FName,
                LName = customer.LName,
                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber,
                Btw = customer.Btw,
                BtwNumber = customer.BtwNumber,
                PostalCode = customer.PostalCode,
                City = customer.City,
                StreetName = customer.StreetName,
                ContactPerson = customer.ContactPerson,
                FirmName = customer.FirmName,
                Birthday = customer.Birthday,
                PreferredDishId = customer.PreferredDishId,
                IdentityUser = customer.IdentityUser,
                PreferredDish = new PreferredDish()
                {
                    PreferredDishId = customer.PreferredDish.PreferredDishId,
                    Name = customer.PreferredDish.Name
                }
                

            }).ToList();

        }
        public void AddToDataBase(Customer customer)
        {
            _context.Add(customer);
            _context.SaveChanges();
        }

        public void UpdateIntoDataBase(Customer customer)
        {

            _context.Update(customer);
            _context.SaveChanges();
        }
        public void DeleteFromDataBase(Customer entity)
        {

            _context.Customers.Attach(entity);

            _context.Customers.Remove(entity);

            //var orders = _context.Orders.Where(pd => pd.CustomerId == entity.CustomerId);
            //var orderLines = _context.OrderLines.Where(pd => pd.Order.CustomerId == entity.CustomerId);
            //foreach (var orderLine in orderLines)
            //{
            //    _context.OrderLines.Remove(orderLine);
            //}

            //foreach (var order in orders)
            //{
            //    _context.Orders.Remove(order);
            //}

            _context.SaveChanges();
        }

        public List<PreferredDish> GetPreferredDishes()
        {
            return _context.PreferredDishes.ToList();
        }
    }
}
