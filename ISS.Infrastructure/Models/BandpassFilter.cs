using System;
using System.Collections.Generic;

namespace ISS.Infrastructure.Models
{
  public class BandpassFilter
  {
    public void Update(double fricBW, double fricCF, double sampleRate)
    {
      var tanValue = Math.Tan((Math.PI * fricBW) / sampleRate);
      var cosValue = Math.Cos((2.0 * Math.PI * fricCF) / sampleRate);

      Beta = (1.0 - tanValue) / (2.0 * (1.0 + tanValue));
      Gamma = (0.5 + Beta) * cosValue;
      Alpha = (0.5 - Beta) / 2.0;
    }

    public double Filter(double input) {
      double output = 2.0 * ((Alpha * (input - xn2)) + (Gamma * yn1) - (Beta * yn2));

      xn2 = xn1;
      xn1 = input;
      yn2 = yn1;
      yn1 = output;

      return (output);
    }
    
    private double Alpha { get; set; }
    private double Beta { get; set; }
    private double Gamma { get; set; }
    private double xn1 = 0.0, xn2 = 0.0, yn1 = 0.0, yn2 = 0.0;
  }
}