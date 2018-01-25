using System;
using ISS.App.Models;
using ISS.Infrastructure.Services;
using ISS.Infrastructure.Stores;

namespace ISS.App
{
  public class ConsoleListener
  {
    private IPhoneticParserService _phoneticParserService;
    private IIntonationModelService _intonationModelService;

    public ConsoleListener(
      IPhoneticParserService phoneticParserService,
      IIntonationModelService intonationModelService)
    {
      _phoneticParserService = phoneticParserService;
      _intonationModelService = intonationModelService;
    }

    public void StartListening()
    {
      Console.WriteLine("Enter text to convert to speech");
      while(true)
      {
        try
        {
          ConvertInput(Console.ReadLine());
        }
        catch(Exception)
        {
          Console.WriteLine("Error parsing input");
        }
      }
    }

    public void ConvertInput(string input)
    {
      var syllables = _phoneticParserService.Parse(input);
          var model = _intonationModelService.CreateModel(syllables);
          OutputFileHelper.Print(new OutputFileProperties() {
            OutputFileFormat = 2, // 0 = AU, 1 = AIFF, 2 = WAVE
            OutputSampleRate = 44100.0, // 22050.0, 44100.0
            InControlRate = 250.0, // 1 - 1000 Hz
            MasterVolume = 60.0, // 0 - 60 dB
            OutChannels = 1, //1 or 2
            StereoBalance =	0.0, // -1 to +1
            GlotSrcType =	0, // 0 = pulse, 1 = sine
            GlotPulseRise = 40.0, // 5 - 50 percent of GP period
            GlotPulseFallMin = 16.0, // 5 - 50 percent of GP period
            GlotPulseFallMax = 32.0, // 5 - 50 percent of GP period
            GlotSrcBreath = 0.8, // 0 - 10 percent of GS amplitude
            TubeLength = 18.5, // 10 - 20 cm
            TubeTemp = 32.0, // 25 - 40 degrees celsius
            JunctionLoss = 1.5,	// 0 - 5 percent of unity gain
            AperatureScalingRadius = 3.05, // 3.05 - 12 cm
            MouthAperatureCoefficient = 5000, // 100 - nyqyist Hz
            NoseAperatureCoefficient = 5000, // 100 - hyqyist Hz
            NoseRadius = new double[] { 1.35, 1.96, 1.91, 1.30, 0.73 }, // 0 - 3 cm
            ThroatFreqCutoff = 1500.0, // 50 - nyquist Hz
            ThroatVolume = 6.0, // 0 - 48 dB
            PulseModulationOfNoise = 1, // 0 = off, 1 = on
            NoiseCrossmixOffset = 48.0, // 30 - 60 dB
          }, model);
    }
  }
}