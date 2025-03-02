using System.Reflection;
using System.Text.Json.Serialization;

using BlocklistAPI.Classes;
using BlocklistAPI.Context;

using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder( args );

//IActivator activator = new Microsoft.AspNetCore.DataProtection.KeyManagement.XmlKeyManager().
// Correct the instantiation of XmlKeyManager
//XmlKeyManager keyManager = new( );
//    Options.Create( new KeyManagementOptions( ) { AutoGenerateKeys = true } ), )
//);
// Web Deploy FTP: 	site17708.siteasp.net, site17708 Bk8#i5+X=eY6
// Web Deploy: 	site17708.siteasp.net, port 8172, site17708 nZ#4A5p-9!Ef

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddMvc( );
builder.Services.AddControllers( )
                .AddJsonOptions( opt =>
                {
                    opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                    //opt.JsonSerializerOptions.Converters.Add( new JsonStringEnumConverter( ) );
                    //opt.JsonSerializerOptions.TypeInfoResolver = new DefaultJsonTypeInfoResolver( );
                    opt.JsonSerializerOptions.IncludeFields = false;
                    opt.JsonSerializerOptions.UnmappedMemberHandling = JsonUnmappedMemberHandling.Skip;
                    opt.JsonSerializerOptions.IgnoreReadOnlyProperties = true;
                    //opt.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    opt.JsonSerializerOptions.PropertyNameCaseInsensitive = false;
                    opt.JsonSerializerOptions.PropertyNamingPolicy = null; // This is important for ability to deserialise to data models
                } );

string connectionString = OpenCartDbContext.GetConnectionString( );

GetUrls( out string httpUrl, out string httpsUrl );
//httpsUrl = conf.GetAppSetting( "HttpsUrl" );

// OpenCartDbContext dbContext = new( );

using MySqlConnector.MySqlConnection connection = new( connectionString );
builder.Services
       .AddDbContext<OpenCartDbContext>( options => options.UseMySQL( connectionString ) ); //, ServerVersion.AutoDetect( connection ) ) );

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
var xmlFilename = Path.Combine( AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly( ).GetName( ).Name}.xml" );
builder.Services
       .AddEndpointsApiExplorer( );
builder.Services
       .AddSwaggerGen( c =>
       {
           c.SwaggerDoc(
               "v1",
               new OpenApiInfo
               {
                   Title = "Blocklist Api",
                   Version = "v1.00",
                   Description = "Blocklist Api",
               }
           );

           c.SchemaFilter<EnumSchemaFilter>( );
           c.IncludeXmlComments( xmlFilename );
           c.UseInlineDefinitionsForEnums( );
           c.DescribeAllParametersInCamelCase( );
       } );

var app = builder.Build( );

// Test Db details
try
{
    connection.Open( );
    if ( connection.State == System.Data.ConnectionState.Open )
    {
        Console.WriteLine( "Connection Successful" );
        connection.Close( );
    }
    else
    {
        Console.WriteLine( "Connection Failed" );
        return;
    }

    //using OpenCartDbContext dbContext = new( );
    //    dbContext.Database.EnsureCreated( );
    //if ( dbContext.Database.ExecuteSqlRaw( "SELECT 1" ) == 1 )
    //{
    //    Console.WriteLine( "Database Connection Successful" );
    //}
    //else
    //{
    //    Console.WriteLine( "Database Connection Failed" );
    //    return;
    //}
    using ( OpenCartDbContext dbContext = new( ) )
    {
        try { dbContext.Database.Migrate( ); } catch { }
        //BlocklistAPI.Migrations.Init init = new BlocklistAPI.Migrations.Init( );
        //init.Up( new MigrationBuilder( dbContext.Database.ProviderName ) );

        //        dbContext.ConfirmStartupDataExists( );
        dbContext.Database.CloseConnection( );
    }
}
catch ( Exception ex )
{
    Console.WriteLine( ex.Message );
}

// Configure the HTTP request pipeline.
//if ( app.Environment.IsDevelopment( ) )
//{
app.UseSwagger( );
app.UseSwaggerUI
    ( opt =>
        opt.EnableTryItOutByDefault( )
    );
//}

app.UseHttpsRedirection( );
app.UseAuthorization( );
app.MapControllers( );


//var summaries = new[]
//{
//    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
//};

//app.MapGet( "/weatherforecast", ( ) =>
//{
//    var forecast = Enumerable.Range( 1, 5 ).Select( index =>
//        new WeatherForecast
//        (
//            DateOnly.FromDateTime( DateTime.Now.AddDays( index ) ),
//            Random.Shared.Next( -20, 55 ),
//            summaries[ Random.Shared.Next( summaries.Length ) ]
//        ) )
//        .ToArray( );
//    return forecast;
//} )
//.WithName( "GetWeatherForecast" )
//.WithOpenApi( );

app.Run( );

static void GetUrls( out string httpUrl, out string httpsUrl )
{
    httpUrl = "http://localhost:5000";
    httpsUrl = "https://localhost:5001";
    string configFilePath = Assembly.GetExecutingAssembly( ).Location;

    if ( !string.IsNullOrEmpty( configFilePath ) )
    {
        if ( configFilePath.Contains( '\\' ) )
            configFilePath = $"{configFilePath[ 0..configFilePath.LastIndexOf( '\\' ) ]}\\appsettings.json";
        else if ( configFilePath.Contains( '/' ) )
            configFilePath = $"{configFilePath[ 0..configFilePath.LastIndexOf( '/' ) ]}/appsettings.json";
    }
    else
        configFilePath = "appsettings.json";

    IConfigurationBuilder configuration = new ConfigurationBuilder( ).AddJsonFile( configFilePath );
    IConfigurationRoot config = configuration.Build( );

    try
    {
        httpUrl = config.GetSection( "Kestrel" ).GetSection( "Endpoints" ).GetSection( "Http" ).GetSection( "Url" ).Value;
    }
    catch { }
    finally
    {
        httpsUrl = config.GetSection( "Kestrel" ).GetSection( "Endpoints" ).GetSection( "Https" ).GetSection( "Url" ).Value;
    }
}

//internal record WeatherForecast( DateOnly Date, int TemperatureC, string? Summary )
//{
//    public int TemperatureF => 32 + (int)( this.TemperatureC / 0.5556 );
//}
