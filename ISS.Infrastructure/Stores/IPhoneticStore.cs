using System.Collections.Generic;
using ISS.Infrastructure.Models;

namespace ISS.Infrastructure.Stores
{
  public interface IPhoneticStore
  {
    IReadOnlyDictionary<string, Phonetic> PhoneticPostures { get; }
  }
}