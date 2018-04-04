using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ISS.Infrastructure.Models;
using ISS.Infrastructure.Services;
using ISS.Infrastructure.Stores;

namespace ISS.App
{
  public class ConsoleListener
  {
    private IPhoneticParserService _phoneticParserService;
    private IIntonationModelService _intonationModelService;
    private IPhoneticStore _phoneticStore;
    private ITextToPhoneticsService _textToPhoneticsService;
    private ModelToWaveService _modelToWaveService;
    private ThroatModelParameters _parameters;

    public ConsoleListener(
      IPhoneticParserService phoneticParserService,
      IIntonationModelService intonationModelService,
      IPhoneticStore phoneticStore,
      ITextToPhoneticsService textToPhoneticsService,
      ThroatModelParameters parameters,
      ModelToWaveService modelToWaveService)
    {
      _phoneticParserService = phoneticParserService;
      _intonationModelService = intonationModelService;
      _phoneticStore = phoneticStore;
      _textToPhoneticsService = textToPhoneticsService;
      _modelToWaveService = modelToWaveService;
      _parameters = parameters;
    }

    public void StartListening(bool skipParsingText)
    {
      Console.WriteLine("Enter text to convert to speech");
      while(true)
      {
        try
        {
          ConvertInput(Console.ReadLine(), skipParsingText);
        }
        catch(Exception)
        {
          Console.WriteLine("Error parsing input");
        }
      }
    }

    public void ConvertInput(string input, bool skipParsingText)
    {
      if (!skipParsingText)
        input = _textToPhoneticsService.Parse(input);

      var syllables = _phoneticParserService.Parse(input).ToList();
      var model = _intonationModelService.CreateModel(syllables).ToList();
      var waveform = _modelToWaveService.Synthesize(model).ToList();
      var file = OutputFileHelper.WriteWaveFile(waveform, _modelToWaveService.SampleRate, _parameters);

      Process.Start(@"powershell", $@"-c (New-Object Media.SoundPlayer '{file}').PlaySync();").WaitForExit();
      File.Delete(file);
      // OutputFileHelper.Print(ThroatModelParameters.CreateDefault(), model);
    }
  }
}