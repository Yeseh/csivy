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
    [Arguments("/", TokenType.slash_forward)]
    public Task SingleTokens(string input, TokenType expected)
        => AssertTokens(input, expected);

    [Test]
    [Arguments("if", TokenType.keyword_if)]
    [Arguments("var", TokenType.keyword_var)]
    [Arguments("return", TokenType.keyword_return)]
    [Arguments("else", TokenType.keyword_else)]
    [Arguments("fn", TokenType.keyword_fn)]
    [Arguments("for", TokenType.keyword_for)]
    public Task Keywords(string input, TokenType expected)
        => AssertTokens(input, expected);
    
    [Test]
    [Arguments("::", TokenType.colon_colon)]
    [Arguments("+=", TokenType.plus_equals)]
    [Arguments("-=", TokenType.minus_equals)]
    [Arguments("!=", TokenType.bang_equals)]
    [Arguments("==", TokenType.equals_equals)]
    [Arguments(">=", TokenType.greater_equals)]
    [Arguments("<=", TokenType.lesser_equals)]
    [Arguments("//", TokenType.comment_line)]
    public Task DoubleTokens(string input, TokenType expected)
        => AssertTokens(input, expected);

    [Test]
    [Arguments("\"bla\"")]
    [Arguments("\"bla bla\"")]
    [Arguments("\"bla\nbla\"")]
    public Task Strings(string input) 
        => AssertTokens(input, TokenType.literal_string);
    
    [Test]
    [Arguments("bla")]
    [Arguments("bla_bla")]
    public Task Identifiers(string input)
        => AssertTokens(input, TokenType.identifier);
    
    [Test]
    [Arguments("1")]
    [Arguments("100")]
    [Arguments("1.0")]
    [Arguments("100.2")]
    public Task Numbers(string input) 
        => AssertTokens(input, TokenType.literal_number);

    private async Task AssertTokens(string input, params TokenType[] expected)
    {
        var scanner = new Scanner(input.AsSpan());
        var tokens = scanner.ScanTokens();

        await Assert.That(tokens.Count).IsEqualTo(expected.Length+1);
        for (int i = 0; i < expected.Length; i++)
        {
            var token = tokens[i];
            var expectedToken = expected[i];
            await Assert.That(token.Type).IsEqualTo(expectedToken);
        }
        
        await Assert.That(tokens.Last().Type).IsEqualTo(TokenType.eof);
    }
}