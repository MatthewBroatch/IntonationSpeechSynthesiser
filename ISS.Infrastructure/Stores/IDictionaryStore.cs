using System.Collections.Generic;
using ISS.Infrastructure.Models;

namespace ISS.Infrastructure.Stores
{
  public interface IDictionaryStore
  {
    IReadOnlyDictionary<string, string> Dictionary { get; }
  }
}