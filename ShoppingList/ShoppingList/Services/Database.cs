using ShoppingList.Models;
using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShoppingList.Services
{
    public class Database
    {
        readonly SQLiteAsyncConnection _database;

        public Database(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<Shop>().Wait();
            _database.CreateTableAsync<Item>().Wait();
        }

        public Task<List<Shop>> GetShopsAsync()
        {
            return _database.Table<Shop>().OrderBy(x => x.Number).ToListAsync();
        }
        
        public Task<Shop> GetShopByNameAsync(string shopName)
        {
            return _database.Table<Shop>().FirstOrDefaultAsync(x => x.Name == shopName);
        }

        public Task InsertShopAsync(Shop shop)
        {
            return _database.InsertAsync(shop);
        }

        public Task UpdateShopAsync(Shop shop)
        {
            return _database.UpdateAsync(shop);
        }

        public Task UpdateShopsAsync(List<Shop> shops)
        {
            return _database.UpdateAllAsync(shops);
        }

        public Task DeleteShopAsync(Shop shop)
        {
            return _database.DeleteAsync(shop);
        }

        public Task DeleteItemAsync(Item item)
        {
            return _database.DeleteAsync(item);
        }

        public Task DeleteItemsAsync(List<Item> items)
        {
            foreach (Item item in items)
            {
                _database.DeleteAsync(item);
            }
            return null;
        }

        public Task<List<Item>> GetShopItemsAsync(int shopId)
        {
            return _database.Table<Item>().Where(x => x.ShopId == shopId).ToListAsync();
        }

        public Task InsertItemAsync(Item item)
        {
            return _database.InsertAsync(item);
        }    

        public Task UpdateItemAsync(Item item)
        {
            return _database.UpdateAsync(item);
        }

        public Task UpdateItemsAsync(List<Item> items)
        {
            return _database.UpdateAllAsync(items);
        }
    }
}
