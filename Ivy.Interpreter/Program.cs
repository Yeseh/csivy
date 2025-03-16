
using Ivy.Interpreter;

ReadOnlySpan<char> source;

var mainIvy = @"./Ivy/main.ivy";
source = args.Length == 1 
    ? File.ReadAllText(args[1]).AsSpan()
    : File.ReadAllText(mainIvy).AsSpan(); 

var tokens = new Scanner(source).ScanTokens();
foreach (var token in tokens)
{
    Console.WriteLine(token);
}

Console.WriteLine("Done!");

public partial class Program { }


