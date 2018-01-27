using System.Collections.Generic;
using System.Linq;
using ISS.Infrastructure.Extenstions;
using ISS.Infrastructure.Models;
using ISS.Infrastructure.Stores;

namespace ISS.Infrastructure.Services
{
  public class TextToPhoneticsService : ITextToPhoneticsService
  {
    private readonly IDictionaryStore _dictionaryStore;

    public TextToPhoneticsService(IDictionaryStore dictionaryStore)
    {
      _dictionaryStore = dictionaryStore;
    }

    public string Parse(string input)
    {
      //handle this way better some other way
      return input
        .Replace(".", " ^")
        .Split(' ')
        .Select(x => _dictionaryStore.Dictionary[x])
        .Aggregate((a, b) => a + " " + b);
    }
  }
}