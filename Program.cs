using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using nocscienceat.MetaDirectory;
using nocscienceat.MetaDirectory.Services.AdService;
using nocscienceat.MetaDirectory.Services.IdmUserService;
using nocscienceat.MetaDirectory.Services.UserSyncService;
using Serilog;
using System;
using System.Threading;
using nocscienceat.MetaDirectory.Services.ComputerSyncService;
using nocscienceat.MetaDirectory.Services.IdmDeviceService;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    IHost host = Host.CreateDefaultBuilder(args)

        .ConfigureAppConfiguration((_, config) =>
        {
            config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("secrets.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args);
        })
        .ConfigureServices((context, services) =>
        {
            IConfiguration config = context.Configuration;
            services.AddSingleton(config);

            services.AddSingleton<IUserSyncService, UserSyncService>();
            services.AddSingleton<IIdmUserService, IdmUserService>();
            services.AddSingleton<IAdService, AdService>();
            services.AddSingleton<IComputerSyncService, ComputerSyncService>();
            services.AddSingleton<IIdmDeviceService, IdmDeviceService>();

            services.AddSingleton<Dispatcher>();
        })
        .UseSerilog((context, services, loggerConfiguration) =>
        {
            loggerConfiguration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext();
        })
        .Build();

    using (host)
    {
        Log.Information("Application Starting {dt}", DateTime.UtcNow);
        Dispatcher dispatcher = host.Services.GetRequiredService<Dispatcher>();
        await dispatcher.ExecuteAsync(CancellationToken.None);
        Log.Information("Application Ending {dt}", DateTime.UtcNow);
    }

}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
