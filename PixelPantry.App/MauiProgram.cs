using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PixelPantry.App.Services;
using PixelPantry.App.ViewModels;
using PixelPantry.DAL;

namespace PixelPantry.App
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

            builder.Services.AddDbContext<PixelPantryContext>(opt => opt.UseSqlServer("Server=mycloudpr4100;Database=PixelPantry;User Id=sa;Password=e3RV9kFu;TrustServerCertificate=True;"));

            builder.Services.AddSingleton<FileScannerService>();
            builder.Services.AddTransient<MainViewModel>();
            builder.Services.AddTransient<MainPage>();

            return builder.Build();
        }
    }
}
