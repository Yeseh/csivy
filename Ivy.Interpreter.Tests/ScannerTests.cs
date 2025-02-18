namespace Ivy.Interpreter.Tests;

public class ScannerTests
{
    [Test]
    [Arguments("(", TokenType.paren_left)]
    [Arguments(")", TokenType.paren_right)]
    [Arguments("{", TokenType.brace_left)]
    [Arguments("}", TokenType.brace_right)]
    [Arguments("[", TokenType.bracket_left)]
    [Arguments("]", TokenType.bracket_right)]
    [Arguments("<", TokenType.angle_left)]
    [Arguments(">", TokenType.angle_right)]
    [Arguments(".", TokenType.dot)]
    [Arguments("*", TokenType.star)]
    [Arguments("=", TokenType.equals)]
    [Arguments("+", TokenType.plus)]
    [Arguments("-", TokenType.minus)]
    [Arguments(";", TokenType.semicolon)]
    [Arguments(":", TokenType.colon)]
    public Task SingleTokens(string input, TokenType expected)
        => AssertTokens(input, expected);

    [Test]
    [Arguments("if", TokenType.keyword_if)]
    public Task Keywords(string input, TokenType expected)
        => AssertTokens(input, expected);
    
    [Test]
    [Arguments("::", TokenType.colon_colon)]
    public Task DoubleTokens(string input, TokenType expected)
        => AssertTokens(input, expected);

    private async Task AssertTokens(string input, params TokenType[] expected)
    {
        var scanner = new Scanner(input.AsSpan());

        for (int i = 0; i < expected.Length; i++)
        {
            var token = scanner.GetNextToken();
            var t = expected[i];
            await Assert.That(token.Type).IsEqualTo(t);
        }
        
        var eof = scanner.GetNextToken();
        await Assert.That(eof.Type).IsEqualTo(TokenType.eof);    
    }
}