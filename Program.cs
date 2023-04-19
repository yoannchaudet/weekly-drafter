﻿using CommandLine;

Parser.Default.ParseArguments<Options>(args).WithParsed<Options>(o =>
{
  if (o.Verbose)
  {
    Console.WriteLine($"Verbose output enabled. Current Arguments: -v {o.Verbose}");
    Console.WriteLine("Quick Start Example! App is in Verbose mode!");
  }
  else
  {
    Console.WriteLine($"Current Arguments: -v {o.Verbose}");
    Console.WriteLine("Quick Start Example!");
  }
});

public class Options
{
  [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
  public bool Verbose { get; set; }
}