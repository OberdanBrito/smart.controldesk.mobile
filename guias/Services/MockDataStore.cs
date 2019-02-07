using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using guias.Models;
using Newtonsoft.Json;

namespace guias.Services
{
    public class MockDataStore : IDataStore<Item>
    {
        List<Item> items;
        RestService ws = new RestService();

        public MockDataStore()
        {
            items = new List<Item>();

            Task.Run(async () =>
            {
                int registros = await App.Database.GetNumRows();

                if (registros == 0)
                {
                    await DownloadAtendimentosAsync(0);
                }
                else
                {
                    List<Item> LastDBAtendimento = await App.Database.MaxId();
                    LastId LastServerAtendimento = await LastServerIdAsync();

                    if (LastServerAtendimento.id > LastDBAtendimento[0].id)
                    {
                        await DownloadAtendimentosAsync(LastDBAtendimento[0].id);
                        await CarregarViaBancolocalAsync();
                    }
                    else
                    {
                        await CarregarViaBancolocalAsync();
                    }

                }


            }).Wait();

        }

        async Task CarregarViaBancolocalAsync()
        {
            var dados = await App.Database.ObterRegistros();
            foreach (var item in dados)
            {
                items.Add(item);
            }
        }

        async Task<LastId> LastServerIdAsync()
        {
            var dados = await ws.QuerySelect("controldesk.mb_guiaslista_atendimentos_id");
            LastId info = null;
            foreach (var item in dados)
            {
                info = JsonConvert.DeserializeObject<LastId>(item.ToString());
            }
            return info;
        }

        async Task DownloadAtendimentosAsync(int id)
        {
            var dados = await ws.QuerySelect("controldesk.mb_guiaslista_atendimentos", $"id > {id}");

            foreach (var item in dados)
            {
                Item info = (Item)JsonConvert.DeserializeObject<Item>(item.ToString());
                items.Add(info);
                await App.Database.Adicionar(info);
            }
        }

        public async Task<bool> AddItemAsync(Item item)
        {
            items.Add(item);
            await App.Database.SaveItemAsync(item);
            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(Item item)
        {
            var oldItem = items.Where((Item arg) => arg.id == item.id).FirstOrDefault();
            items.Remove(oldItem);
            items.Add(item);
            await App.Database.SaveItemAsync(item);
            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            var oldItem = items.Where((Item arg) => arg.id == int.Parse(id)).FirstOrDefault();
            items.Remove(oldItem);
            await App.Database.DeleteItemAsync(oldItem);
            return await Task.FromResult(true);
        }

        public async Task<Item> GetItemAsync(string id)
        {
            return await Task.FromResult(items.FirstOrDefault(s => s.id == int.Parse(id)));
        }

        public async Task<IEnumerable<Item>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(items);
        }
    }


}