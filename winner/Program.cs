using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.IO;
using winner.Interfaces;
using winner.Services;

namespace winner
{
  class Program
  {
    static void Main(string[] args)
    {

      //Setting up the appsettings file
     var builder = new ConfigurationBuilder();
      BuildConfig(builder);


      //Configure logger
      Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Build())
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .CreateLogger();

      Log.Logger.Information("Starting application");
      
      var host = Host.CreateDefaultBuilder()
        .ConfigureServices((context, services) =>
        {
          Log.Logger.Information("Setting up Dependency Injection");
          services.AddTransient<IGameRunnerService, GameRunnerService>();
          services.AddTransient<IGameConfigs, GameConfigs>();
          services.AddTransient<IPlayerService, PlayerService>();
          services.AddTransient<IGameService, GameService>();
        })
        .UseSerilog()
        .Build();

      Log.Logger.Information($"Instantiating {nameof(GameRunnerService)}");
      var svc = ActivatorUtilities.CreateInstance<GameRunnerService>(host.Services);

      Log.Logger.Information($"Starting up {nameof(GameRunnerService)} - IOC");
      svc.Run(args);
    }

    static void BuildConfig(IConfigurationBuilder builder)
    {
      builder.SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true, reloadOnChange: true)
        .AddEnvironmentVariables();
    }
  }
}
