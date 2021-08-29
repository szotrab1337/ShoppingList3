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
            try
            {
                FileLogger.LogMessage("Login get.");
                FileLogger.LogMessage(HttpContext.User.Identity.Name);

                if (Request.QueryString["success"] != null)
                    ViewBag.Success = Request.QueryString["success"];

                ViewBag.ReturnUrl = returnUrl;
                return View();
            }
            catch (Exception ex)
            {
                FileLogger.LogMessage("Error: " + ex.ToString(), 3);
                return View("~/Views/Shared/Error.cshtml");
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string returnUrl, UserLogin userLogin)
        {
            try
            {
                FileLogger.LogMessage("Login post.");

                if (!ModelState.IsValid)
                {
                    FileLogger.LogMessage("ModelState invalid.");
                    return View(userLogin);
                }

                using (Context context = new Context())
                {
                    string passwordEncrypted = PasswordAdapter.EncryptPlainTextToCipherText(userLogin.Password);

                    User user = context.Users.FirstOrDefault(x => x.Email == userLogin.Email &&
                        x.Password == passwordEncrypted);

                    if (user == null)
                    {
                        ModelState.AddModelError("wrongAccount", "Podano błędne hasło lub adres e-mail.");
                        return View(userLogin);
                    }
                }

                FormsAuthentication.SetAuthCookie(userLogin.Email, userLogin.RememberMe);

                return RedirectToLocal(returnUrl);
            }
            catch (Exception ex)
            {
                FileLogger.LogMessage("Error: " + ex.ToString(), 3);
                return View("~/Views/Shared/Error.cshtml");
            }
        }

        [AllowAnonymous]
        public ActionResult Register(string returnUrl)
        {
            try
            {
                FileLogger.LogMessage("Login get.");
                ViewBag.ReturnUrl = returnUrl;
                return View();
            }
            catch (Exception ex)
            {
                FileLogger.LogMessage("Error: " + ex.ToString(), 3);
                return View("~/Views/Shared/Error.cshtml");
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(User user)
        {
            try
            {
                FileLogger.LogMessage("Login post.");

                if (!ModelState.IsValid)
                {
                    FileLogger.LogMessage("ModelState invalid.");
                    return View(user);
                }

                using (Context context = new Context())
                {
                    User checkUser = context.Users.FirstOrDefault(x => x.Email == user.Email);

                    if (checkUser != null)
                    {
                        ModelState.AddModelError("wrongNewAccout", "Konto z podanym adresem e-mail już istnieje.");
                        return View(user);
                    }

                    user.IsAdmin = false;
                    user.Password = PasswordAdapter.EncryptPlainTextToCipherText(user.Password);
                    user.Email = user.Email.ToLower();
                    user.CreatedOn = DateTime.Now;
                    context.Users.AddOrUpdate(user);
                    context.Configuration.ValidateOnSaveEnabled = false;
                    context.SaveChanges();
                }

                return RedirectToAction("Login", new { returnUrl = "", success = "Utworzono nowe konto. Możesz się teraz zalogować." });
            }
            catch (Exception ex)
            {
                FileLogger.LogMessage("Error: " + ex.ToString(), 3);
                return View("~/Views/Shared/Error.cshtml");
            }
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

        public ActionResult Account()
        {
            try
            {
                User user = new User();

                using (Context context = new Context())
                {
                    user = context.Users.FirstOrDefault(x => x.UserId == UserIdentity.User.UserId);
                }

                if (user is null)
                    return View("~/Views/Shared/Error.cshtml");

                EditAccount editAccount = new EditAccount()
                {
                    Name = user.Name,
                    Surname = user.Surname,
                    Email = user.Email,
                    Password = string.Empty,
                    NewPassword = string.Empty,
                    ConfirmNewPassword = string.Empty,
                    CreatedOn = user.CreatedOn,
                    ModifiedOn = user.ModifiedOn
                };

                return View(editAccount);
            }
            catch (Exception ex)
            {
                FileLogger.LogMessage("Error: " + ex.ToString(), 3);
                return View("~/Views/Shared/Error.cshtml");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Account(EditAccount editAccount)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    FileLogger.LogMessage("ModelState invalid.");
                    return View(editAccount);
                }

                User user = new User();
                User checkEmail = new User();
                using (Context context = new Context())
                {
                    user = context.Users.FirstOrDefault(x => x.UserId == UserIdentity.User.UserId);
                    checkEmail = context.Users.FirstOrDefault(x => x.Email == editAccount.Email && x.UserId != UserIdentity.User.UserId);
                }

                if (checkEmail != null)
                {
                    ModelState.AddModelError("Email", "Istnieje już konto z podanym adresem e-mail.");
                    return View(editAccount);
                }

                if (!string.IsNullOrWhiteSpace(editAccount.Password) || (!string.IsNullOrWhiteSpace(editAccount.NewPassword) || !string.IsNullOrWhiteSpace(editAccount.ConfirmNewPassword)))
                {
                    if (PasswordAdapter.EncryptPlainTextToCipherText(editAccount.Password) != user.Password)
                    {
                        ModelState.AddModelError("Password", "Podałeś błędne hasło.");
                        return View(editAccount);
                    }
                }

                using (Context context = new Context())
                {
                    if (!string.IsNullOrWhiteSpace(editAccount.NewPassword))
                    {
                        user.Password = PasswordAdapter.EncryptPlainTextToCipherText(editAccount.NewPassword);
                        user.PasswordConfirm = PasswordAdapter.EncryptPlainTextToCipherText(editAccount.NewPassword);
                    }

                    user.Email = editAccount.Email.ToLower();
                    user.ModifiedOn = DateTime.Now;
                    user.Name = editAccount.Name;
                    user.Surname = editAccount.Surname;
                    context.Users.AddOrUpdate(user);
                    context.Configuration.ValidateOnSaveEnabled = false;
                    context.SaveChanges();
                }

                return RedirectToAction("Index", "Home", new { success = "Zmiany zostały zapisane." });
            }
            catch (Exception ex)
            {
                FileLogger.LogMessage("Error: " + ex.ToString(), 3);
                return View("~/Views/Shared/Error.cshtml");
            }
        }

    }
}