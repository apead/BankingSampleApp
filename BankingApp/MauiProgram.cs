using BankingApp.Services;
using BankingApp.Views;
using Microsoft.Extensions.Logging;

namespace BankingApp
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
            
            builder.Services.AddSingleton< BankingDatabaseService>();

            builder.Services.AddTransient<CustomerListView>();

            return builder.Build();
        }
    }
}
