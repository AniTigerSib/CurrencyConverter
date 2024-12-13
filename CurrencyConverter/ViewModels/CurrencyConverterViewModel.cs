using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CurrencyConverter.Models;
using CurrencyConverter.Services;

namespace CurrencyConverter.ViewModels
{
    public class CurrencyConverterViewModel : INotifyPropertyChanged
    {
        private readonly IExchangeRateService _exchangeRateService;
        private DateTime _selectedDate;
        private Currency _fromCurrency;
        private Currency _toCurrency;
        private string _fromCode;
        private string _toCode;
        private decimal _fromAmount;
        private decimal _toAmount;
        private ObservableCollection<Currency> _currencies;

        public event PropertyChangedEventHandler PropertyChanged;

        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                _selectedDate = value;
                Console.WriteLine(value);
                LoadExchangeRates();
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Currency> Currencies
        {
            get => _currencies;
            set
            {
                _currencies = value;
                OnPropertyChanged();
            }
        }

        public Currency FromCurrency
        {
            get => _fromCurrency;
            set
            {
                _fromCurrency = value;
                ConvertCurrency(true);
                OnPropertyChanged();
            }
        }

        public Currency ToCurrency
        {
            get => _toCurrency;
            set
            {
                _toCurrency = value;
                ConvertCurrency(false);
                OnPropertyChanged();
            }
        }

        public decimal FromAmount
        {
            get => _fromAmount;
            set
            {
                if (_fromAmount != value)
                {
                    _fromAmount = value;
                    ConvertCurrency(true);
                    OnPropertyChanged();
                }
            }
        }

        public decimal ToAmount
        {
            get => _toAmount;
            set
            {
                if (_toAmount != value)
                {
                    _toAmount = value;
                    ConvertCurrency(false);
                    OnPropertyChanged();
                }
            }
        }

        public ICommand ConvertCommand { get; }

        public CurrencyConverterViewModel(IExchangeRateService exchangeRateService)
        {
            _exchangeRateService = exchangeRateService;
            SelectedDate = DateTime.Today;
            Currencies = [];
            LoadExchangeRates();
        }

        private async void LoadExchangeRates()
        {
            _fromCode = FromCurrency?.CharCode;
            _toCode = ToCurrency?.CharCode;
            var flag = false;
            while (!flag)
            {
                try
                {
                    var response = await _exchangeRateService.GetExchangeRatesAsync(SelectedDate);
                    Console.WriteLine(response);
                    var currencyList = response.Valute.Values.ToList();
                    currencyList.Insert(0, new Currency { CharCode = "RUB", Name = "Российский рубль", Value = 1, Nominal = 1 });

                    Currencies.Clear();
                    foreach (var currency in currencyList)
                    {
                        Currencies.Add(currency);
                    }

                    flag = true;

                    FromCurrency = Currencies.FirstOrDefault(c => c.CharCode ==
                                                         (string.IsNullOrEmpty(_fromCode) ? "USD" : _fromCode));
                    ToCurrency = Currencies.FirstOrDefault(c => c.CharCode == (string.IsNullOrEmpty(_toCode) ? "RUB" : _toCode));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    SelectedDate = SelectedDate.AddDays(-1);
                    OnPropertyChanged();
                }
            }
        }

        private void ConvertCurrency(bool fromFrom)
        {
            if (FromCurrency != null && ToCurrency != null)
            {
                // Конвертация с учетом номиналов
                if (fromFrom)
                {
                    decimal rubAmount = FromAmount * FromCurrency.Value / FromCurrency.Nominal;
                    ToAmount = rubAmount / (ToCurrency.Value / ToCurrency.Nominal);
                }
                else
                {
                    decimal rubAmount = ToAmount * ToCurrency.Value / ToCurrency.Nominal;
                    FromAmount = rubAmount / (FromCurrency.Value / FromCurrency.Nominal);
                }
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
