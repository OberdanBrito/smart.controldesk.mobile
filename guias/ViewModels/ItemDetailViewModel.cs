using System;

using guias.Models;

namespace guias.ViewModels
{
    public class ItemDetailViewModel : BaseViewModel
    {
        public Item Item { get; set; }
        public ItemDetailViewModel(Item item = null)
        {
            Title = item?.assunto;
            Item = item;
        }
    }
}
