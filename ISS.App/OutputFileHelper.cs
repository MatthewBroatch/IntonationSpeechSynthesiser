using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using ISS.Infrastructure.Helpers;
using ISS.Infrastructure.Models;

namespace ISS.App
{
  public static class OutputFileHelper
  {
    public const string OutputFileName = "intonation-synth-output.txt";
    public static void Print(ThroatModelParameters properties, IEnumerable<IntonationModel> model)
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
      writer.WriteLine($"{properties.GlotPulseRiseTime:N1}\t; glottal pulse rise time (5 - 50 percent of GP period)");
      writer.WriteLine($"{properties.GlotPulseFallTimeMin:N1}\t; glottal pulse fall time minimum (5 - 50 percent of GP period)");
      writer.WriteLine($"{properties.GlotPulseFallTimeMax:N1}\t; glottal pulse fall time maximum (5 - 50 percent of GP period)");
      writer.WriteLine($"{properties.GlotSrcBreathiness:N1}\t; glottal source breathiness (0 - 10 percent of GS amplitude)");
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
        writer.Write($"{state.Pitch:N2} {state.GlotVolume:N2} {state.AspirationVolume:N2} {state.FricationVolume:N2} {state.FricationPosition:N2} {state.FricationCF:N} {state.FricationBW:N2} ");
        foreach (var radius in state.RadiusSegments)
          writer.Write($"{radius:N2} ");
        writer.WriteLine($"{state.Velum:N2}");
      }
      writer.Flush();
      writer.Close();
    }

    public static string WriteWaveFile(IEnumerable<double> waveform, double sampleRate, ThroatModelParameters parameters)
    {
      var outputScale = 0.25;
      var rangeMax = 32767d;
      var balance = 0;

      // var sampleRateRatio = outputRate / sampleRate;
      var (fileStream, path) = CreateFileStream();
      //WriteWaveFileHeader(2, waveform.Count(), parameters.OutputSampleRate, fileStream);
      WriteWaveFileHeader(2, waveform.Count(), sampleRate, fileStream);
      var scale = outputScale * (rangeMax / waveform.Max()) * WaveHelper.CalculateAmplitude(parameters.MasterVolume);
      var leftScale = -((balance / 2.0) - 0.5) * scale * 2.0;
	    var rightScale = ((balance / 2.0) + 0.5) * scale * 2.0;
      foreach(var data in waveform) {
        WriteShort(fileStream, (short)Math.Round(data * leftScale));
        WriteShort(fileStream, (short)Math.Round(data * rightScale));
      }
      fileStream.Flush();
      fileStream.Close();
      return path;
      // int endPtr;
      // /*  CALCULATE END POINTER  */
      // endPtr = fillPtr - padSize;

      // /*  ADJUST THE END POINTER, IF LESS THAN ZERO  */
      // if (endPtr < 0)
      //   endPtr += BUFFER_SIZE;

      // /*  ADJUST THE ENDPOINT, IF LESS THEN THE EMPTY POINTER  */
      // if (endPtr < emptyPtr)
      //   endPtr += BUFFER_SIZE;

      /*  UPSAMPLE LOOP (SLIGHTLY MORE EFFICIENT THAN DOWNSAMPLING)  */
      // if (sampleRateRatio >= 1.0) {
      //   while (emptyPtr < endPtr) {
      //     int index;
      //     unsigned int filterIndex;
      //     double output, interpolation, absoluteSampleValue;

      //     /*  RESET ACCUMULATOR TO ZERO  */
      //     output = 0.0;

      //     /*  CALCULATE INTERPOLATION VALUE (STATIC WHEN UPSAMPLING)  */
      //     interpolation = (double)mValue(timeRegister) / (double)M_RANGE;

      //     /*  COMPUTE THE LEFT SIDE OF THE FILTER CONVOLUTION  */
      //     index = emptyPtr;
      //     for (filterIndex = lValue(timeRegister); filterIndex < FILTER_LENGTH; srDecrement(&index,BUFFER_SIZE), filterIndex += filterIncrement) {
      //       output += (buffer[index] * (h[filterIndex] + (deltaH[filterIndex] * interpolation)));
      //     }

      //     /*  ADJUST VALUES FOR RIGHT SIDE CALCULATION  */
      //     timeRegister = ~timeRegister;
      //     interpolation = (double)mValue(timeRegister) / (double)M_RANGE;

      //     /*  COMPUTE THE RIGHT SIDE OF THE FILTER CONVOLUTION  */
      //     index = emptyPtr;
      //     srIncrement(&index,BUFFER_SIZE);
      //     for (filterIndex = lValue(timeRegister); filterIndex < FILTER_LENGTH; srIncrement(&index,BUFFER_SIZE), filterIndex += filterIncrement) {
      //       output += (buffer[index] * (h[filterIndex] + (deltaH[filterIndex] * interpolation)));
      //     }

      //     /*  RECORD MAXIMUM SAMPLE VALUE  */
      //     absoluteSampleValue = fabs(output);
      //     if (absoluteSampleValue > maximumSampleValue)
      //       maximumSampleValue = absoluteSampleValue;

      //     /*  INCREMENT SAMPLE NUMBER  */
      //     numberSamples++;

      //     /*  OUTPUT THE SAMPLE TO THE TEMPORARY FILE  */
      //     fwrite((char *)&output, sizeof(output), 1, tempFilePtr);

      //     /*  CHANGE TIME REGISTER BACK TO ORIGINAL FORM  */
      //     timeRegister = ~timeRegister;

      //     /*  INCREMENT THE TIME REGISTER  */
      //     timeRegister += timeRegisterIncrement;

      //     /*  INCREMENT THE EMPTY POINTER, ADJUSTING IT AND END POINTER  */
      //     emptyPtr += nValue(timeRegister);

      //     if (emptyPtr >= BUFFER_SIZE) {
      //       emptyPtr -= BUFFER_SIZE;
      //       endPtr -= BUFFER_SIZE;
      //     }

      //     /*  CLEAR N PART OF TIME REGISTER  */
      //     timeRegister &= (~N_MASK);
      //   }
      // }
      // else { /*  DOWNSAMPLING CONVERSION LOOP  */
      //   while (emptyPtr < endPtr) {
      //     int index;
      //     unsigned int phaseIndex, impulseIndex;
      //     double absoluteSampleValue, output, impulse;

      //     /*  RESET ACCUMULATOR TO ZERO  */
      //     output = 0.0;

      //     /*  COMPUTE P PRIME  */
      //     phaseIndex = (unsigned int)rint(((double)fractionValue(timeRegister)) * sampleRateRatio);

      //     /*  COMPUTE THE LEFT SIDE OF THE FILTER CONVOLUTION  */
      //     index = emptyPtr;
      //     while ((impulseIndex = (phaseIndex>>M_BITS)) < FILTER_LENGTH) {
      //       impulse = h[impulseIndex] + (deltaH[impulseIndex] * (((double)mValue(phaseIndex)) / (double)M_RANGE));
      //       output += (buffer[index] * impulse);
      //       srDecrement(&index,BUFFER_SIZE);
      //       phaseIndex += phaseIncrement;
      //     }

      //     /*  COMPUTE P PRIME, ADJUSTED FOR RIGHT SIDE  */
      //     phaseIndex = (unsigned int)rint(((double)fractionValue(~timeRegister)) * sampleRateRatio);

      //     /*  COMPUTE THE RIGHT SIDE OF THE FILTER CONVOLUTION  */
      //     index = emptyPtr;
      //     srIncrement(&index,BUFFER_SIZE);
      //     while ((impulseIndex = (phaseIndex>>M_BITS)) < FILTER_LENGTH) {
      //       impulse = h[impulseIndex] + (deltaH[impulseIndex] * (((double)mValue(phaseIndex)) / (double)M_RANGE));
      //       output += (buffer[index] * impulse);
      //       srIncrement(&index,BUFFER_SIZE);
      //       phaseIndex += phaseIncrement;
      //     }

      //     /*  RECORD MAXIMUM SAMPLE VALUE  */
      //     absoluteSampleValue = fabs(output);
      //     if (absoluteSampleValue > maximumSampleValue)
      //       maximumSampleValue = absoluteSampleValue;

      //     /*  INCREMENT SAMPLE NUMBER  */
      //     numberSamples++;

      //     /*  OUTPUT THE SAMPLE TO THE TEMPORARY FILE  */
      //     fwrite((char *)&output, sizeof(output), 1, tempFilePtr);

      //     /*  INCREMENT THE TIME REGISTER  */
      //     timeRegister += timeRegisterIncrement;

      //     /*  INCREMENT THE EMPTY POINTER, ADJUSTING IT AND END POINTER  */
      //     emptyPtr += nValue(timeRegister);
      //     if (emptyPtr >= BUFFER_SIZE) {
      //       emptyPtr -= BUFFER_SIZE;
      //       endPtr -= BUFFER_SIZE;
      //     }

      //     /*  CLEAR N PART OF TIME REGISTER  */
      //     timeRegister &= (~N_MASK);
      //   }
      // }
    }
    private static (FileStream, string) CreateFileStream() {
      var dir = Directory.GetCurrentDirectory();
      var fileName = Guid.NewGuid().ToString() + ".wav";
      var path = Path.Combine(dir, fileName);
      return (File.Create(path), path);
    }

    private static void WriteWaveFileHeader(int channels, int numberSamples, double outputRate, FileStream outputFile)
    {
      int soundDataSize = channels * numberSamples * sizeof(short);
      int dataChunkSize = soundDataSize;
      int formSize = dataChunkSize + 8 + 24 + 4;
      var bitsPerSample = 16;
      int frameSize = (int)Math.Ceiling(channels * ((double)bitsPerSample / 8));
      int bytesPerSecond = (int)Math.Ceiling(outputRate * frameSize);

      /*  Form container identifier  */
      WriteString(outputFile, "RIFF");

      /*  Form size  */
      WriteInt(outputFile, formSize);

      /*  Form container type  */
      WriteString(outputFile, "WAVE");

      /*  Format chunk identifier (Note:  space after 't' needed)  */
      WriteString(outputFile, "fmt ");

      /*  Chunk size (fixed at 16 bytes)  */
      WriteInt(outputFile, 16);

      /*  Compression code:  1 = PCM  */
      WriteShort(outputFile, 1);

      /*  Number of channels  */
      WriteShort(outputFile, 2);

      /*  Output Sample Rate  */
      WriteInt(outputFile, (int)outputRate);

      /*  Bytes per second  */
      WriteInt(outputFile, bytesPerSecond);

      /*  Block alignment (frame size)  */
      WriteShort(outputFile, (short)frameSize);

      /*  Bits per sample  */
      WriteShort(outputFile, (short)bitsPerSample);

      /*  Sound Data chunk identifier  */
      WriteString(outputFile, "data");

      /*  Chunk size  */
      WriteInt(outputFile, dataChunkSize);
    }

    private static void WriteInt(FileStream outputFile, int value) {
      var data = BitConverter.GetBytes(value);
      if (!BitConverter.IsLittleEndian)
          Array.Reverse(data);
      outputFile.Write(data, 0, data.Length);
    }

    private static void WriteShort(FileStream outputFile, short value) {
      var data = BitConverter.GetBytes(value);
      if (!BitConverter.IsLittleEndian)
          Array.Reverse(data);
      outputFile.Write(data, 0, data.Length);
    }

    private static void WriteString(FileStream outputFile, string value) {
      var data = new ASCIIEncoding().GetBytes(value);
      outputFile.Write(data, 0, data.Length);
    }
  }
}