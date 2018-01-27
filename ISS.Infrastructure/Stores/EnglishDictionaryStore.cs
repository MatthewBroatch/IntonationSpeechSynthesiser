using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using ISS.Infrastructure.Models;
using MoreLinq;

namespace ISS.Infrastructure.Stores
{
  public class EnglishDictionaryStore : IDictionaryStore
  {
    public IReadOnlyDictionary<string, string> Dictionary { get; private set; }
    
    public EnglishDictionaryStore(string dictionaryFile)
    {
      var fileText = File.ReadLines(dictionaryFile);
      Dictionary = fileText
        .Skip(1)
        .Select(x => x.Split(' '))
        .DistinctBy(x => x[0])
        .ToDictionary(x => x[0], x => x[1].Split('%')[0]);
    }
  }
}