using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using ISS.Infrastructure.Models;

namespace ISS.Infrastructure.Stores
{
  public class MonetPhoneticStore : IPhoneticStore
  {
    public IReadOnlyDictionary<string, Phonetic> PhoneticPostures { get; private set; }
    
    public MonetPhoneticStore(string phoneListFile)
    {
      var phonesXml = File.ReadAllText(phoneListFile);
      var phonesDocument = XDocument.Parse(phonesXml);

      PhoneticPostures = phonesDocument
        .Element("Monet")
        .Element("MainPhoneList")
        .Elements("Phone")
        .Select(phone => {
          var elements = phone.Elements().Where(x => x.Name != "Category");
          var categories = phone.Elements().Where(x => x.Name == "Category");
          var phonetic = new Phonetic()
          {
            Name = phone.Attribute("name").Value.Contains("'")
              ? "'" + phone.Attribute("name").Value.Replace("'", "")
              : phone.Attribute("name").Value,
            Categories = categories.Select(category => 
              (Category)Enum.Parse(typeof(Category), category.Attribute("name").Value, true)),
            AspVol = GetDoubleByName(elements, "aspVol"),
            GlotVol = GetDoubleByName(elements, "glotVol"),
            Duration = GetDoubleByName(elements, "duration"),
            FricBW = GetDoubleByName(elements, "fricBW"),
            FricCF = GetDoubleByName(elements, "fricCF"),
            FricPos = GetDoubleByName(elements, "fricPos"),
            FricVol = GetDoubleByName(elements, "fricVol"),
            MicroInt = GetDoubleByName(elements, "microInt"),
            Velum = GetDoubleByName(elements, "velum"),
            Qssa = GetDoubleByName(elements, "qssa"),
            Qssb = GetDoubleByName(elements, "qssb"),
            RadiusSegments = Enumerable.Range(1, 8).Select(x => GetDoubleByName(elements, $"r{x}")),
          };
          return new KeyValuePair<string, Phonetic>(phonetic.Name, phonetic);
        })
        .ToDictionary(x => x.Key, x => x.Value);
    }

    private double GetDoubleByName(IEnumerable<XElement> elements, string name)
    {
      var stringValue = elements.First(x => x.Attribute("name").Value == name).Attribute("value").Value;
      return Convert.ToDouble(stringValue);
    }
  }
}