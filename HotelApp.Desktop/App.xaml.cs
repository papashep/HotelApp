using System.IO;
using System.Windows;
using HotelAppLibrary.Data;
using HotelAppLibrary.Databases;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HotelApp.Desktop;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
/* 
 * NuGet Packages:
 * Microsoft.Extensions.Configuration.Binder
 * Microsoft.Extensions.Configuration.Json
 * Microsoft.Extensions.DependencyInjection
 */
public partial class App : Application
{
   public static ServiceProvider serviceProvider;

   protected override void OnStartup( StartupEventArgs e )
   {
      base.OnStartup( e );

      var services = new ServiceCollection();
      services.AddTransient<MainWindow>();  // This gives back an instance of MainWIndow,
                                            // this way you can have more than one
                                            // instance of MainWindow which you probably
                                            // won't need.

      services.AddTransient<CheckInForm>(); // Add to dependency injection

      services.AddTransient<ISqlDataAccess, SqlDataAccess>();
      services.AddTransient<ISqliteDataAccess, SqliteDataAccess>();

      var builder = new ConfigurationBuilder()
         .SetBasePath( Directory.GetCurrentDirectory() )
         .AddJsonFile( "appsettings.json" );


      IConfiguration config = builder.Build();

      string? dbChoice = config.GetValue<string>( "DatabaseChoice" ).ToLower();
      if ( dbChoice == "sql" )
      {
         services.AddTransient<IDatabaseData, SqlData>();
      }
      else if ( dbChoice == "sqlite" )
      {
         services.AddTransient<IDatabaseData, SqliteData>();
      }
      else
      {
         // Fallback // Default value
         services.AddTransient<IDatabaseData, SqlData>();
      }

      services.AddSingleton( config ); // Get the same instance each time

     serviceProvider = services.BuildServiceProvider();
      var mainWindow = serviceProvider.GetService<MainWindow>(); // Instance of
                                                                           // MainWIndow
      mainWindow.Show();




   }
}
