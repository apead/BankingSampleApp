using BankingApp.Services;
using BankingApp.Views;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace BankingApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var culture = new CultureInfo("en-ZA");
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
         
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
            
            builder.Services.AddSingleton< BankingDatabaseService>();

            builder.Services.AddTransient<CustomerListView>();
            builder.Services.AddTransient<CustomerAccountsView>();
            builder.Services.AddTransient<BankAccountTransactionView>();
            builder.Services.AddTransient<TransactionRecordView>();

            return builder.Build();
        }
    }
}
