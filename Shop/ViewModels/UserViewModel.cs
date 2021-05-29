using Shop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shop.ViewModels
{
    public class UserViewModel
    {
        public User user { get; set; }


        public string refreshGuid(ShopEntities db)
        {
            return "";
        }
    }
}