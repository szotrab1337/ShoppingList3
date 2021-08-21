using ShoppingListWeb.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Globalization;
using System.Linq;
using System.Threading;
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
                    Context.Shops.AddOrUpdate(shop);

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

                return RedirectToActionPermanent("EditShoppingList", new {id = shop.ShopId, success = ViewBag.Success });
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

                shop.ModifiedOn = DateTime.Now;
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

        public ActionResult EditItem(int? itemId, int? shopId)
        {
            try
            {
                if (itemId is null && shopId is null)
                {
                    ViewBag.Error = "Wystąpił błąd.";
                    return RedirectToAction("List", new { error = ViewBag.Error });
                }

                Item item = new Item();
                Shop shop = new Shop();

                if (itemId == null && shopId != null)
                {
                    shop.ShopId = 0;
                    item.ShopId = shopId.Value;
                    return View(item);
                }

                Context = new Context();

                item = Context.Items.FirstOrDefault(x => x.ItemId == itemId);
                shop = Context.Shops.FirstOrDefault(x => x.ShopId == shopId);

                if (item is null || shop is null)
                {
                    ViewBag.Error = "Brak " + (item is null ? "przedmiotu" : "listy") + " w bazie danych.";
                    return RedirectToAction("List", new { error = ViewBag.Error });
                }

                return View(item);
            }
            catch (Exception ex)
            {
                FileLogger.LogMessage("Error: " + ex.ToString(), 3);
                return View("~/Views/Shared/Error.cshtml");
            }
        }

        [HttpPost]
        public ActionResult EditItem(Item item)
        {
            try
            {
                if (item is null)
                {
                    ViewBag.Error = "Wystąpił błąd.";
                    return RedirectToActionPermanent("List", new { error = ViewBag.Error });
                }

                Context = new Context();

                if(item.ValidateItem() != string.Empty)
                {
                    ViewBag.Error = item.ValidateItem();
                    return View("EditItem", item);
                }

                if (item.ItemId == 0)
                {
                    item.CreatedOn = DateTime.Now;

                    if (item.Quantity == null)
                        item.UnitId = null;

                    Context.Items.AddOrUpdate(item);

                    ViewBag.Success = "Pomyślnie dodano nowy przedmiot.";
                }

                else
                {
                    Item editedItem = Context.Items.FirstOrDefault(x => x.ItemId == item.ItemId);

                    editedItem.ModifiedOn = DateTime.Now;
                    editedItem.Name = item.Name;
                    editedItem.Description = item.Description;
                    editedItem.Quantity = item.Quantity;
                    editedItem.UnitId = item.UnitId;

                    if (editedItem.Quantity == null)
                        editedItem.UnitId = null;

                    ViewBag.Success = "Edycja zakończona powodzeniem.";
                }
                Shop shop = Context.Shops.FirstOrDefault(x => x.ShopId == item.ShopId);
                shop.ModifiedOn = DateTime.Now;

                Context.SaveChanges();

                return RedirectToActionPermanent("EditShoppingList", new { id = item.ShopId, success = ViewBag.Success });
            }
            catch (Exception ex)
            {
                FileLogger.LogMessage("Error: " + ex.ToString(), 3);
                return View("~/Views/Shared/Error.cshtml");
            }
        }
    }
}