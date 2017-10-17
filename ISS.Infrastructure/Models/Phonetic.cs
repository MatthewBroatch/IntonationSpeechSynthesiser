using System.Collections.Generic;

namespace ISS.Infrastructure.Models
{
  internal class Phonetic
  {
    string Name { get; set; }
    double MicroInt { get; set; }
    double GlotVol { get; set; }
    double AspVol { get; set; }
    double FricVol { get; set; }
    double FricPos { get; set; }
    double FricCF { get; set; }
    double FricBW { get; set; }

    //Throat and mouth raduis
    IEnumerable<float> RaduisSegments { get; set; }

    //The bit connecting your nose and mouth
    double Velum { get; set; }
    double Duration { get; set; }
    double Transition { get; set; }

    //Todo Figure out what this is
    // double qssa { get; set; }
    // double qssb { get; set; }

    short Fricative { get; set; }
    short Affricate { get; set; }
    short Glide { get; set; }
    short Voiced { get; set; }
    short Stopped { get; set; }
    short Aspiration { get; set; }
    Vowel Vowel { get; set; }
    PhoneticEnd End { get; set; }
  }
}