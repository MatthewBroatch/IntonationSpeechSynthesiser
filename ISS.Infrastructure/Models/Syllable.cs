using System.Collections.Generic;

namespace ISS.Infrastructure.Models
{
    public class Syllable
    {
      public IEnumerable<Phonetic> Postures { get; set; }
      public SyllableEnd SyllableEnd { get; set; }
    }
}