using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace ShoppingListWeb.Models
{
    public class UserIdentityBase
    {
        public User User { get; set; }
        public string Role { get; set; }
        public HttpContext HttpContext { get; set; }
        public string IpAddress { get; set; }
        public bool IsLoggedIn { get; set; }
        public HttpCookie httpCookie { get; set; }

        public string Fullname => User is null ? "Guest" : User.Name + " " + User.Surname;

        private void ValidateHttpContext()
        {
            if (HttpContext == null)
            {
                FileLogger.LogMessage("HttpContext is null.");
                return;
            }
            if (HttpContext.Request == null)
            {
                FileLogger.LogMessage("Request is null.");
                return;
            }
            if (HttpContext.User == null)
            {
                FileLogger.LogMessage("User is null.");
                return;
            }

            if (!HttpContext.Request.IsAuthenticated)
            {
                SetDefaultUser();
                return;
            }
        }

        public void SetIdentity(HttpContext httpContext)
        {
            HttpContext = httpContext;

            ValidateHttpContext();

            IpAddress = HttpContext.Request.ServerVariables["REMOTE_ADDR"];

            using (Context context = new Context())
            {
                User user = context.Users.FirstOrDefault(x => x.Email == HttpContext.User.Identity.Name.ToLower());

                if (user is null)
                {
                    SetDefaultUser();
                    return;
                }

                User = user;
                IsLoggedIn = true;

                if (user.IsAdmin)
                    Role = "Admin";
                else
                    Role = "Użytkownik";
            }

            ModifyCookie();
        }

        private void ModifyCookie()
        {
            httpCookie = FormsAuthentication.GetAuthCookie(User.Email, true);
            httpCookie.Expires = DateTime.Now.AddMonths(1);
            HttpContext.Current.Response.Cookies.Set(httpCookie);
        }

        public void Logout()
        {
            SetDefaultUser();
        }

        private void SetDefaultUser()
        {
            Role = "Guest";
            User = null;
            IsLoggedIn = false;
        }
    }
}