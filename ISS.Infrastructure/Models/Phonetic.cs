using System.Collections.Generic;

namespace ISS.Infrastructure.Models
{
  public class Phonetic
  {
    public string Name { get; set; }
    public double MicroInt { get; set; }
    public double GlotVol { get; set; }
    public double AspVol { get; set; }
    public double FricVol { get; set; }
    public double FricPos { get; set; }
    public double FricCF { get; set; }
    public double FricBW { get; set; }

    //Throat and mouth raduis
    public IEnumerable<double> RadiusSegments { get; set; }

    //The bit connecting your nose and mouth
    public double Velum { get; set; }
    public double Duration { get; set; }
    public double Transition { get; set; }

    //Todo Figure out what this is
    public double Qssa { get; set; }
    public double Qssb { get; set; }

    // public Vowel Vowel { get; set; }
    // public PhoneticEnd End { get; set; }
    public IEnumerable<Category> Categories { get; set; }
  }
}