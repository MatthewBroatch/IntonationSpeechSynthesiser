using System.Collections.Generic;

namespace ISS.Infrastructure.Models
{
  public class Syllable
  {
    float TotalTime { get; set; }
    float OnsetTime { get; set; }
    float CodaTime { get; set; }
    bool BreatheAfter { get; set; }
    bool NewWord { get; set; }
    IEnumerable<Phonetic> Onset { get; set; }
    Phonetic Nucleus { get; set; }
    IEnumerable<Phonetic> Coda { get; set; }
  }
}