using DROWeb.Persistence;
using FastEndpoints;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.UseStartup<Startup>();
    });


var host = builder.Build();


using (var scope = host.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    try
    {
        var context = serviceProvider.GetRequiredService<PlayersDbContext>();
        DbInitializer.Initialize(context);
    }
    catch (Exception ex)
    {

    }
}

host.Run();