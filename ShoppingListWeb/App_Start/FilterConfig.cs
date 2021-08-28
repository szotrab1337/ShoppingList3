﻿using ShoppingListWeb.Models;
using System.Web;
using System.Web.Mvc;

namespace ShoppingListWeb
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new AuthorizeAttribute());
            filters.Add(new UserFilterAttribute());
        }
    }
}
