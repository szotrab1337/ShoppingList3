using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShoppingListWeb.Models
{
    public static class UserIdentity
    {
        public static string Fullname { get { return GetIdentity() == null ? "Guest" : GetIdentity().Fullname; } }
        public static string Role { get { return GetIdentity() == null ? "Guest" : GetIdentity().Role; } }
        public static string IpAddress { get { return GetIdentity() == null ? "unknown" : GetIdentity().IpAddress; } }
        public static User User { get { return GetIdentity()?.User; } }
        public static bool IsLoggedIn { get { return GetIdentity() == null ? false : GetIdentity().IsLoggedIn; } }

        public static UserIdentityBase GetIdentity()
        {
            return (UserIdentityBase)HttpContext.Current.Items["UserIdentity"];
        }
    }
}