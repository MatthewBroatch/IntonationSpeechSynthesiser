using System.Collections.Generic;

namespace ISS.Infrastructure.Models
{
  internal class Vowel
  {
    string IpaName { get; set; }
    string GnuName { get; set; }
    double Distance { get; set; }
    double Height { get; set; }
    VowelRounded Rounded { get; set; } 
    float Range { get; set; }
    IEnumerable<float> RadiusSegments { get; set; }
  }
}