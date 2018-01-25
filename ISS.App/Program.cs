using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ISS.App
{
  class Program
  {
    static void Main(string[] args)
    {
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
      if(args.Length == 0)
        listener.StartListening();
      else
        listener.ConvertInput(args[0]);
    }
  }
}