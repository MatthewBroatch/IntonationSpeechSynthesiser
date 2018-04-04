using System;
using System.Collections.Generic;

namespace ISS.Infrastructure.Models
{
  public class WaveTable
  {
    private const int TABLE_LENGTH = 512;
    public double[] Wavetable { get; set; }
    public int CurrentPosition { get; set; }
    private int _risePortion { get; set; }
    private int _fallPortion { get; set; }
    private double tnLength { get; set; }
    private double tnDelta { get; set; }
    private double _basicIncrement { get; set; }

    public WaveTable(double sampleRate, ThroatModelParameters parameters)
    {
      Wavetable = new double[TABLE_LENGTH];

      /*  CALCULATE WAVE TABLE PARAMETERS  */
      _risePortion = (int)Math.Round(TABLE_LENGTH * (parameters.GlotPulseRiseTime / parameters.SectionLength));
      _fallPortion = (int)Math.Round(TABLE_LENGTH * ((parameters.GlotPulseRiseTime + parameters.GlotPulseFallTimeMax) / parameters.SectionLength));
      tnLength = _fallPortion - _risePortion;
      tnDelta = Math.Round(TABLE_LENGTH * ((parameters.GlotPulseFallTimeMax - parameters.GlotPulseFallTimeMin) / parameters.SectionLength));
      _basicIncrement = (double)TABLE_LENGTH / sampleRate;
      CurrentPosition = 0;

      int i, j;
      if (parameters.GlotSrcType == GlotSrcType.Pulse) {
        /*  CALCULATE RISE PORTION OF WAVE TABLE  */
        for (i = 0; i < _risePortion; i++) {
          double x = (double)i / (double)_risePortion;
          double x2 = x * x;
          double x3 = x2 * x;
          Wavetable[i] = (3.0 * x2) - (2.0 * x3);
        }

        /*  CALCULATE FALL PORTION OF WAVE TABLE  */
        for (i = _risePortion, j = 0; i < _fallPortion; i++, j++) {
          double x = (double)j / tnLength;
          Wavetable[i] = 1.0 - (x * x);
        }

        /*  SET CLOSED PORTION OF WAVE TABLE  */
        for (i = _fallPortion; i < TABLE_LENGTH; i++)
          Wavetable[i] = 0.0;
      }
      else
      {
        /*  SINE WAVE  */
        for (i = 0; i < TABLE_LENGTH; i++) {
            Wavetable[i] = Math.Sin( ((double)i/(double)TABLE_LENGTH) * 2.0 * Math.PI );
        }
      }	
    }

    public void UpdateWavetable(double amplitude)
    {
      /*  CALCULATE NEW CLOSURE POINT, BASED ON AMPLITUDE  */
      double newFallPortion = _fallPortion - Math.Round(amplitude * tnDelta);
      double newTnLength = newFallPortion - _risePortion;

      /*  RECALCULATE THE FALLING PORTION OF THE GLOTTAL PULSE  */
      for (int i = _risePortion, j = 0; i < newFallPortion; i++, j++) {
        double x = (double)j / newTnLength;
        Wavetable[i] = 1.0 - (x * x);
      }

        /*  FILL IN WITH CLOSED PORTION OF GLOTTAL PULSE  */
      for (int i = (int)newFallPortion; i < _fallPortion; i++)
        Wavetable[i] = 0.0;
    }

    public double Oscillator(double frequency)
    {
      IncrementTablePosition(frequency);

      /*  FIND SURROUNDING INTEGER TABLE POSITIONS  */
      var lowerPosition = CurrentPosition;
      var upperPosition = (lowerPosition + 1) % TABLE_LENGTH;

      /*  RETURN INTERPOLATED TABLE VALUE  */
      return (Wavetable[lowerPosition] +
        ((CurrentPosition - lowerPosition) *
        (Wavetable[upperPosition] - Wavetable[lowerPosition])));
    }

    private void IncrementTablePosition(double frequency)
    {
      CurrentPosition = (CurrentPosition + (int)Math.Floor(frequency * _basicIncrement)) % TABLE_LENGTH;
    }
  }
}