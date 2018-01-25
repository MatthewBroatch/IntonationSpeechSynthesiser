using System.Collections.Generic;
using ISS.Infrastructure.Models;

namespace ISS.Infrastructure.Services
{
  public interface IPhoneticParserService
  {
    IEnumerable<Syllable> Parse(string input);
  }
}