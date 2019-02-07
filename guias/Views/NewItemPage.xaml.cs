using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using guias.Models;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace guias.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewItemPage : ContentPage
    {
        public Item Item { get; set; }
        List<Tipos> tipos;
        List<Clientes> clientes;
        List<Categorias> categorias;
        RestService ws = new RestService();

        public NewItemPage()
        {
            PreparaForm();
            Item = new Item
            {
                filedate = new DateTimeOffset(DateTime.UtcNow).DateTime,
                fechamento = new DateTimeOffset(DateTime.UtcNow).DateTime
            };

            this._timefechamento.Time = DateTime.UtcNow.TimeOfDay;
            this._timefiledate.Time = DateTime.UtcNow.TimeOfDay;

            BindingContext = this;
        }

        public NewItemPage(Item commandParameter)
        {
            PreparaForm();
            Item = commandParameter;

            this._timefechamento.Time = Item.filedate.TimeOfDay;
            this._timefiledate.Time = Item.fechamento.TimeOfDay;

            BindingContext = this;
        }

        private void PreparaForm()
        {
            InitializeComponent();

            tipos = new List<Tipos>();
            clientes = new List<Clientes>();
            categorias = new List<Categorias>();

            Task.Run(async () =>
            {
                var listaTipos = await ws.QuerySelect("controldesk.atendimento_tipos");

                foreach (var item in listaTipos)
                {
                    Tipos itemtipo = (Tipos)JsonConvert.DeserializeObject<Tipos>(item.ToString());
                    tipos.Add(itemtipo);
                   pckTipo.Items.Add(itemtipo.nome);
                }

                var listaClientes = await ws.QuerySelect("controldesk.cliente");

                foreach (var item in listaClientes)
                {
                    Clientes cliente = (Clientes)JsonConvert.DeserializeObject<Clientes>(item.ToString());
                    clientes.Add(cliente);
                    pckCliente.Items.Add(cliente.nome);
                }

                var listaCategorias = await ws.QuerySelect("controldesk.atendimento_categoria");

                foreach (var item in listaCategorias)
                {
                    Categorias categoria = (Categorias)JsonConvert.DeserializeObject<Categorias>(item.ToString());
                    categorias.Add(categoria);
                    pckCategoria.Items.Add(categoria.nome);
                }

            }).Wait();

            this.pckFiledate.MinimumDate = new DateTime(DateTime.UtcNow.Year, 1, 1);
            this.pckFechamento.MinimumDate = new DateTime(DateTime.UtcNow.Year, 1, 1);

        }

        async void Save_Clicked(object sender, EventArgs e)
        {
            Task.Run(async () =>
            {
                Dictionary<string, string> campos = new Dictionary<string, string>
                {
                    ["filedate"] = Item.filedate.ToString("yyyy-MM-dd") + " " + _timefiledate.Time,
                    ["chamado"] = Item.chamado,
                    ["resposta"] = Item.resposta,
                    ["fechamento"] = Item.fechamento.ToString("yyyy-MM-dd") + " " + _timefechamento.Time,
                    ["tecnico"] = Item.tecnico,
                    ["categoria"] = categorias.Find(x => x.nome == Item.categoria).id.ToString(),
                    ["tipo"] = tipos.Find(x => x.nome == Item.tipo).id.ToString(),
                    ["responsavel"] = Item.responsavel,
                    ["cliente"] = clientes.Find(x => x.nome == Item.cliente).id.ToString(),
                    ["assunto"] = Item.assunto
                };

                if (Item.id > 0)
                {
                    await ws.QueryUpdate("controldesk.atendimento_historico", campos, Item.id, "id");
                }
                else
                {
                    await ws.QueryInsert("controldesk.atendimento_historico", campos, "id");
                }


            }).Wait();

            await App.Database.SaveItemAsync(Item);
            MessagingCenter.Send(this, "AddItem", Item);

            await Navigation.PopModalAsync();
        }

        async void Cancelar_Clicked(object sender, EventArgs e)
        {
            MessagingCenter.Send(this, "AddItem", Item);

            await Navigation.PopModalAsync();
        }

    }

}

