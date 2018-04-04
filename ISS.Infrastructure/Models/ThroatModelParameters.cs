using System.Collections.Generic;

namespace ISS.Infrastructure.Models
{
  public class ThroatModelParameters
  {
    public static ThroatModelParameters CreateDefault()
    {
      return new ThroatModelParameters() {
        OutputFileFormat = 2, // 0 = AU, 1 = AIFF, 2 = WAVE
        OutputSampleRate = 44100.0, // 22050.0, 44100.0
        InControlRate = 250.0, // 1 - 1000 Hz
        MasterVolume = 60.0, // 0 - 60 dB
        OutChannels = 1, //1 or 2
        StereoBalance =	0.0, // -1 to +1
        GlotSrcType =	0, // 0 = pulse, 1 = sine
        GlotPulseRiseTime = 40.0, // 5 - 50 percent of GP period
        GlotPulseFallTimeMin = 16.0, // 5 - 50 percent of GP period
        GlotPulseFallTimeMax = 32.0, // 5 - 50 percent of GP period
        GlotSrcBreathiness = 0.8, // 0 - 10 percent of GS amplitude
        TubeLength = 18.5, // 10 - 20 cm
        TubeTemp = 32.0, // 25 - 40 degrees celsius
        JunctionLoss = 1.5,	// 0 - 5 percent of unity gain
        AperatureScalingRadius = 3.05, // 3.05 - 12 cm
        MouthAperatureCoefficient = 5000, // 100 - nyqyist Hz
        NoseAperatureCoefficient = 5000, // 100 - hyqyist Hz
        NoseRadius = new double[] { 1.35, 1.96, 1.91, 1.30, 0.73 }, // 0 - 3 cm
        ThroatFreqCutoff = 1500.0, // 50 - nyquist Hz
        ThroatVolume = 6.0, // 0 - 48 dB
        PulseModulationOfNoise = true, //On or off
        NoiseCrossmixOffset = 48.0, // 30 - 60 dB
        TotalSections = 10,
        SectionLength = 100.0d,
        VocalTractScale = 0.25
      };
    }
    public int OutputFileFormat { get; set; }
    public double OutputSampleRate { get; set; }
    public double InControlRate { get; set; }
    public double MasterVolume { get; set; }
    public int OutChannels { get; set; }
    public double StereoBalance { get; set; }
    public GlotSrcType GlotSrcType { get; set; }
    public double GlotPulseRiseTime { get; set; }
    public double GlotPulseFallTimeMin { get; set; }
    public double GlotPulseFallTimeMax { get; set; }
    public double GlotSrcBreathiness { get; set; }
    public double TubeLength { get; set; }
    public double TubeTemp { get; set; }
    public double JunctionLoss { get; set; }
    public double AperatureScalingRadius { get; set; }
    public int MouthAperatureCoefficient { get; set; }
    public int NoseAperatureCoefficient { get; set; }
    public IEnumerable<double> NoseRadius { get; set; }
    public double ThroatFreqCutoff { get; set; }
    public double ThroatVolume { get; set; }
    public bool PulseModulationOfNoise { get; set; }
    public double NoiseCrossmixOffset { get; set; }
    public int TotalSections { get; set; }
    public double SectionLength { get; set; }
    //SCALING CONSTANT FOR INPUT TO VOCAL TRACT & THROAT
    public double VocalTractScale { get; set; }
  }
}