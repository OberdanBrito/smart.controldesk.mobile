using System;
using System.Collections.Generic;
using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace guias.Views
{
    public partial class Settings : ContentPage
    {
        public Settings()
        {
            InitializeComponent();
        }

        async void ConfigurarClientes(object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new NavigationPage(new SettingsClientes()));
        }

        async void ConfigurarContratos(object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new NavigationPage(new SettingsContratos()));
        }

        async void ConfigurarTipos(object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new NavigationPage(new SettingsTipos()));
        }
    }

    [ContentProperty(nameof(Source))]
    public class ImageResourceExtension : IMarkupExtension
    {
        public string Source { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Source == null)
            {
                return null;
            }

            // Do your translation lookup here, using whatever method you require
            var imageSource = ImageSource.FromResource(Source, typeof(ImageResourceExtension).GetTypeInfo().Assembly);

            return imageSource;
        }
    }
}
