namespace Ivy.Interpreter;

public static class Data 
{
    private static Dictionary<string, TokenType> KeyWords = new()
    {
        ["if"] = TokenType.keyword_if,
        ["else"] = TokenType.keyword_else,
        ["return"] = TokenType.keyword_return,
        ["for"] = TokenType.keyword_for,
        ["var"] = TokenType.keyword_var,
        ["fn"] = TokenType.keyword_fn,    
    };

    public static Dictionary<string, TokenType>.AlternateLookup<ReadOnlySpan<char>> 
        KeywordLookup => KeyWords.GetAlternateLookup<ReadOnlySpan<char>>();
}