using PagedList;
using ShoppingListWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShoppingListWeb.Controllers
{
    public class RecipesController : Controller
    {
        public ActionResult List(int? page)
        {
            try
            {
                if (Request.QueryString["error"] != null)
                    ViewBag.Error = Request.QueryString["error"];

                if (Request.QueryString["success"] != null)
                    ViewBag.Success = Request.QueryString["success"];

                Context context= new Context();
                List<Recipe> recipes = context.Recipes.OrderBy(x => x.Name).ToList();

                int pageSize = 6;
                int pageNumber = (page ?? 1);

                return View(recipes.ToPagedList(pageNumber, pageSize));
            }
            catch (Exception ex)
            {
                FileLogger.LogMessage("Error: " + ex.ToString(), 3);
                return View("~/Views/Shared/Error.cshtml");
            }
        }

        public ActionResult RecipeDelete(int? id)
        {
            try
            {
                if (id is null)
                {
                    ViewBag.Error = "Wystąpił błąd.";
                    return RedirectToAction("List", new { error = ViewBag.Error });
                }

                Context context = new Context();
                Recipe recipe = context.Recipes.FirstOrDefault(x => x.RecipeId == id);

                return View(recipe);
            }
            catch (Exception ex)
            {
                FileLogger.LogMessage("Error: " + ex.ToString(), 3);
                return View("~/Views/Shared/Error.cshtml");
            }
        }

        [HttpPost]
        public ActionResult RecipeDelete(Recipe recipe)
        {
            try
            {
                Context context = new Context();
                Recipe recipeToDelete = context.Recipes.FirstOrDefault(x => x.RecipeId == recipe.RecipeId);

                if(recipeToDelete is null)
                {
                    ViewBag.Error = "Wystąpił błąd.";
                    return RedirectToAction("List", new { error = ViewBag.Error });
                }

                context.Recipes.Remove(recipeToDelete);
                context.SaveChanges();

                ViewBag.Success = "Pomyślnie usunięto przepis.";

                return RedirectToAction("List", new { success = ViewBag.Success });
            }
            catch (Exception ex)
            {
                FileLogger.LogMessage("Error: " + ex.ToString(), 3);
                return View("~/Views/Shared/Error.cshtml");
            }
        }

        public ActionResult RecipeDetails(int? id)
        {
            try
            {
                if (id is null)
                {
                    ViewBag.Error = "Wystąpił błąd.";
                    return RedirectToAction("List", new { error = ViewBag.Error });
                }

                Context context = new Context();
                Recipe recipe = context.Recipes.FirstOrDefault(x => x.RecipeId == id);

                return View(recipe);
            }
            catch (Exception ex)
            {
                FileLogger.LogMessage("Error: " + ex.ToString(), 3);
                return View("~/Views/Shared/Error.cshtml");
            }
        }
        
        public ActionResult RecipeEdit(int? id)
        {
            try
            {
                if (id is null)
                {
                    ViewBag.Error = "Wystąpił błąd.";
                    return RedirectToAction("List", new { error = ViewBag.Error });
                }

                Context context = new Context();
                Recipe recipe = context.Recipes.FirstOrDefault(x => x.RecipeId == id);

                return View(recipe);
            }
            catch (Exception ex)
            {
                FileLogger.LogMessage("Error: " + ex.ToString(), 3);
                return View("~/Views/Shared/Error.cshtml");
            }
        }
    }
}