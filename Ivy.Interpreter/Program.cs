
using Ivy.Interpreter;

ReadOnlySpan<char> source;

if (args.Length > 1) 
{
    Console.WriteLine("Usage: csivy [script]");
    return 64;
}

source = args.Length == 1 
    ? File.ReadAllText(args[1]).AsSpan()
    : Console.ReadLine().AsSpan();

var error = new Interpreter(source).Execute();
if (error.HasValue)
{
    Console.WriteLine($"Error: {error.Value.Message}");
    return 1;
}


Console.WriteLine("Done!");
return 0;


public partial class Program { }


