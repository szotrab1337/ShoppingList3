using Newtonsoft.Json;
using ShoppingListWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace ShoppingListWebAPI.Controllers
{
    public class ShopsController : ApiController
    {
        // GET api/shops
        [HttpGet]
        public string Get()
        {
            var settings = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.None,
                Error = (sender, args) =>
                {
                    args.ErrorContext.Handled = true;
                },
            };

            Context context = new Context();
            List<Shop> shops = context.Shops.ToList();

            return JsonConvert.SerializeObject(shops, settings);
        }

        // GET api/shops/5
        [HttpGet]
        public string Get(int id)
        {
            var settings = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.None,
                Error = (sender, args) =>
                {
                    args.ErrorContext.Handled = true;
                },
            };

            Context context = new Context();
            Shop shop = context.Shops.FirstOrDefault(x => x.ShopId == id);
            
            return JsonConvert.SerializeObject(shop, settings);
        }

        // POST api/shops/1
        [HttpPost]
        public void Post(int id, [FromBody] Shop shop)
        {
            using (Context context = new Context())
            {
                shop.CreatedOn = DateTime.Now;

                context.Shops.AddOrUpdate(shop);
                context.SaveChanges();
            }
        }

        // DELETE api/shops/5
        public void Delete(int id)
        {
            using (Context context = new Context())
            {
                Shop shop = context.Shops.FirstOrDefault(x => x.ShopId == id);

                context.Items.RemoveRange(shop.Items);
                context.Shops.Remove(context.Shops.FirstOrDefault(x => x.ShopId == id));
                context.SaveChanges();
            }
        }
    }
}