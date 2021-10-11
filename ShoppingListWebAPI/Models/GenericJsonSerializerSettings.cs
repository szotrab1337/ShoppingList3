using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShoppingListWebAPI.Models
{
    public static class GenericJsonSerializerSettings
    {
        public static JsonSerializerSettings GetSettings()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.None,
                Error = (sender, args) =>
                {
                    args.ErrorContext.Handled = true;
                },
            };

            return settings;
        }
    }
}