using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Shop.Models;
using Shop.ViewModels;

namespace Shop.Controllers
{
    public class CustomersController : Controller
    {
        private ShopEntities db = new ShopEntities();

        // GET: Customers
        public ActionResult Index()
        {
            var customers = db.Customers.Include(c => c.User);
            return View(customers.ToList());
        }

        // GET: Customers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // GET: Customers/Create
        public ActionResult Create()
        {
            ViewBag.UserID = new SelectList(db.Users, "UserID", "Username");
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CustomerID,UserID,FullName,ContactNo,AltContactNo")] Customer customer, User user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    UserViewModel newUser = new UserViewModel();
                    newUser.user.Username = user.Username;
                    newUser.user.Password = HashPass(AddSalt(user.Password));
                    db.Users.Add(newUser.user);
                    if ( db.SaveChanges() > 0)
                    {
                        customer.UserID = newUser.user.UserID;
                        db.Customers.Add(customer);


                        TempData["userVm"] = newUser;

                        db.SaveChanges();
                        return RedirectToAction("Index");

                    }
                }

                return View(customer);
            }
            catch (Exception err)
            {

                return View(err.Message);
            }
            
        }

        // GET: Customers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserID = new SelectList(db.Users, "UserID", "Username", customer.UserID);
            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CustomerID,UserID,FullName,ContactNo,AltContactNo")] Customer customer)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(customer).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(customer);
            }
            catch (Exception)
            {

                throw;
            }
            
        }

        //HASH PASSWORD
        private string HashPass(string password)
        { 
            using(SHA256 sha = SHA256.Create())
            {
                //COMPUTE HASH - RETURNS BYTE ARRAY
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));

                //CONVERT BYTE ARRAY TO A STRING
                StringBuilder builder = new StringBuilder();
                for(int i=0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        //ADD SALT TO PASSWORD
        private string AddSalt(string password)
        {
            return password + "dicvhdfhgbnktwkijhnsgbwivbksiuhj";
        }

        // GET: Customers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Customer customer = db.Customers.Find(id);
            db.Customers.Remove(customer);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
