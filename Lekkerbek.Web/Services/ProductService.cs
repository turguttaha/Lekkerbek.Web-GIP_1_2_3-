﻿using Lekkerbek.Web.Data;
using Lekkerbek.Web.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;

namespace Lekkerbek.Web.NewFolder
{
    public class ProductService
    {
        private static bool UpdateDatabase = true;//If true, stores info in the database. If false, session.

        private LekkerbekContext _context;

        public ProductService(LekkerbekContext entities)
        {
            this._context = entities;
        }

        private IList<Customer> GetAll()
        {

            var result = _context.Customers.Select(customer => new Customer
            //This is another way to make a new object
            {
                CustomerId = customer.CustomerId,
                FName = customer.FName,
                LName = customer.LName,
                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber,
                Address = customer.Address,
                Birthday = customer.Birthday,
                PreferredDishId = customer.PreferredDishId,
                PreferredDish = new PreferredDish()
                {
                    PreferredDishId = customer.PreferredDish.PreferredDishId,
                    Name = customer.PreferredDish.Name
                }

            }).ToList();

            return result;
        }

        public IEnumerable<Customer> Read()
        {
            return GetAll();
        }

        public void Create(Customer customer)
        {
            var customer1 = new Customer();//Declaring new customer entity and sticking in the info given from View.

            customer1.CustomerId = customer.CustomerId;
            customer1.FName = customer.FName;
            customer1.LName = customer.LName;
            customer1.Email = customer.Email;
            customer1.PhoneNumber = customer.PhoneNumber;
            customer1.Address = customer.Address;
            customer1.Birthday = customer.Birthday;
            customer1.PreferredDishId = customer.PreferredDishId;
            //customer1.PreferredDish = new PreferredDish();


            if (customer1.PreferredDishId == null)
            {
                customer1.PreferredDishId = 1;
            }

            else
            {
                customer1.PreferredDishId = customer.PreferredDish.PreferredDishId;
            }

            _context.Customers.Add(customer1);
            _context.SaveChanges();

            customer.CustomerId = customer1.CustomerId;
        }

        public void Update(Customer customer)
        {
            var customer1 = new Customer();

            customer1.CustomerId = customer.CustomerId;
            customer1.FName = customer.FName;
            customer1.LName = customer.LName;
            customer1.Email = customer.Email;
            customer1.PhoneNumber = customer.PhoneNumber;
            customer1.Address = customer.Address;
            customer1.Birthday = customer.Birthday;
            customer1.PreferredDishId = customer.PreferredDishId;

            if (customer.PreferredDish != null)
            {
                customer1.CustomerId = customer.CustomerId;
            }

            _context.Customers.Attach(customer1);
            _context.Entry(customer1).State = EntityState.Modified;
            _context.SaveChanges();
        }
    } }
    
            
               /* public void Destroy(Customer customer)
                {
                    var customer1 = new Customer();

                    customer1.CustomerId = customer.CustomerId;

                    _context.customer.Attach(customer1);

                    _context.Products.Remove(customer1);

                    var orderDetails = _context.Order_Details.Where(pd => pd.ProductID == customer1.c);

                    foreach (var orderDetail in orderDetails)
                    {
                        _context.Order_Details.Remove(orderDetail);
                    }

                    _context.SaveChanges();
                }
                }
            
            /*Customer One(Func<Customer, bool> predicate)
            {
                return GetAll().FirstOrDefault(predicate);
            }*/
        

            /*public void Dispose() //why not @override?
            {
                entities.Dispose();
            }*/


    

