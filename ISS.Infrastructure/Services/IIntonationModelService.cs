using System.Collections.Generic;
using ISS.Infrastructure.Models;

namespace ISS.Infrastructure.Services
{
  public interface IIntonationModelService
  {
    IEnumerable<IntonationModel> CreateModel(IEnumerable<Syllable> syallables);
  }
}