using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ISS.Infrastructure.Helpers;
using ISS.Infrastructure.Models;

namespace ISS.Infrastructure.Services
{
  public class ModelToWaveService
  {
    public double SampleRate;
    public ModelToWaveService(ThroatModelParameters parameters)
    {
      if (parameters.TubeLength <= 0)
        throw new Exception("Illegal tube length.");

      _parameters = parameters;
      ///CALCULATE THE SAMPLE RATE, BASED ON NOMINAL
      ///TUBE LENGTH AND SPEED OF SOUND
      var speedOfSound = WaveHelper.SpeedOfSound(parameters.TubeTemp);
      _controlPeriod = Math.Round((speedOfSound * parameters.TotalSections * parameters.SectionLength) /(parameters.TubeLength * parameters.InControlRate));
      SampleRate = parameters.InControlRate * _controlPeriod;
      //var actualTubeLength = (speedOfSound * totalSections * sectionLength) / sampleRate;
      _nyquist = SampleRate / 2.0;
      _breathinessFactor = parameters.GlotSrcBreathiness / 100d;
      _crossmixFactor = 1.0d / WaveHelper.CalculateAmplitude(parameters.NoiseCrossmixOffset);
      _dampingFactor = (1.0d - (parameters.JunctionLoss / 100d));
      _waveTable = new WaveTable(SampleRate, parameters);
      _bandpassFilter = new BandpassFilter();
      _vocalTract = new VocalTract(parameters, _nyquist, SampleRate);
      // initializeFIRFilter(FIR_BETA, FIR_GAMMA, FIR_CUTOFF);
      // /*  INITIALIZE THE SAMPLE RATE CONVERSION ROUTINES  */
      // initializeConversion();

      /*  INITIALIZE THE TEMPORARY OUTPUT FILE  */
      // tempFilePtr = tmpfile();
      // rewind(tempFilePtr);
    }

    public IEnumerable<double> Synthesize(IEnumerable<IntonationModel> model)
    {
      double frequency, amplitude, aspVol, pulse, noise, pulsedNoise, signal, crossmix;
      /*  CONTROL RATE LOOP  */
      foreach(var modelSection in model)
      {
        /*  SAMPLE RATE LOOP  */
        for (int i = 0; i < _controlPeriod; i++)
        {
          /*  CONVERT PARAMETERS HERE  */
          frequency = WaveHelper.CaclulateFrequency(modelSection.Pitch);
          amplitude = WaveHelper.CalculateAmplitude(modelSection.GlotVolume);
          aspVol = WaveHelper.CalculateAmplitude(modelSection.AspirationVolume);
          _vocalTract.CalculateTubeCoefficients(modelSection, _parameters);
          _vocalTract.SetFricationTaps(modelSection);
          _bandpassFilter.Update(modelSection.FricationBW, modelSection.FricationCF, SampleRate);

          noise = WaveHelper.Noise();

          /*  UPDATE THE SHAPE OF THE GLOTTAL PULSE, IF NECESSARY  */
          if (_parameters.GlotSrcType == GlotSrcType.Pulse)
            _waveTable.UpdateWavetable(amplitude);

          /*  CREATE GLOTTAL PULSE (OR SINE TONE)  */
          pulse = _waveTable.Oscillator(frequency);

          /*  CREATE PULSED NOISE  */
          pulsedNoise = noise * pulse;

          /*  CREATE NOISY GLOTTAL PULSE  */
          pulse = amplitude * ((pulse * (1.0 - _breathinessFactor)) + (pulsedNoise * _breathinessFactor));

          /*  CROSS-MIX PURE NOISE WITH PULSED NOISE  */
          if (_parameters.PulseModulationOfNoise)
          {
            crossmix = amplitude * _crossmixFactor;
            crossmix = (crossmix < 1.0) ? crossmix : 1.0;
            signal = (pulsedNoise * crossmix) + (noise * (1.0 - crossmix));
          }
          else
            signal = noise;

          /*  PUT SIGNAL THROUGH VOCAL TRACT  */
          signal = _vocalTract.CalculateVocalTractSignal(((pulse + (aspVol * signal)) * _parameters.VocalTractScale), _bandpassFilter.Filter(signal), _dampingFactor);
          /*  PUT PULSE THROUGH THROAT  */
          signal += _vocalTract.CalculateThroatSignal(pulse * _parameters.VocalTractScale);

          // /*  OUTPUT SAMPLE HERE  */
          // dataFill(signal);
          yield return signal;

          // /*  DO SAMPLE RATE INTERPOLATION OF CONTROL PARAMETERS  */
          // sampleRateInterpolation();
        }
      }
      yield break;
    }

    private double _breathinessFactor;
    private double _crossmixFactor;
    private double _dampingFactor;
    private WaveTable _waveTable;
    private BandpassFilter _bandpassFilter;
    private ThroatModelParameters _parameters;
    private double _nyquist;
    private double _controlPeriod;
    private VocalTract _vocalTract;
  }
}