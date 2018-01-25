using System.Collections.Generic;

namespace ISS.Infrastructure.Models
{
  public class IntonationModel
  {
    public double Pitch { get; set; }
    public double Voicing { get; set; }
    public double Aspiration { get; set; }
    public double Frication { get; set; }
    public double FricationPosition { get; set; }
    public double FricationCF { get; set; }
    public double FricationBW { get; set; }
    public IEnumerable<double> RadiusSegments { get; set; }
    public double Velum { get; set; }
  }
}