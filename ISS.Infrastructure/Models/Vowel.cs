using System.Collections.Generic;

namespace ISS.Infrastructure.Models
{
  public class Vowel
  {
    public string IpaName { get; set; }
    public string GnuName { get; set; }
    public double Distance { get; set; }
    public double Height { get; set; }
    public VowelRounded Rounded { get; set; } 
    public double Range { get; set; }
    public IEnumerable<double> RadiusSegments { get; set; }
  }
}