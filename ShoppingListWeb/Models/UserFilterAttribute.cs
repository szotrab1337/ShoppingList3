using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShoppingListWeb.Models
{
    public class UserFilterAttribute : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                UserIdentityBase userIdentity = new UserIdentityBase();
                userIdentity.SetIdentity(HttpContext.Current);
                HttpContext.Current.Items.Add("UserIdentity", userIdentity);
            }
            catch (Exception ex)
            {
                FileLogger.LogMessage(ex.ToString());
                throw ex;
            }
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }
    }
}