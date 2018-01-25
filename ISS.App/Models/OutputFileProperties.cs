using System.Collections.Generic;

namespace ISS.App.Models
{
  public class OutputFileProperties
  {
    public int OutputFileFormat { get; set; }
    public double OutputSampleRate { get; set; }
    public double InControlRate { get; set; }
    public double MasterVolume { get; set; }
    public int OutChannels { get; set; }
    public double StereoBalance { get; set; }
    public int GlotSrcType { get; set; }
    public double GlotPulseRise { get; set; }
    public double GlotPulseFallMin { get; set; }
    public double GlotPulseFallMax { get; set; }
    public double GlotSrcBreath { get; set; }
    public double TubeLength { get; set; }
    public double TubeTemp { get; set; }
    public double JunctionLoss { get; set; }
    public double AperatureScalingRadius { get; set; }
    public int MouthAperatureCoefficient { get; set; }
    public int NoseAperatureCoefficient { get; set; }
    public IEnumerable<double> NoseRadius { get; set; }
    public double ThroatFreqCutoff { get; set; }
    public double ThroatVolume { get; set; }
    public int PulseModulationOfNoise { get; set; }
    public double NoiseCrossmixOffset { get; set; }
  }
}