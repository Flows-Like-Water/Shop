using Shop.Models;
using Shop.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Shop.Controllers
{
    public class HomeController : Controller
    {
        private ShopEntities db = new ShopEntities();

        public ActionResult Index()
        {
            try
            {
                return View();
            }
            catch (Exception err)
            {
                return ViewBag(err.Message);
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult CheckUser()
        {
            return View();
        }

        // POST --LOGIN ACTION
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CheckUser(User user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (user.Username == null || user.Password == null)
                    {
                        ViewBag.Error = "Username/Password not specified";
                        return View();
                    }
                    string hashedPass = HashPass(AddSalt(user.Password));
                    User LoginUser = db.Users.Where(x => x.Username == user.Username && x.Password == hashedPass).FirstOrDefault();
                    if (LoginUser == null)
                    {
                        ViewBag.Error = "The user does not exist, please make sure everything is entered correctly.";
                        return View();
                    }

                    Session["UserID"] = LoginUser.UserID.ToString();
                    Session["Username"] = LoginUser.Username.ToString();

                    ViewBag.Success = "Successfully logged in!";
                    return RedirectToAction("Index");
                }

                return View(user);
            }
            catch (Exception err)
            {

                return View(err.Message);
            }
        }

        private string HashPass(string password)
        {
            using (SHA256 sha = SHA256.Create())
            {
                //COMPUTE HASH - RETURNS BYTE ARRAY
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));

                //CONVERT BYTE ARRAY TO A STRING
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
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
    }
}