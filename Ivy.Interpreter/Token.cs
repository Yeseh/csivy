using System.Runtime.InteropServices;

namespace Ivy.Interpreter;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public record struct Token(int Start, int End, int Line, TokenType Type)
{
    public override string ToString() => $"[{Type}] {Line}:{Start}-{End}";
};