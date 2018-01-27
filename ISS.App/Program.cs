using System;
using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ISS.App
{
  class Program
  {
    static void Main(string[] args)
    {
      CommandLine.Parser.Default.ParseArguments<Options>(args)
        .WithParsed<Options>(options => {
          IServiceCollection services = new ServiceCollection();
          // Startup.cs finally :)
          Startup startup = new Startup();
          startup.ConfigureServices(services);
          IServiceProvider serviceProvider = services.BuildServiceProvider();

          //configure console logging
          serviceProvider
              .GetService<ILoggerFactory>()
              .AddConsole(LogLevel.Debug);

          var logger = serviceProvider.GetService<ILoggerFactory>()
              .CreateLogger<Program>();

          // Get Service and call method
          var listener = serviceProvider.GetService<ConsoleListener>();
          if(string.IsNullOrEmpty(options.Input))
            listener.StartListening(options.PhonesMode);
          else
            listener.ConvertInput(options.Input, options.PhonesMode);
        })
        .WithNotParsed(error => {
          Console.WriteLine(error.ToString());
        });
    }
  }
}