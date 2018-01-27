using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ISS.App.Models;
using ISS.Infrastructure.Models;

namespace ISS.App
{
  public static class OutputFileHelper
  {
    public const string OutputFileName = "intonation-synth-output.txt";
    public static void Print(OutputFileProperties properties, IEnumerable<IntonationModel> model)
    {
      StreamWriter writer = null;
      if (false) //todo write to file
      {
        // if (File.Exists(OutputFileName))
        //   File.Delete(OutputFileName);
        // File.Create(OutputFileName).Close();
        // writer = File.AppendText(OutputFileName);
      }
      else 
      {
        writer = new StreamWriter(Console.OpenStandardOutput());
        writer.AutoFlush = true;
        Console.SetOut(writer);
      }

      writer.WriteLine($"{properties.OutputFileFormat}\t; output file format (0 = AU, 1 = AIFF, 2 = WAVE)");
      writer.WriteLine($"{properties.OutputSampleRate:F1}\t; output sample rate (22050.0, 44100.0)");
      writer.WriteLine($"{properties.InControlRate:F1}\t; input control rate (1 - 1000 Hz)");
      writer.WriteLine($"{properties.MasterVolume:N1}\t; master volume (0 - 60 dB)");
      writer.WriteLine($"{properties.OutChannels}\t; number of sound output channels (1 or 2)");
      writer.WriteLine($"{properties.StereoBalance:N1}\t; stereo balance (-1 to +1)");
      writer.WriteLine($"{properties.GlotSrcType}\t; glottal source waveform type (0 = pulse, 1 = sine)");
      writer.WriteLine($"{properties.GlotPulseRise:N1}\t; glottal pulse rise time (5 - 50 percent of GP period)");
      writer.WriteLine($"{properties.GlotPulseFallMin:N1}\t; glottal pulse fall time minimum (5 - 50 percent of GP period)");
      writer.WriteLine($"{properties.GlotPulseFallMax:N1}\t; glottal pulse fall time maximum (5 - 50 percent of GP period)");
      writer.WriteLine($"{properties.GlotSrcBreath:N1}\t; glottal source breathiness (0 - 10 percent of GS amplitude)");
      writer.WriteLine($"{properties.TubeLength:N1}\t; nominal tube length (10 - 20 cm)");
      writer.WriteLine($"{properties.TubeTemp:N1}\t; tube temperature (25 - 40 degrees celsius)");
      writer.WriteLine($"{properties.JunctionLoss:N1}\t; junction loss factor (0 - 5 percent of unity gain)");
      writer.WriteLine($"{properties.AperatureScalingRadius:N2}\t; aperture scaling radius (3.05 - 12 cm)");
      writer.WriteLine($"{properties.MouthAperatureCoefficient}\t; mouth aperture coefficient (100 - nyqyist Hz)");
      writer.WriteLine($"{properties.NoseAperatureCoefficient}\t; nose aperture coefficient (100 - nyqyist Hz)");
      for (int i = 0; i < properties.NoseRadius.Count(); i++)
        writer.WriteLine($"{properties.NoseRadius.ElementAt(i):N2}\t; radius of nose section {i + 1} (0 - 3 cm)");
      writer.WriteLine($"{properties.ThroatFreqCutoff:F2}\t; throat lowpass frequency cutoff (50 - nyquist Hz)");
      writer.WriteLine($"{properties.ThroatVolume:N2}\t; throat volume (0 - 48 dB)");
      writer.WriteLine($"{properties.PulseModulationOfNoise}\t; pulse modulation of noise (0 = off, 1 = on)");
      writer.WriteLine($"{properties.NoiseCrossmixOffset:N2}\t; noise crossmix offset (30 - 60 db)");
      foreach (var state in model) {
        writer.Write($"{state.Pitch:N2} {state.Voicing:N2} {state.Aspiration:N2} {state.Frication:N2} {state.FricationPosition:N2} {state.FricationCF:N} {state.FricationBW:N2} ");
        foreach (var radius in state.RadiusSegments)
          writer.Write($"{radius:N2} ");
        writer.WriteLine($"{state.Velum:N2}");
      }
      writer.Flush();
      writer.Close();
    }
  }
}