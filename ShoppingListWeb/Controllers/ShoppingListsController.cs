using ShoppingListWeb.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShoppingListWeb.Controllers
{
    public class ShoppingListsController : Controller
    {
        Context Context;
        public ActionResult List()
        {
            try
            {
                if (Request.QueryString["error"] != null)
                    ViewBag.Error = Request.QueryString["error"];
                
                if (Request.QueryString["success"] != null)
                    ViewBag.Success = Request.QueryString["success"];

                Context = new Context();
                List<Shop> shops = Context.Shops.OrderByDescending(x => x.CreatedOn).ToList();

                return View(shops);
            }
            catch (Exception ex)
            {
                FileLogger.LogMessage("Error: " + ex.ToString(), 3);
                return View("~/Views/Shared/Error.cshtml");
            }
        }

        public ActionResult EditShoppingList(int? id)
        {
            try
            {
                if (Request.QueryString["success"] != null)
                    ViewBag.Success = Request.QueryString["success"];

                Shop shop = new Shop();

                if (id == null)
                {
                    shop.ShopId = 0;
                    return View(shop);
                }

                Context = new Context();
                shop = Context.Shops.FirstOrDefault(x => x.ShopId == id);

                if (shop == null)
                {
                    ViewBag.Error = "Brak listy w bazie danych.";
                    return RedirectToAction("List", new { error = ViewBag.Error });
                }

                return View(shop);
            }
            catch (Exception ex)
            {
                FileLogger.LogMessage("Error: " + ex.ToString(), 3);
                return View("~/Views/Shared/Error.cshtml");
            }
        }

        [HttpPost]
        public ActionResult EditShoppingList(Shop shop)
        {
            try
            {
                if (shop is null)
                {
                    ViewBag.Error = "Wystąpił błąd.";
                    return RedirectToActionPermanent("List", new { error = ViewBag.Error });
                }

                Context = new Context();

                if (string.IsNullOrEmpty(shop.Name))
                {
                    if(shop.ShopId > 0)
                    shop = Context.Shops.FirstOrDefault(x => x.ShopId == shop.ShopId);
                    shop.Name = string.Empty;

                    ViewBag.Error = "Nazwa sklepu nie może być pusta.";

                    return View("EditShoppingList", shop);
                }
                if (shop.ShopId == 0)
                {
                    shop.CreatedOn = DateTime.Now;
                    Context.Shops.Add(shop);

                    ViewBag.Success = "Pomyślnie dodano nowy sklep.";
                }

                else
                {
                    Shop editedShop = Context.Shops.FirstOrDefault(x => x.ShopId == shop.ShopId);

                    editedShop.ModifiedOn = DateTime.Now;
                    editedShop.Name = shop.Name;

                    Context.Shops.AddOrUpdate(editedShop);

                    ViewBag.Success = "Edycja zakończona powodzeniem.";
                }
                Context.SaveChanges();

                return RedirectToActionPermanent("List", new { success = ViewBag.Success });
            }
            catch (Exception ex)
            {
                FileLogger.LogMessage("Error: " + ex.ToString(), 3);
                return View("~/Views/Shared/Error.cshtml");
            }
        }

        public ActionResult DeleteItem(int? itemId, int? shopId)
        {
            try
            {
                if (itemId is null || shopId is null)
                {
                    ViewBag.Error = "Wystąpił błąd.";
                    return RedirectToAction("List", new { error = ViewBag.Error });
                }

                Context = new Context();

                Item item = Context.Items.FirstOrDefault(x => x.ItemId == itemId);
                Shop shop = Context.Shops.FirstOrDefault(x => x.ShopId == shopId);

                if (item is null || shop is null)
                {
                    ViewBag.Error = "Brak " + (item is null ? "przedmiotu" : "listy") + " w bazie danych.";
                    return RedirectToAction("List", new { error = ViewBag.Error });
                }

                Context.Items.Remove(item);
                Context.SaveChanges();

                ViewBag.Success = "Pomyślnie usunięto przedmiot.";

                return RedirectToAction("EditShoppingList", new { id = shopId, success = ViewBag.Success });
            }
            catch (Exception ex)
            {
                FileLogger.LogMessage("Error: " + ex.ToString(), 3);
                return View("~/Views/Shared/Error.cshtml");
            }
        }

        public ActionResult DeleteShoppingList(int? id)
        {
            try
            {
                if (id is null)
                {
                    ViewBag.Error = "Wystąpił błąd.";
                    return RedirectToAction("List", new { error = ViewBag.Error });
                }

                Context = new Context();

                Shop shop = Context.Shops.FirstOrDefault(x => x.ShopId == id);

                if (shop is null)
                {
                    ViewBag.Error = "Brak sklepu w bazie danych.";
                    return RedirectToAction("List", new { error = ViewBag.Error });
                }

                Context.Items.RemoveRange(shop.Items);
                Context.Shops.Remove(shop);
                Context.SaveChanges();

                ViewBag.Success = "Pomyślnie usunięto sklep.";

                return RedirectToAction("List", new { success = ViewBag.Success });
            }
            catch (Exception ex)
            {
                FileLogger.LogMessage("Error: " + ex.ToString(), 3);
                return View("~/Views/Shared/Error.cshtml");
            }
        }
    }
}