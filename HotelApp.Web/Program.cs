using HotelAppLibrary.Data;
using HotelAppLibrary.Databases;

using System.Configuration;

using ConfigurationManager = Microsoft.Extensions.Configuration.ConfigurationManager;


/* 
* NuGet Packages:
* Microsoft.DependencyValidation.Analyzers
*/

var builder = WebApplication.CreateBuilder( args );

/* 
A Singleton service is created when it is first requested. 
This same instance is then used by all the subsequent requests. 
So in general, a Singleton service is created only one time per application and that single 
instance is used throughout the application life time.
Singleton objects are the same for every object and every request.
Whereas
Transient objects are always different; a new instance is provided to every controller and 
every service.
Scoped objects are the same within a request, but different across different requests.
We are using Transient because we want everybody to have their own conncection
to the database.
*/

// Add services to the container.
builder.Services.AddRazorPages();
//ConfigurationManager configuration = builder.Configuration; // allows both to access and to set up the config
//IWebHostEnvironment environment = builder.Environment;
string dbChoice = builder.Configuration.GetValue<string>( "DatabaseChoice" ).ToLower();
if ( dbChoice == "sql" )
{
   builder.Services.AddTransient<IDatabaseData, SqlData>();
}
else if ( dbChoice == "sqlite" )
{
   builder.Services.AddTransient<IDatabaseData, SqliteData>();
}
else
{
   // Fallback // Default value
   builder.Services.AddTransient<IDatabaseData, SqlData>();
}

builder.Services.AddTransient<ISqlDataAccess, SqlDataAccess>();
builder.Services.AddTransient<ISqliteDataAccess, SqliteDataAccess>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if ( !app.Environment.IsDevelopment() )
{
   app.UseExceptionHandler( "/Error" );
   // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
   app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
