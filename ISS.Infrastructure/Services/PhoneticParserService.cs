using System.Collections.Generic;
using System.Linq;
using ISS.Infrastructure.Extenstions;
using ISS.Infrastructure.Models;
using ISS.Infrastructure.Stores;

namespace ISS.Infrastructure.Services
{
  public class PhoneticParserService : IPhoneticParserService
  {
    private const char SPLITPHONECHAR = '_';
    private const char SPLITSYLLABLECHAR = '.';
    private const char SPLITWORDCHAR = ' ';
    private const char BREATH = '\t';
    private readonly IPhoneticStore _phoneticStore;

    public PhoneticParserService(IPhoneticStore phoneticStore)
    {
      _phoneticStore = phoneticStore;
    }

    public IEnumerable<Syllable> Parse(string input)
    {
      return input
        .MultiSplit(new char[] { BREATH, SPLITWORDCHAR, SPLITSYLLABLECHAR })
        .Select<(string phones, char splitChar), Syllable>(x => new Syllable() 
        {
          Postures = x.phones.Split(SPLITPHONECHAR).Select(phoneName => _phoneticStore.PhoneticPostures[phoneName]),
          SyllableEnd = 
              x.splitChar == SPLITSYLLABLECHAR ? SyllableEnd.EndSyllable
            : x.splitChar == SPLITWORDCHAR ? SyllableEnd.EndWord
            : SyllableEnd.EndWordWithBreath
        });
    }
  }
}