namespace Ivy.Interpreter;

using System.Runtime.ConstrainedExecution;
using static Data;
using static TokenType;

public class Scanner(ReadOnlySpan<char> source)
{
    public readonly record struct Error(int Line, string Message);
    
    
    private readonly char[] _src = source.ToArray();
    private readonly int _len = source.Length;

    private List<Token> tokens = new(source.Length / 2);
    private int _start = 0;
    private int _pos = 0;
    private int _line = 1;

    public List<Token> ScanTokens()
    {
        while (!IsAtEnd()) { ScanToken(); }
        tokens.Add(new Token(_pos, _pos, _line, eof));
        return tokens;
    }
    
    public void ScanToken()
    {
        _start = _pos;
        var c = _src[_pos++];
        var token = new Token(_start, _pos, _line, eof);
        
        switch(c)
        {
            case '\n': _line += 1; break;
            case '\t': 
            case ' ': 
                break;
            case '\r':
            {
                if (Peek() == '\n')
                {
                    _pos++; 
                    _line++; 
                }
                else token.Type = invalid; 
                break;
            } 
            
            case '(': token.Type = paren_left; break; 
            case ')': token.Type = paren_right; break; 
            case '{': token.Type = brace_left; break;  
            case '}': token.Type = brace_right; break;  
            case '[': token.Type = bracket_left; break;  
            case ']': token.Type = bracket_right;break; 
            case '.': token.Type = dot; break;  
            case '*': token.Type = star; break;  
            case ';': token.Type = semicolon; break; 

            case '<': token.Type = MatchNext('=') ? lesser_equals : angle_left; break; 
            case '>': token.Type = MatchNext('=') ? greater_equals : angle_right; break;
            case '+': token.Type = MatchNext('=') ? plus_equals : plus; break;
            case '-': token.Type = MatchNext('=') ? minus_equals : minus; break;
            case '!': token.Type = MatchNext('=') ? bang_equals : bang; break; 
            case '=': token.Type = MatchNext('=') ? equals_equals : equals; break; 
            case ':': token.Type = MatchNext(':') ? colon_colon: colon; break; 
            
            case '/':
            {
                if (MatchNext('/'))
                {
                    token.Type = comment_line;
                    AdvanceUntilNewline();
                }
                else token.Type = slash_forward;
                break;
            }

            case '"':
            {
                while (Peek() != '"' && !IsAtEnd())
                {
                    if (Peek() == '\n') { _line++; }
                    // TODO: string escapes
                    _pos++;
                }
                
                if (IsAtEnd()) { token.Type = invalid; break; }
                
                _pos++;
                token.Type = literal_string;
                break;
            }

            case var _ when char.IsNumber(c):
            {
               while (char.IsNumber(Peek())) _pos++;
               if (Peek() == '.' && char.IsNumber(PeekNext()))
               {
                   _pos++;
                   while (char.IsNumber(Peek())) { _pos++; }
               }

               token.Type = literal_number;
               break; 
            }
            
            case var _ when char.IsWhiteSpace(c): token.Start = _pos + 1;  break;
            case var _ when char.IsLetter(c) || c == '_':
            {
               while (char.IsLetterOrDigit(Peek()) || Peek() == '_') _pos++;
               
               var isKeyword = KeywordLookup.TryGetValue(_src[_start.._pos], out var tokenType);
               token.Type = isKeyword ? tokenType : identifier; 
               
               break;
            }

            case var _ when char.IsNumber(c):
            {
                token.Type = literal_number;
                break;
            }

            default:
                token.Type = invalid;
                token.End = _pos;
                _pos += 1;
                break;
        }

        if (token.Type != eof)
        {
            token.End = _pos;
            tokens.Add(token);
        }
    }
    
    

    public bool MatchNext(char expected)
    {
        if (IsAtEnd()) { return false; }
        if (_src[_pos] != expected) { return false; }
        
        _pos++;
        return true;
    }

    public char Peek()
    {
        if (IsAtEnd()) { return '\0'; }
        return _src[_pos];
    }

    public char PeekNext()
    {
        if (_pos + 1 > _src.Length) return '\0';
        return _src[_pos + 1];
    }
    
    public void AdvanceIdentifier()
    {
       var cur = _src[_pos];
       while (char.IsBetween(cur, 'a', 'Z') || cur == '_' && !IsAtEnd())
       {
           _pos++;
       }
    }

    public void AdvanceUntilNewline()
    {
        while (Peek() != '\n' && !IsAtEnd())
        {
           _pos++; 
        }
    }

    public bool IsAtEnd() => _pos >= _len;
};