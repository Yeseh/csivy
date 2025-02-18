namespace Ivy.Interpreter;

public ref struct Interpreter
{
    private Scanner _scanner;

    public Interpreter(ReadOnlySpan<char> source)
    {
        _scanner = new Scanner(source);
    }

    public InterpreterError? Execute()
    {
        var cur = _scanner.GetNextToken();

        while (true)
        {
            if (cur.Type == TokenType.eof)
            {
                break;
            }
        }

        return null;
    }
}

