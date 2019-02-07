using System;
using System.Diagnostics;
using System.Reflection;
using SQLite;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace guias.Models
{
    public class LastId
    {
        public int id { get; set; }
    }

    public class Item
    {
        [PrimaryKey, AutoIncrement]
        public int dbid { get; set; }

        public int id { get; set; }
        public int dias { get; set; }
        public DateTime filedate { get; set; }
        public string DataAtendimento
        {
            get
            {
                return filedate.ToString("dd/MM/yyyy HH:mm");
            }
        }

        public string assunto { get; set; }
        public string chamado { get; set; }
        public string resposta { get; set; }

        public string RespostaChamado
        {
            get
            {
                if (resposta.Length >= 100)
                {
                    return $"{resposta.Substring(0, 100)}...";
                }
                else
                {
                    return resposta;
                }
            }
        }

        public string categoria { get; set; }
        public string cliente { get; set; }
        public DateTime fechamento { get; set; }
        public string tecnico { get; set; }
        public string tipo { get; set; }
        public string icone { get; set; }
        public string IconeChamado { get; set; }
        public string classtipo { get; set; }
        public ImageSource Image
        {
            get
            {
                return ImageSource.FromResource($"guias.img.{icone}", typeof(ImageResourceExtension).GetTypeInfo().Assembly);
            }
        }

        public ImageSource TipoAtendimento
        {
            get
            {
                if (classtipo != null)
                {
                    return ImageSource.FromResource("guias.img.24horas.png", typeof(ImageResourceExtension).GetTypeInfo().Assembly);
                }
                else
                {
                    return null;
                }

            }
        }

        public string responsavel { get; set; }
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