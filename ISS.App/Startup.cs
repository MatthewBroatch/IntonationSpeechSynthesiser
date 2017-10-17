using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ISS.App
{
  public class Startup
  {
    IConfigurationRoot Configuration { get; }

    public Startup()
    {
      var builder = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json");

      Configuration = builder.Build();
    }

    public void ConfigureServices(IServiceCollection services)
    {
      services.AddLogging();
      services.AddSingleton<IConfigurationRoot>(Configuration);
      // services.AddSingleton<IMyService, MyService>();
    }
  }
}