using System;
using System.Collections.Generic;
using System.Linq;
using ISS.Infrastructure.Models;

namespace ISS.Infrastructure.Services
{
  public class IntonationModelService : IIntonationModelService
  {
    public IEnumerable<IntonationModel> CreateModel(IEnumerable<Syllable> syallables)
    {
      return syallables.SelectMany(syllable => {
        var duration = syllable.Postures.Sum(x => x.Duration);

        return Enumerable.Range(0, Convert.ToInt32(duration))
          .Select(x => new IntonationModel()
          {
            Aspiration = 0,
            Frication = 0,
            FricationBW = 0,
            FricationCF = 0,
            FricationPosition = 0,
            Pitch = 0,
            RadiusSegments = syllable.Postures.First().RadiusSegments,
            Voicing = 1,
            Velum = syllable.Postures.First().Velum,
          });
      });
    }
  }
}