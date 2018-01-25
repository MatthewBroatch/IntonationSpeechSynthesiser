using System.Collections.Generic;
using ISS.Infrastructure.Models;

namespace ISS.Infrastructure.Stores
{
  public interface IPhoneticStore
  {
    IDictionary<string, Phonetic> PhoneticPostures { get; }
  }
}