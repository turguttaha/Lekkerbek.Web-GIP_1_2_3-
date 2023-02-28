using Lekkerbek.Web.Data;
using Lekkerbek.Web.Migrations;
using Lekkerbek.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Azure.Core.HttpHeader;

namespace Lekkerbek.Web.Controllers
{
    public class CustomerController : Controller
    {
        //!!!!Onze DataBase!!!!
        LekkerbekContext db = new LekkerbekContext();

        // GET: CustomerController
        public ActionResult Index()
        {
            return View(db.Customers);
        }
        //GET: CustomerController/Details/5
        public ActionResult Details(int id)
        {

            var data = db.Customers.Find(id);
            if (data == null)
            {
                return View();//we kunnen hier foutmelden toevoegen
            }
            return View(data);
        }
        // GET: CustomerController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CustomerController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                Customer customer = new Customer();
                customer.Name = collection["Name"];
                customer.Address = collection["Address"];
                customer.Birthday = DateTime.Parse(collection["Birthday"]);
                db.Customers.Add(customer);
                db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CustomerController/Edit/5
        public ActionResult Edit(int id)
        {
            var data = db.Customers.Find(id);
            if (data == null)
            {
                return View();//we kunnen hier foutmelden toevoegen
            }
            return View(data);
        }

        // POST: CustomerController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                var firstVersion = db.Customers.Find(id);
                firstVersion.Name = collection["Name"];
                firstVersion.Address = collection["Address"];
                firstVersion.Birthday = DateTime.Parse(collection["Birthday"]);
                db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CustomerController/Delete/5
        public ActionResult Delete(int id)
        {
            var data = db.Customers.Find(id);
            if (data == null)
            {
                return View();//we kunnen hier foutmelden toevoegen
            }
            return View(data);
        }

        // POST: CustomerController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                var data = db.Customers.Find(id);
                db.Remove(data);
                db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
