using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;
using guias.Services;
using guias.Models;

namespace guias.Services
{
    public class TodoItemDatabase
    {
        readonly SQLiteAsyncConnection database;

        public TodoItemDatabase(string dbPath)
        {
            database = new SQLiteAsyncConnection(dbPath);
            database.CreateTableAsync<Item>().Wait();
        }

        public Task<List<Item>> GetItemsAsync()
        {
            return database.Table<Item>().ToListAsync();
        }

        public Task<List<Item>> ObterRegistros()
        {
            return database.QueryAsync<Item>("SELECT * FROM [Item] ORDER BY id DESC");
        }

        public Task<List<Item>> MaxId()
        {
            return database.QueryAsync<Item>("SELECT * FROM [Item] ORDER BY id DESC LIMIT 1");
        }


        public Task<Item> GetItemAsync(int id)
        {
            return database.Table<Item>().Where(i => i.dbid == id).FirstOrDefaultAsync();
        }

        public Task<int> GetNumRows()
        {
            return database.Table<Item>().CountAsync();
        }

        public Task<int> Adicionar(Item item)
        {
            return database.InsertAsync(item);
        }

        public Task<int> SaveItemAsync(Item item)
        {
            if (item.id != 0)
            {
                return database.UpdateAsync(item);
            }
            else
            {
                return database.InsertAsync(item);
            }
        }

        public Task<int> DeleteItemAsync(Item item)
        {
            return database.DeleteAsync(item);
        }
    }
}
