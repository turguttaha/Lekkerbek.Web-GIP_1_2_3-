using Lekkerbek.Web.Data;
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
        public class ProductService : IDisposable
        {
            private static bool UpdateDatabase = false;
            
            private LekkerbekContext _context;

            public ProductService(LekkerbekContext entities)
            {
                this._context = entities;
            }
            
            public IList<Customer> GetAll()
            {
               // var result = HttpContext.Session["Products"] as IList<Customer>;

                //if (result == null || UpdateDatabase)
               // {
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

                   // HttpContext.Session["Products"] = result;
              //  }

                return result;
            }

            public IEnumerable<Customer> Read()
            {
                return GetAll();
            }

            public void Create(Customer customer)
            {
                //if (!UpdateDatabase)
                //{
               // var first = GetAll().OrderByDescending(e => e.CustomerId).FirstOrDefault();
               //  var id = (first != null) ? first.CustomerId : 0;

                // customer.CustomerId = id + 1;

                //    if (customer.PreferredDishId == null)
                //    {
                //        customer.PreferredDishId = 1;
                //    }

                //    if (customer.PreferredDishId == null)
                //    {
                //        customer.PreferredDishId = new PreferredDish() { PreferredDishId = 1, Name = "Beverages" };
                //    }

                //    GetAll().Insert(0, customer);
                //}
                //else
                //{
                    var entity = new Customer();//Declaring new customer entity and sticking in the info given from View.

                    entity.FName = customer.FName;
                    entity.Address = customer.Address;
                    entity.LName = customer.LName;
                    entity.PhoneNumber = customer.PhoneNumber;
                    entity.Birthday = customer.Birthday;
                    entity.Email = customer.Email;

                    if (entity.PreferredDishId == null)
                    {
                        entity.PreferredDishId = 1;
                    }

                    if (customer.PreferredDishId != null)
                    {
                        entity.PreferredDishId = customer.PreferredDish.PreferredDishId;
                    }

                    _context.Customers.Add(entity);
                    _context.SaveChanges();

                    customer.CustomerId = entity.CustomerId;
               // }
            }
        /*
            public void Update(ProductViewModel product)
            {
                if (!UpdateDatabase)
                {
                    var target = One(e => e.ProductID == product.ProductID);

                    if (target != null)
                    {
                        target.ProductName = product.ProductName;
                        target.UnitPrice = product.UnitPrice;
                        target.UnitsInStock = product.UnitsInStock;
                        target.Discontinued = product.Discontinued;

                        if (product.CategoryID == null)
                        {
                            product.CategoryID = 1;
                        }

                        if (product.Category != null)
                        {
                            product.CategoryID = product.Category.CategoryID;
                        }
                        else
                        {
                            product.Category = new CategoryViewModel()
                            {
                                CategoryID = (int)product.CategoryID,
                                CategoryName = _context.Categories.Where(s => s.CategoryID == product.CategoryID).Select(s => s.CategoryName).First()
                            };
                        }

                        target.CategoryID = product.CategoryID;
                        target.Category = product.Category;
                    }
                }
                else
                {
                    var entity = new Product();

                    entity.ProductID = product.ProductID;
                    entity.ProductName = product.ProductName;
                    entity.UnitPrice = product.UnitPrice;
                    entity.UnitsInStock = (short)product.UnitsInStock;
                    entity.Discontinued = product.Discontinued;
                    entity.CategoryID = product.CategoryID;

                    if (product.Category != null)
                    {
                        entity.CategoryID = product.Category.CategoryID;
                    }

                    _context.Products.Attach(entity);
                    _context.Entry(entity).State = EntityState.Modified;
                    _context.SaveChanges();
                }
            }

            public void Destroy(ProductViewModel product)
            {
                if (!UpdateDatabase)
                {
                    var target = GetAll().FirstOrDefault(p => p.ProductID == product.ProductID);
                    if (target != null)
                    {
                        GetAll().Remove(target);
                    }
                }
                else
                {
                    var entity = new Product();

                    entity.ProductID = product.ProductID;

                    _context.Products.Attach(entity);

                    _context.Products.Remove(entity);

                    var orderDetails = _context.Order_Details.Where(pd => pd.ProductID == entity.ProductID);

                    foreach (var orderDetail in orderDetails)
                    {
                        _context.Order_Details.Remove(orderDetail);
                    }

                    _context.SaveChanges();
                }
            }
        */
            public Customer One(Func<Customer, bool> predicate)
            {
                return GetAll().FirstOrDefault(predicate);
            }

            public void Dispose()
            {
                _context.Dispose();
            }
        }
    }

