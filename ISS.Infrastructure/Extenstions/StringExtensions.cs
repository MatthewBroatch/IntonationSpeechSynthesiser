using System;
using System.Collections.Generic;
using System.Linq;

namespace ISS.Infrastructure.Extenstions
{
  public static class StringExtensions
  {
    public static IEnumerable<(string, char)> MultiSplit(this string value, params char[] separators)
    {
      if (separators.Length == 0) {
        yield return (value, default(char));
        yield break;
      }
      
      var results = value.Split(separators[0])
        .SelectMany(result => MultiSplit(result, separators.Skip(1).ToArray()))
        .ToList();

      var lastResult = results.Last();
      results.RemoveAt(results.Count - 1);

      foreach(var result in results)
        yield return result;
        
      lastResult.Item2 = separators[0];
      yield return lastResult;
      yield break;
    }
  }
}