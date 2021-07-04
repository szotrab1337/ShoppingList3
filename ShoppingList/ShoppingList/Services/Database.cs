using ShoppingList.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
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

        //public Task<Item> GetItemAsync(Item item)
        //{
        //    return _database.Table<Item>().Where(x => x.ItemID == item.ItemID).FirstOrDefaultAsync();
        //}
        //public Task<Shop> GetShopAsync(Shop shop)
        //{
        //    return _database.Table<Shop>().Where(x => x.ShopID == shop.ShopID).FirstOrDefaultAsync();
        //}

        //public Task<Shop> GetShopByID(int id)
        //{
        //    return _database.Table<Shop>().Where(x => x.ShopID == id).FirstOrDefaultAsync();
        //}

        //public Task<Shop> GetShopByName(string Name)
        //{
        //    return _database.Table<Shop>().Where(x => x.Name == Name).FirstOrDefaultAsync();
        //}



        //public Task UpdateItemAsync(Item item)
        //{
        //    return _database.UpdateAsync(item);
        //}

        //public Task InsertItemAsync(Item item)
        //{
        //    return _database.InsertOrReplaceAsync(item);
        //}
        //public Task InsertShopAsync(Shop shop)
        //{
        //    return _database.InsertOrReplaceAsync(shop);
        //}
        //public Task DeleteItemAsync(Item item)
        //{
        //    return _database.DeleteAsync(item);
        //}
        //public Task DeleteShopAsync(Shop shop)
        //{
        //    return _database.DeleteAsync(shop);
        //}

        //public Task<Setting> GetSettingsAsync()
        //{
        //    return _database.Table<Setting>().FirstOrDefaultAsync();
        //}

        //public Task InsertSettingAsync(Setting setting)
        //{
        //    return _database.InsertAsync(setting);
        //}

        //public Task UpdateSettingAsync(Setting setting)
        //{
        //    return _database.UpdateAsync(setting);
        //}
    }
}
