using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using nocscienceat.MetaDirectory;
using nocscienceat.MetaDirectory.Logging;
using nocscienceat.MetaDirectory.Services.AdService;
using nocscienceat.MetaDirectory.Services.ComputerSyncService;
using nocscienceat.MetaDirectory.Services.IdmDeviceService;
using nocscienceat.MetaDirectory.Services.IdmUserService;
using nocscienceat.MetaDirectory.Services.UserSyncService;
using Serilog;
using System;
using System.Text;
using System.Threading;

IConfiguration? config = null;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger(); // Bootstrap logger for early-start logging before full configuration

StringBuilder logInfo = new(); // In-memory log buffer used for emailing execution logs

try
{
    IHost host = Host.CreateDefaultBuilder(args)

        .ConfigureAppConfiguration((_, configuration) =>
        {
            configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("secrets.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args); // Aggregate configuration from multiple sources
        })
        .ConfigureServices((context, services) =>
        {
            config = context.Configuration;
            services.AddSingleton(config); // Expose configuration via DI

            // Register synchronization and directory services
            services.AddSingleton<IUserSyncService, UserSyncService>();
            services.AddSingleton<IIdmUserService, IdmUserService>();
            services.AddSingleton<IAdService, AdService>();
            services.AddSingleton<IComputerSyncService, ComputerSyncService>();
            services.AddSingleton<IIdmDeviceService, IdmDeviceService>();

            services.AddSingleton<Dispatcher>(); // Central dispatcher orchestrating the sync workflow
        })
        .UseSerilog((context, services, loggerConfiguration) =>
        {
            loggerConfiguration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .WriteTo.Sink(new StringBuilderSink(logInfo)); // Mirror logs into StringBuilder for email reporting
        })
        .Build();

    using (host)
    {
        Log.Information("Application Starting {dt}", DateTime.UtcNow);
        Dispatcher dispatcher = host.Services.GetRequiredService<Dispatcher>();
        await dispatcher.ExecuteAsync(CancellationToken.None); // Run main synchronization pipeline
        Log.Information("Application Ending {dt}", DateTime.UtcNow);
    }

}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly"); // Critical failure logging
}
finally
{
    Log.CloseAndFlush();

    await MailLog.Send(config, logInfo); // Email accumulated log information after shutdown
}

