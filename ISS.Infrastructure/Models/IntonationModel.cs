using System.Collections.Generic;

namespace ISS.Infrastructure.Models
{
  public class IntonationModel
  {
    public double Pitch { get; set; }
    public double GlotVolume { get; set; }
    public double AspirationVolume { get; set; }
    public double FricationVolume { get; set; }
    public double FricationPosition { get; set; }
    public double FricationCF { get; set; }
    public double FricationBW { get; set; }
    public IEnumerable<double> RadiusSegments { get; set; }
    public double Velum { get; set; }
  }
}