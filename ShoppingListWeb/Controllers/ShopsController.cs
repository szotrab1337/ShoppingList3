using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShoppingListWeb.Controllers
{
    public class ShopsController : Controller
    {
        public ActionResult List()
        {
            return View();
        }
    }
}