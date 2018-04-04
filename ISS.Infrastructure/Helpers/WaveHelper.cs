using System;
using System.Collections.Generic;
using System.Linq;
using ISS.Infrastructure.Models;

namespace ISS.Infrastructure.Helpers
{
  public static class WaveHelper
  {
    public static double SpeedOfSound(double temperature) => 331.4 + (0.6 * temperature);
    public static double CaclulateFrequency(double pitch) => PITCH_BASE * Math.Pow(2.0, (((double)(pitch + PITCH_OFFSET)) / 12.0));
    public static double Noise() => _random.NextDouble();
    public static double CalculateAmplitude(double decibelLevel)
    {
      /*  CONVERT 0-60 RANGE TO -60-0 RANGE  */
      decibelLevel -= VOL_MAX;
      /*  IF -60 OR LESS, RETURN AMPLITUDE OF 0  */
      if (decibelLevel <= (-VOL_MAX))
          return(0.0);
      /*  IF 0 OR GREATER, RETURN AMPLITUDE OF 1  */
      if (decibelLevel >= 0.0)
          return(1.0);
      /*  ELSE RETURN INVERSE LOG VALUE  */
      return(Math.Pow(10.0,(decibelLevel / 20.0)));
    }

    //SCALING CONSTANT FOR INPUT TO VOCAL TRACT & THROAT
    private const int VOL_MAX = 60;
    private const double PITCH_BASE = 220.0;
    private const int PITCH_OFFSET = 3;
    private static Random _random = new Random();
  }
}