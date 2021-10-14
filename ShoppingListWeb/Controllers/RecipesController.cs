using PagedList;
using ShoppingListWeb.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShoppingListWeb.Controllers
{
    public class RecipesController : Controller
    {
        Context Context;

        public ActionResult List(int? page)
        {
            try
            {
                if (Request.QueryString["error"] != null)
                    ViewBag.Error = Request.QueryString["error"];

                if (Request.QueryString["success"] != null)
                    ViewBag.Success = Request.QueryString["success"];

                Context = new Context();
                List<Recipe> recipes = Context.Recipes.OrderBy(x => x.Name).ToList();

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

                Context = new Context();
                Recipe recipe = Context.Recipes.FirstOrDefault(x => x.RecipeId == id);

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
                Context = new Context();
                Recipe recipeToDelete = Context.Recipes.FirstOrDefault(x => x.RecipeId == recipe.RecipeId);

                if(recipeToDelete is null)
                {
                    ViewBag.Error = "Wystąpił błąd.";
                    return RedirectToAction("List", new { error = ViewBag.Error });
                }

                Context.Recipes.Remove(recipeToDelete);
                Context.SaveChanges();

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

                Context = new Context();
                Recipe recipe = Context.Recipes.FirstOrDefault(x => x.RecipeId == id);

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
                    Recipe newRecipe = new Recipe();
                    return View(newRecipe);
                }

                if (Request.QueryString["error"] != null)
                    ViewBag.Error = Request.QueryString["error"];

                if (Request.QueryString["success"] != null)
                    ViewBag.Success = Request.QueryString["success"];

                Context = new Context();
                Recipe recipe = Context.Recipes.FirstOrDefault(x => x.RecipeId == id);

                return View(recipe);
            }
            catch (Exception ex)
            {
                FileLogger.LogMessage("Error: " + ex.ToString(), 3);
                return View("~/Views/Shared/Error.cshtml");
            }
        }

        public ActionResult IngredientDelete(int? id)
        {
            try
            {
                if (id is null)
                {
                    ViewBag.Error = "Wystąpił błąd.";
                    return RedirectToAction("List", new { error = ViewBag.Error });
                }

                Context = new Context();
                Ingredient ingredient = Context.Ingredients.FirstOrDefault(x => x.IngredientId == id);

                Context.Ingredients.Remove(ingredient);
                Context.SaveChanges();

                ViewBag.Success = "Składnik został usunięty.";

                return RedirectToAction("RecipeEdit", new { id = ingredient.RecipeId, success = ViewBag.Success });
            }
            catch (Exception ex)
            {
                FileLogger.LogMessage("Error: " + ex.ToString(), 3);
                return View("~/Views/Shared/Error.cshtml");
            }
        }
        
        public ActionResult MakingStepDelete(int? id)
        {
            try
            {
                if (id is null)
                {
                    ViewBag.Error = "Wystąpił błąd.";
                    return RedirectToAction("List", new { error = ViewBag.Error });
                }

                Context = new Context();
                MakingStep makingStep = Context.MakingSteps.FirstOrDefault(x => x.MakingStepId == id);

                Context.MakingSteps.Remove(makingStep);
                Context.SaveChanges();

                ViewBag.Success = "Krok został usunięty.";

                return RedirectToAction("RecipeEdit", new { id = makingStep.RecipeId, success = ViewBag.Success });
            }
            catch (Exception ex)
            {
                FileLogger.LogMessage("Error: " + ex.ToString(), 3);
                return View("~/Views/Shared/Error.cshtml");
            }
        }
        
        public ActionResult IngredientEdit(int? id, int? recipeId)
        {
            try
            {
                if (recipeId is null)
                {
                    ViewBag.Error = "Wystąpił błąd.";
                    return RedirectToAction("List", new { error = ViewBag.Error });
                }

                Ingredient ingredient = new Ingredient();
                Context = new Context();

                if (id != null)
                    ingredient = Context.Ingredients.FirstOrDefault(x => x.IngredientId == id);

                ViewBag.RecipeId = recipeId.Value;

                return View(ingredient);
            }
            catch (Exception ex)
            {
                FileLogger.LogMessage("Error: " + ex.ToString(), 3);
                return View("~/Views/Shared/Error.cshtml");
            }
        }
        
        [HttpPost]
        public ActionResult IngredientEdit(Ingredient ingredient)
        {
            try
            {
                if (ingredient is null)
                {
                    ViewBag.Error = "Wystąpił błąd.";
                    return RedirectToActionPermanent("List", new { error = ViewBag.Error });
                }

                Context = new Context();

                if (ingredient.ValidateItem() != string.Empty)
                {
                    ViewBag.Error = ingredient.ValidateItem();
                    return View("IngredientEdit", ingredient);
                }

                if (ingredient.IngredientId== 0)
                {
                    if (ingredient.Quantity == null)
                        ingredient.UnitId = null;

                    Context.Ingredients.AddOrUpdate(ingredient);

                    ViewBag.Success = "Pomyślnie dodano nowy składnik.";
                }

                else
                {
                    Ingredient editedIngredient = Context.Ingredients.FirstOrDefault(x => x.IngredientId == ingredient.IngredientId);

                    editedIngredient.Name = ingredient.Name;
                    editedIngredient.Quantity = ingredient.Quantity;
                    editedIngredient.UnitId = ingredient.UnitId;

                    if (editedIngredient.Quantity == null)
                        editedIngredient.UnitId = null;

                    ViewBag.Success = "Edycja zakończona powodzeniem.";
                }

                Context.SaveChanges();

                return RedirectToActionPermanent("RecipeEdit", new { id = ingredient.RecipeId, success = ViewBag.Success });
            }
            catch (Exception ex)
            {
                FileLogger.LogMessage("Error: " + ex.ToString(), 3);
                return View("~/Views/Shared/Error.cshtml");
            }
        }
    }
}