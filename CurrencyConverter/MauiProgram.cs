using CurrencyConverter.Services;
using CurrencyConverter.ViewModels;
using Microsoft.Extensions.Logging;

namespace CurrencyConverter
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            builder.Services.AddSingleton<IExchangeRateService, CbrExchangeRateService>();
            builder.Services.AddTransient<CurrencyConverterViewModel>();
            builder.Services.AddTransient<MainPage>();

            return builder.Build();
        }
    }
}
