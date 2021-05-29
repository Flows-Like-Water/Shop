using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using Shop.Models;
using Shop.ViewModels;

namespace Shop.Controllers
{
    public class ItemsController : Controller
    {
        private ShopEntities db = new ShopEntities();
        //************SHOP***************

        // GET: Items
        public ActionResult Index()
        {
            var items = db.Items.Include(i => i.Price);
            return View(items.ToList());
        }

        // GET: Items/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Item item = db.Items.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        //POST : ADD ITEM TO CART
        public ActionResult AddToCart(int? id, int quantity)
        {
            try
            {
                //CHECK IF USER IS LOGGED IN FIRST
                if (Session["UserID"] == null)
                {
                    return RedirectToAction("CheckUser", "Home");
                }
                
                //FIND USER
                User user = db.Users.Find(Convert.ToInt32(Session["UserID"]));
                if(user == null)
                {
                    ViewBag.Error = "Please log in before placing items in your basket";
                    return RedirectToAction("CheckUser", "Home");
                }

                Item itm = db.Items.Where(x => x.ItemID == id).Include(p => p.Price).FirstOrDefault();
                if (itm == null)
                {
                    ViewBag.Error = "The item selected is no longer available";
                    return RedirectToAction("Index");
                }

                Item_Cart itmCart = new Item_Cart();
                itmCart.ItemID = itm.ItemID;
                itmCart.Quantity = quantity;
                itmCart.Price = itm.Price.Amount;

                //FIND USER CARTS THAT ARE NOT CHECKED-OUT
                Cart userCart = db.Carts.Where(x => x.UserID == user.UserID && x.CheckedOut == "false").FirstOrDefault();
                if(userCart != null)
                {
                    //CHECK IF ITEM IS NOT ALREADY IN CART, IF SO JUST INCREMENT THE NUMBER OF ITEMS  
                    var itmExists = userCart.Item_Cart.Where(x => x.ItemID == itmCart.ItemID).FirstOrDefault();
                    if (itmExists != null)
                    {
                        itmExists.Quantity += quantity;
                        db.Entry(itmExists).State = EntityState.Modified;
                        db.SaveChanges();

                        ViewBag.Success = "Item already in cart : Successfully added to the quantity of the item!";
                        return RedirectToAction("Index");
                    }

                    itmCart.CartID = userCart.CartID;
                    userCart.Item_Cart.Add(itmCart);
                    db.Entry(userCart).State = EntityState.Modified;
                    db.SaveChanges();

                    ViewBag.Success = "Successfully added item to cart!";
                    return RedirectToAction("Index");
                }

                Cart newCart = new Cart();
                newCart.UserID = user.UserID;
                newCart.CheckedOut = "false";
                newCart.Item_Cart.Add(itmCart);
                db.Carts.Add(newCart);
                db.SaveChanges();

                ViewBag.Success = "Successfully added item to cart!";

                return RedirectToAction("Index");
            }
            catch (Exception err)
            {

                return Content(err.Message);
            }
        }

        // GET: Items/Create
        public ActionResult Create()
        {
            ViewBag.PriceID = new SelectList(db.Prices, "PriceID", "PriceID");
            return View();
        }

        // POST: Items/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ItemID,PriceID,ItemName,Description,PicturePath")] Item item, Price price)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    db.Prices.Add(price);
                    if (db.SaveChanges() > 0)
                    {
                        item.PriceID = price.PriceID;
                        db.Items.Add(item);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                        return Content("There was a problem saving the Price of the Item. Please double check if you entered correct values.");
                }

                return View(item);
            }
            catch (Exception err)
            {
                ViewBag.Error = err.Message;
                return View();
            }
        }

        // GET: Items/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Item item = db.Items.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            ViewBag.PriceID = new SelectList(db.Prices, "PriceID", "PriceID", item.PriceID);
            return View(item);
        }

        // POST: Items/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ItemID,PriceID,ItemName,Description,PicturePath")] Item item)
        {
            if (ModelState.IsValid)
            {
                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PriceID = new SelectList(db.Prices, "PriceID", "PriceID", item.PriceID);
            return View(item);
        }

        // GET: Items/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Item item = db.Items.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        // POST: Items/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Item item = db.Items.Find(id);
            db.Items.Remove(item);
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
