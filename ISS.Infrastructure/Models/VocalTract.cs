using System;
using System.Collections.Generic;
using System.Linq;
using ISS.Infrastructure.Helpers;

namespace ISS.Infrastructure.Models
{
  public class VocalTract
  {
    public VocalTract(ThroatModelParameters parameters, double nyquist, double sampleRate) {
      Oropharynx = Enumerable.Range(0,10).Select(x => new TubeSection()).ToArray();
      Nasal = Enumerable.Range(0,6).Select(x => new TubeSection()).ToArray();
      OropharynxCoefficients = new double[8];
      NasalCoefficients = new double[6];
      FricationTap = new double[8];
      Alpha = new Junction();
      InitializeMouthCoefficients((nyquist - parameters.MouthAperatureCoefficient) / nyquist);
      InitializeNasalFilterCoefficients((nyquist - parameters.NoseAperatureCoefficient) / nyquist);
      InitializeNasalCavity(parameters);
      InitializeThroat(parameters, sampleRate);
    }

    public TubeSection[] Oropharynx { get; set; }
    public double[] OropharynxCoefficients { get; set; }
    public TubeSection[] Nasal { get; set; }
    public double[] NasalCoefficients { get; set; }
    public double[] FricationTap { get; set; }
    public Junction Alpha { get; set; }
    private int currentPointer = 1;
    private int previousPointer = 0;

    public void CalculateTubeCoefficients(IntonationModel model, ThroatModelParameters parameters)
    {
      /*  CALCULATE COEFFICIENTS FOR THE Oropharynx  */
      for (int i = 0; i < OropharynxCoefficients.Count(); i++) {
        var lastSegment = i + 1 == OropharynxCoefficients.Count();
        var radA2 = Square(model.RadiusSegments.ElementAt(i));
        var radB2 = lastSegment ? Square(parameters.AperatureScalingRadius) : Square(model.RadiusSegments.ElementAt(i + 1));
        OropharynxCoefficients[i] = (radA2 - radB2) / (radA2 + radB2);
      }	

      /*  CALCULATE ALPHA COEFFICIENTS FOR 3-WAY JUNCTION  */
      var sides = Square(model.RadiusSegments.ElementAt(3));
      var velum = Square(model.Velum);
      var sum = 2.0 / (sides + sides + velum);
      Alpha.Left = sum * sides;
      Alpha.Right = sum * sides;
      Alpha.Upper = sum * velum;

      /*  AND 1ST NASAL PASSAGE COEFFICIENT  */
      var nose = Square(parameters.NoseRadius.ElementAt(1));
      NasalCoefficients[0] = (velum - nose) / (velum + nose);
    }

    public void SetFricationTaps(IntonationModel model)
    {
      double fricationAmplitude = WaveHelper.CalculateAmplitude(model.FricationVolume);


      /*  CALCULATE POSITION REMAINDER AND COMPLEMENT  */
      var integerPart = (int)model.FricationPosition;
      var complement = model.FricationPosition - (double)integerPart;
      var remainder = 1.0 - complement;

      /*  SET THE FRICATION TAPS  */
      for (int i = 0; i < 8; i++) {
        if (i == integerPart) {
          FricationTap[i] = remainder * fricationAmplitude;
        if ((i+1) < 8)
          FricationTap[++i] = complement * fricationAmplitude;
      }
      else
        FricationTap[i] = 0.0;
      }
    }

    private double throatY = 0.0;
    public double CalculateThroatSignal(double input)
    {
      double output = (ta0 * input) + (tb1 * throatY);
      throatY = output;
      return (output * throatGain);
    }

    public double CalculateVocalTractSignal(double input, double frication, double dampingFactor)
    {
      /*  INCREMENT CURRENT AND PREVIOUS POINTERS  */
      if (++currentPointer > 1)
        currentPointer = 0;
      if (++previousPointer > 1)
        previousPointer = 0;

      /*  UPDATE Oropharynx  */
      /*  INPUT TO TOP OF TUBE  */
      Oropharynx[0].Top[currentPointer] = (Oropharynx[0].Bottom[previousPointer] * dampingFactor) + input;

      /*  CALCULATE THE SCATTERING JUNCTIONS FOR S1-S2  */
      var delta = OropharynxCoefficients[0] * (Oropharynx[0].Top[previousPointer] - Oropharynx[1].Bottom[previousPointer]);
      Oropharynx[1].Top[currentPointer] = (Oropharynx[0].Top[previousPointer] + delta) * dampingFactor;
      Oropharynx[0].Bottom[currentPointer] = (Oropharynx[1].Bottom[previousPointer] + delta) * dampingFactor;

      /*  CALCULATE THE SCATTERING JUNCTIONS FOR S2-S3 AND S3-S4  */
      for (int i = 1, j = 0; i < 3; i++, j++)
      {
        delta = OropharynxCoefficients[i] * (Oropharynx[i].Top[previousPointer] - Oropharynx[i+1].Bottom[previousPointer]);
        Oropharynx[i+1].Top[currentPointer] = ((Oropharynx[i].Top[previousPointer] + delta) * dampingFactor) + (FricationTap[j] * frication);
        Oropharynx[i].Bottom[currentPointer] = (Oropharynx[i+1].Bottom[previousPointer] + delta) * dampingFactor;
      }

      /*  UPDATE 3-WAY JUNCTION BETWEEN THE MIDDLE OF R4 AND NASAL CAVITY  */
      var junctionPressure = (Alpha.Left * Oropharynx[3].Top[previousPointer]) + (Alpha.Right * Oropharynx[4].Bottom[previousPointer]) + (Alpha.Upper * Nasal[0].Bottom[previousPointer]);
      Oropharynx[3].Bottom[currentPointer] = (junctionPressure - Oropharynx[3].Top[previousPointer]) * dampingFactor;
      Oropharynx[4].Top[currentPointer] = ((junctionPressure - Oropharynx[4].Bottom[previousPointer]) * dampingFactor) + (FricationTap[2] * frication);
      Nasal[0].Top[currentPointer] = (junctionPressure - Nasal[0].Bottom[previousPointer]) * dampingFactor;

        /*  CALCULATE JUNCTION BETWEEN R4 AND R5 (S5-S6)  */
      delta = OropharynxCoefficients[3] * (Oropharynx[4].Top[previousPointer] - Oropharynx[5].Bottom[previousPointer]);
      Oropharynx[5].Top[currentPointer] = ((Oropharynx[4].Top[previousPointer] + delta) * dampingFactor) + (FricationTap[3] * frication);
      Oropharynx[4].Bottom[currentPointer] = (Oropharynx[5].Bottom[previousPointer] + delta) * dampingFactor;

      /*  CALCULATE JUNCTION INSIDE R5 (S6-S7) (PURE DELAY WITH DAMPING)  */
      Oropharynx[6].Top[currentPointer] = (Oropharynx[5].Top[previousPointer] * dampingFactor) + (FricationTap[4] * frication);
      Oropharynx[5].Bottom[currentPointer] = Oropharynx[6].Bottom[previousPointer] * dampingFactor;

      /*  CALCULATE LAST 3 INTERNAL JUNCTIONS (S7-S8, S8-S9, S9-S10)  */
      for (int i = 6, j = 4, k = 5; i < 9; i++, j++, k++)
      {
        delta = OropharynxCoefficients[j] * (Oropharynx[i].Top[previousPointer] - Oropharynx[i+1].Bottom[previousPointer]);
        Oropharynx[i+1].Top[currentPointer] = ((Oropharynx[i].Top[previousPointer] + delta) * dampingFactor) + (FricationTap[k] * frication);
        Oropharynx[i].Bottom[currentPointer] = (Oropharynx[i+1].Bottom[previousPointer] + delta) * dampingFactor;
      }

      /*  REFLECTED SIGNAL AT MOUTH GOES THROUGH A LOWPASS FILTER  */
      Oropharynx[9].Bottom[currentPointer] =  dampingFactor * ReflectionFilter(OropharynxCoefficients[7] * Oropharynx[9].Top[previousPointer]);

      /*  OUTPUT FROM MOUTH GOES THROUGH A HIGHPASS FILTER  */
      var output = RadiationFilter((1.0 + OropharynxCoefficients[7]) * Oropharynx[9].Top[previousPointer]);

      /*  UPDATE NASAL CAVITY  */
      for (int i = 0, j = 0; i < 5; i++, j++)
      {
        delta = NasalCoefficients[j] * (Nasal[i].Top[previousPointer] - Nasal[i+1].Bottom[previousPointer]);
        Nasal[i+1].Top[currentPointer] = (Nasal[i].Top[previousPointer] + delta) * dampingFactor;
        Nasal[i].Bottom[currentPointer] = (Nasal[i+1].Bottom[previousPointer] + delta) * dampingFactor;
      }

      /*  REFLECTED SIGNAL AT NOSE GOES THROUGH A LOWPASS FILTER  */
      Nasal[5].Bottom[currentPointer] = dampingFactor * NasalReflectionFilter(NasalCoefficients[5] * Nasal[5].Top[previousPointer]);

      /*  OUTPUT FROM NOSE GOES THROUGH A HIGHPASS FILTER  */
      output += NasalRadiationFilter((1.0 + NasalCoefficients[5]) * Nasal[5].Top[previousPointer]);

      /*  RETURN SUMMED OUTPUT FROM MOUTH AND NOSE  */
      return(output);
    }

    private double b11 = 0.0, a10 = 0.0, a20 = 0.0, a21 = 0.0, b21 = 0.0;
    public void InitializeMouthCoefficients(double coeff)
    {
      b11 = -coeff;
      a10 = 1.0 - Math.Abs(b11);

      a20 = coeff;
      a21 = b21 = -a20;
    }

    private double reflectionY = 0.0;
    private double ReflectionFilter(double input)
    {
      double output = (a10 * input) - (b11 * reflectionY);
      reflectionY = output;
      return (output);
    }

    private double radiationX = 0.0, radiationY = 0.0;
    private double RadiationFilter(double input)
    {
      double output = (a20 * input) + (a21 * radiationX) - (b21 * radiationY);
      radiationX = input;
      radiationY = output;
      return (output);
    }

    private double nb11 = 0.0, na10 = 0.0, na20 = 0.0, na21 = 0.0, nb21 = 0.0;
    public void InitializeNasalFilterCoefficients(double coeff)
    {
      nb11 = -coeff;
      na10 = 1.0 - Math.Abs(nb11);

      na20 = coeff;
      na21 = nb21 = -na20;
    }

    private double nasalReflectionY = 0.0;
    private double NasalReflectionFilter(double input)
    {
      double output = (na10 * input) - (nb11 * nasalReflectionY);
      nasalReflectionY = output;
      return (output);
    }

    private double nasalRadiationX = 0.0, nasalRadiationY = 0.0;
    private double NasalRadiationFilter(double input)
    {
      double output = (na20 * input) + (na21 * nasalRadiationX) - (nb21 * nasalRadiationY);
      nasalRadiationX = input;
      nasalRadiationY = output;
      return (output);
    }

    private double ta0 = 0.0, tb1 = 0.0, throatGain = 0.0;
    private void InitializeThroat(ThroatModelParameters parameters, double sampleRate)
    {
      ta0 = (parameters.ThroatFreqCutoff * 2.0) / sampleRate;
      tb1 = 1.0 - ta0;

      throatGain = WaveHelper.CalculateAmplitude(parameters.ThroatVolume);
    }

    private void InitializeNasalCavity(ThroatModelParameters parameters)
    {
      double radA2, radB2;
      /*  CALCULATE COEFFICIENTS FOR INTERNAL FIXED SECTIONS OF NASAL CAVITY  */
      for (int i = 0, j = 1; i < 4; i++, j++) {
        radA2 = Square(parameters.NoseRadius.ElementAt(i));
        radB2 = Square(parameters.NoseRadius.ElementAt(i+1));
        NasalCoefficients[j] = (radA2 - radB2) / (radA2 + radB2);
      }

      /*  CALCULATE THE FIXED COEFFICIENT FOR THE NOSE APERTURE  */
      radA2 = Square(parameters.NoseRadius.ElementAt(4));
      radB2 = parameters.AperatureScalingRadius;
      NasalCoefficients[5] = (radA2 - radB2) / (radA2 + radB2);
    }

    private double Square(double number) => number * number;
  }

  public class TubeSection
  {
    public double[] Top = new double[] { 0, 0 };
    public double[] Bottom = new double[] { 0, 0 };
  }

  public class Junction
  {
    public double Left = 0;
    public double Right = 0;
    public double Upper = 0;
  }
}

// /*  Oropharynx REGIONS  */
// #define R1                        0      /*  S1  */
// #define R2                        1      /*  S2  */
// #define R3                        2      /*  S3  */
// #define R4                        3      /*  S4 & S5  */
// #define R5                        4      /*  S6 & S7  */
// #define R6                        5      /*  S8  */
// #define R7                        6      /*  S9  */
// #define R8                        7      /*  S10  */
// #define TOTAL_REGIONS             8

// /*  Oropharynx SCATTERING JUNCTION COEFFICIENTS (BETWEEN EACH REGION)  */
// #define C1                        R1     /*  R1-R2 (S1-S2)  */
// #define C2                        R2     /*  R2-R3 (S2-S3)  */
// #define C3                        R3     /*  R3-R4 (S3-S4)  */
// #define C4                        R4     /*  R4-R5 (S5-S6)  */
// #define C5                        R5     /*  R5-R6 (S7-S8)  */
// #define C6                        R6     /*  R6-R7 (S8-S9)  */
// #define C7                        R7     /*  R7-R8 (S9-S10)  */
// #define C8                        R8     /*  R8-AIR (S10-AIR)  */
// #define TOTAL_COEFFICIENTS        TOTAL_REGIONS

// /*  Oropharynx SECTIONS  */
// #define S1                        0      /*  R1  */
// #define S2                        1      /*  R2  */
// #define S3                        2      /*  R3  */
// #define S4                        3      /*  R4  */
// #define S5                        4      /*  R4  */
// #define S6                        5      /*  R5  */
// #define S7                        6      /*  R5  */
// #define S8                        7      /*  R6  */
// #define S9                        8      /*  R7  */
// #define S10                       9      /*  R8  */
// #define TOTAL_SECTIONS            10

// /*  NASAL TRACT SECTIONS  */
// #define N1                        0
// #define VELUM                     N1
// #define N2                        1
// #define N3                        2
// #define N4                        3
// #define N5                        4
// #define N6                        5
// #define TOTAL_NASAL_SECTIONS      6

// /*  NASAL TRACT COEFFICIENTS  */
// #define NC1                       N1     /*  N1-N2  */
// #define NC2                       N2     /*  N2-N3  */
// #define NC3                       N3     /*  N3-N4  */
// #define NC4                       N4     /*  N4-N5  */
// #define NC5                       N5     /*  N5-N6  */
// #define NC6                       N6     /*  N6-AIR  */
// #define TOTAL_NASAL_COEFFICIENTS  TOTAL_NASAL_SECTIONS

// /*  THREE-WAY JUNCTION ALPHA COEFFICIENTS  */
// #define LEFT                      0
// #define RIGHT                     1
// #define UPPER                     2
// #define TOTAL_ALPHA_COEFFICIENTS  3

// /*  FRICATION INJECTION COEFFICIENTS  */
// #define FC1                       0      /*  S3  */
// #define FC2                       1      /*  S4  */
// #define FC3                       2      /*  S5  */
// #define FC4                       3      /*  S6  */
// #define FC5                       4      /*  S7  */
// #define FC6                       5      /*  S8  */
// #define FC7                       6      /*  S9  */
// #define FC8                       7      /*  S10  */
// #define TOTAL_FRIC_COEFFICIENTS   8

// double Oropharynx[TOTAL_SECTIONS][2][2];
// double Oropharynx_coeff[TOTAL_COEFFICIENTS];

// double nasal[TOTAL_NASAL_SECTIONS][2][2];
// double nasal_coeff[TOTAL_NASAL_COEFFICIENTS];

// double alpha[TOTAL_ALPHA_COEFFICIENTS];
//double fricationTap[TOTAL_FRIC_COEFFICIENTS];