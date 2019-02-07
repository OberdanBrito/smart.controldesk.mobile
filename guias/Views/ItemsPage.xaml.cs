using System;
using System.Threading.Tasks;
using guias.Models;
using guias.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace guias.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ItemsPage : ContentPage
    {
        ItemsViewModel viewModel;
        RestService ws = new RestService();
        SearchBar searchBar;

        public ItemsPage()
        {
            InitializeComponent();
            BindingContext = viewModel = new ItemsViewModel();
            ItemsListView.IsPullToRefreshEnabled = true;


        }

        async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            var item = args.SelectedItem as Item;
            if (item == null)
                return;

            await Navigation.PushAsync(new ItemDetailPage(new ItemDetailViewModel(item)));

            // Manually deselect item.
            ItemsListView.SelectedItem = null;
        }

        async void AddItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new NavigationPage(new NewItemPage()));
        }

        private void Localizar_Clicked(object sender, EventArgs e)
        {
            searchBar = new SearchBar
            {
                Placeholder = "Pesquisar chamado",
                SearchCommand = new Command(() => { })
            };

            layoutrolagem.Children.Add(searchBar);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ItemsListView.BeginRefresh();
            if (viewModel.Items.Count == 0)
                viewModel.LoadItemsCommand.Execute(null);
        }

        public async void OnClone(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            Item clone = (Item)mi.CommandParameter;
            clone.id = 0;
            await Navigation.PushModalAsync(new NavigationPage(new NewItemPage(clone)));
        }

        public async void OnEdit(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            await Navigation.PushModalAsync(new NavigationPage(new NewItemPage((Item)mi.CommandParameter)));
        }

        public async void OnDelete(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            ItemsListView.IsRefreshing = true;
            var answer = await DisplayAlert("Registro de Atendimento", "Você deseja remover este registro?", "Sim", "Não");
            if (answer)
            {
                Task.Run(async () =>
                {

                    Item item = (Item)mi.CommandParameter;
                    var resposta = await ws.QueryDelete("controldesk.atendimento_historico", item.id, "id");

                }).Wait();
                ItemsListView.BeginRefresh();
                await DisplayAlert("Registro de atendimento", "O registro foi removido com sucesso", "OK");
                ItemsListView.IsRefreshing = false;
            }

        }
    }
}