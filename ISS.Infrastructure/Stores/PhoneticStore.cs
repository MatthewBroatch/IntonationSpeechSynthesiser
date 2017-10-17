using System.IO;
using System.Linq;

namespace ISS.Infrastructure.Stores
{
  public class PhoneticStore : IPhoneticStore
  {
    public PhoneticStore(string phoneListFile)
    {
      var phoneListLines = File.ReadAllLines(phoneListFile);
      syllableLines.Select(x => x.)
    }
  }
}