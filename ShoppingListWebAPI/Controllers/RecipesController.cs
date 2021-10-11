using Newtonsoft.Json;
using ShoppingListWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace ShoppingListWebAPI.Controllers
{
    public class RecipesController : ApiController
    {
        // GET api/recipes
        [HttpGet]
        public HttpResponseMessage Get()
        {
            Context context = new Context();
            List<Recipe> recipes = context.Recipes.ToList();

            return new HttpResponseMessage()
            {
                Content = new StringContent(JsonConvert.SerializeObject(recipes, GenericJsonSerializerSettings.GetSettings()),
                    System.Text.Encoding.UTF8, "application/json")
            };
        }

        // GET api/recipes/5
        [HttpGet]
        public HttpResponseMessage Get(int id)
        {
            Context context = new Context();
            Recipe recipe = context.Recipes.FirstOrDefault(x => x.RecipeId == id);

            return new HttpResponseMessage()
            {
                Content = new StringContent(JsonConvert.SerializeObject(recipe, GenericJsonSerializerSettings.GetSettings()),
                    System.Text.Encoding.UTF8, "application/json")
            };
        }

        // POST api/recipes/1
        [HttpPost]
        public void Post(int id, [FromBody] Recipe recipe)
        {
            using (Context context = new Context())
            {
                recipe.CreatedOn = DateTime.Now;

                context.Recipes.AddOrUpdate(recipe);
                context.SaveChanges();
            }
        }

        //DELETE api/recipes/5
        [HttpDelete]
        public void Delete(int id)
        {
            using (Context context = new Context())
            {
                Recipe recipe = context.Recipes.FirstOrDefault(x => x.RecipeId == id);

                context.MakingSteps.RemoveRange(recipe.MakingSteps);
                context.Ingredients.RemoveRange(recipe.Ingredients);
                context.Recipes.Remove(context.Recipes.FirstOrDefault(x => x.RecipeId == id));
                context.SaveChanges();
            }
        }
    }
}