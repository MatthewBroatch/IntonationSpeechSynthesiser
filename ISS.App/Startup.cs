using ISS.Infrastructure.Models;
using ISS.Infrastructure.Services;
using ISS.Infrastructure.Stores;
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

      services.AddTransient<ConsoleListener>();
      services.AddTransient<IPhoneticParserService, PhoneticParserService>();
      services.AddTransient<ITextToPhoneticsService, TextToPhoneticsService>();
      services.AddTransient<IIntonationModelService, IntonationModelService>();
      services.AddTransient<ModelToWaveService>();
      services.AddSingleton<ThroatModelParameters>(provider => ThroatModelParameters.CreateDefault());
      services.AddSingleton<IPhoneticStore>(provider => new MonetPhoneticStore(Configuration["phoneList"]));
      services.AddSingleton<IDictionaryStore>(provider => new EnglishDictionaryStore(Configuration["syllableDictionary"]));
    }
  }
}