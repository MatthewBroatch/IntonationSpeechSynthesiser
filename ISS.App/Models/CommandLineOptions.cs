using System.Collections.Generic;
using CommandLine;

public class Options
{
  [Option('p', "phones", Default = false, HelpText = "Phones mode.")]
  public bool PhonesMode { get; set; }

  [Option(HelpText = "Text to transform")]
  public string Input { get; set; }
}