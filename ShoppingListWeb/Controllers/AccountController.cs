using ShoppingListWeb.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ShoppingListWeb.Controllers
{
    public class AccountController : Controller
    {
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            FileLogger.LogMessage("Login get.");
            FileLogger.LogMessage(HttpContext.User.Identity.Name);

            if (Request.QueryString["success"] != null)
                ViewBag.Success = Request.QueryString["success"];

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string returnUrl, UserLogin userLogin)
        {
            FileLogger.LogMessage("Login post.");

            if(!ModelState.IsValid)
            {
                FileLogger.LogMessage("ModelState invalid.");
                return View(userLogin);
            }

            using(Context context = new Context())
            {
                string passwordEncrypted = PasswordAdapter.EncryptPlainTextToCipherText(userLogin.Password);

                User user = context.Users.FirstOrDefault(x => x.Email == userLogin.Email &&
                    x.Password == passwordEncrypted);

                if(user == null)
                {
                    ModelState.AddModelError("wrongAccount", "Podano błędne hasło lub adres e-mail.");
                    return View(userLogin);
                }
            }

            FormsAuthentication.SetAuthCookie(userLogin.Email, userLogin.RememberMe);
            
            return RedirectToLocal(returnUrl);
        }
        
        [AllowAnonymous]
        public ActionResult Register(string returnUrl)
        {
            FileLogger.LogMessage("Login get.");
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(User user)
        {
            FileLogger.LogMessage("Login post.");

            if(!ModelState.IsValid)
            {
                FileLogger.LogMessage("ModelState invalid.");
                return View(user);
            }

            using(Context context = new Context())
            {
                User checkUser = context.Users.FirstOrDefault(x => x.Email == user.Email);

                if (checkUser != null)
                {
                    ModelState.AddModelError("wrongNewAccout", "Konto z podanym adresem e-mail już istnieje.");
                    return View(user);
                }

                user.IsAdmin = false;
                user.Password = PasswordAdapter.EncryptPlainTextToCipherText(user.Password);
                user.PasswordConfirm = PasswordAdapter.EncryptPlainTextToCipherText(user.PasswordConfirm);
                user.Email = user.Email.ToLower();
                context.Users.AddOrUpdate(user);
                context.SaveChanges();
            }

            return RedirectToAction("Login", new { returnUrl = "", success = "Utworzono nowe konto. Możesz się teraz zalogować." });
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

    }
}