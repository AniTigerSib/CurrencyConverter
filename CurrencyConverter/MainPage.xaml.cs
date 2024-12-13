using CurrencyConverter.Services;
using CurrencyConverter.ViewModels;

namespace CurrencyConverter
{
    public partial class MainPage : ContentPage
    {
        public MainPage(CurrencyConverterViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }

}
