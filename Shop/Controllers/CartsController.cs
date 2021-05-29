using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using Shop.ViewModels;

namespace Shop.Controllers
{
    public class CartsController : Controller
    {
        private ShopEntities db = new ShopEntities();

        // GET: Carts
        public ActionResult Index()
        {
            try
            {
                var usr = db.Users.Find(Convert.ToInt32(Session["UserID"]));
                if(usr == null)
                {
                    return RedirectToAction("CheckUser", "Home");
                }

                Cart userCart = db.Carts.Where(x => x.UserID == usr.UserID).Include(itm => itm.Item_Cart).FirstOrDefault();

                return View(userCart);

            }
            catch (Exception)
            {

                throw;
            }
        }

        // GET: Carts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cart cart = db.Carts.Find(id);
            if (cart == null)
            {
                return HttpNotFound();
            }
            return View(cart);
        }

        // GET: Carts/Create
        public ActionResult Create()
        {
            ViewBag.UserID = new SelectList(db.Users, "UserID", "Username");
            return View();
        }

        // POST: Carts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CartID,UserID,Total,NumberOfItems,CheckedOut")] Cart cart)
        {
            if (ModelState.IsValid)
            {
                db.Carts.Add(cart);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UserID = new SelectList(db.Users, "UserID", "Username", cart.UserID);
            return View(cart);
        }

        // GET: Carts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cart cart = db.Carts.Find(id);
            if (cart == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserID = new SelectList(db.Users, "UserID", "Username", cart.UserID);
            return View(cart);
        }

        // POST: Carts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CartID,UserID,Total,NumberOfItems,CheckedOut")] Cart cart)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cart).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserID = new SelectList(db.Users, "UserID", "Username", cart.UserID);
            return View(cart);
        }

        // GET: Carts/Delete/5
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                if (Session["UserID"] == null)
                {
                    return RedirectToAction("CheckUser", "Home");
                }

                User usr = db.Users.Find(Convert.ToInt32(Session["UserID"]));
                if (usr == null)
                {
                    ViewBag.Error = "There was a problem verifying your user details";
                    return View();
                }

                Cart userCart = db.Carts.Where(x => x.CheckedOut == "false" && x.UserID == usr.UserID).Include(m => m.Item_Cart).FirstOrDefault();
                if (userCart == null)
                {
                    ViewBag.Error = "There was a provlem getting your cart details";
                }

                var remove = userCart.Item_Cart.Where(z => z.ItemID == id).FirstOrDefault();

                if (remove == null)
                {
                    ViewBag.Error = "Error: Item no longer exists";
                    return View();
                }

                db.Item_Cart.Remove(remove);
                db.SaveChanges();
                ViewBag.Success = "Successfully removed item!";

                return View();
            }
            catch (Exception err)
            {
                ViewBag.Error = err.Message;
                return View();
            }
            
        }

        // POST: Carts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Cart cart = db.Carts.Find(id);
            db.Carts.Remove(cart);
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
