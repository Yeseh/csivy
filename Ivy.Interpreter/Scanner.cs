namespace Ivy.Interpreter;

using System.Runtime.ConstrainedExecution;
using static Data;

public class Scanner(ReadOnlySpan<char> source)
{
    public enum State : byte
    {
        start,
        plus,
        minus,
        bang,
        slash,
        equals,
        angle_right,
        angle_left,
        colon,
        identifier,
        comment_start,
        literal_string,
        literal_number,
        comment_multiline,
        comment_multiline_saw_bang,
        comment_line,
        newline
    }
    
    // Scanner takes ownership of the source buffer
    private readonly char[] _src = source.ToArray();
    private readonly int _len = source.Length;

    private State _state;
    private int _pos = 0;
    private int _line = 0;
    private Token? _pendingInvalid = null;

    public Token GetNextToken()
    {
        _state = State.start;
        Token token = new Token(_pos, _pos, _line, TokenType.eof);

        while (_pos < _len)
        {
            var c =_src[_pos];
            switch(_state) {
                
                case State.start:
                    switch(c)
                    {
                        case var _ when char.IsWhiteSpace(c): token.Start = _pos + 1;  break;
                        case var _ when c.ToString() == Environment.NewLine: _pos += 1; _line += 1; break; 
                        
                        case '(':  token.Type = TokenType.paren_left; _pos += 1; return token; 
                        case ')':  token.Type = TokenType.paren_right; _pos += 1; return token; 
                        case '{':  token.Type = TokenType.brace_left; _pos += 1; return token;  
                        case '}':  token.Type = TokenType.brace_right; _pos += 1; return token;  
                        case '[':  token.Type = TokenType.bracket_left; _pos += 1; return token;  
                        case ']':  token.Type = TokenType.bracket_right; _pos += 1; return token; 
                        case '.':  token.Type = TokenType.dot; _pos += 1; return token;  
                        case '*':  token.Type = TokenType.star; _pos += 1; return token;  
                        case ';':  token.Type = TokenType.semicolon; _pos += 1; return token; 

                        case '<':  _state = State.angle_left; break; 
                        case '>':  _state = State.angle_right; break; 
                        case '+':  _state = State.plus; break;  
                        case '-':  _state = State.minus; break;  
                        case '/':  _state = State.slash; break;  
                        case '!':  _state = State.bang; break; 
                        case '=':  _state = State.equals; break; 
                        case ':':  _state = State.colon; break;  
                        case '#':  _state = State.comment_start; break; 
                        case '"':  _state = State.literal_string; break; 
                        
                        case var _ when char.IsLetter(c) || c == '_':
                        {
                           token.Type = TokenType.identifier; 
                           _state = State.identifier;
                           break;
                        }

                        case var _ when char.IsNumber(c):
                        {
                            _state = State.literal_number;
                            token.Type = TokenType.literal_number;
                            break;
                        }

                        default:
                            token.Type = TokenType.invalid;
                            token.End = _pos;
                            _pos += 1;
                            return token;
                    }
                    break;
                
                case State.plus:
                    token.Type = Peek() switch
                    {
                        '=' => TokenType.plus_equals,
                        _ => TokenType.plus
                    };

                    _pos += 1;
                    return token;
                
                case State.minus:
                    token.Type = c switch
                    {
                        '=' => TokenType.minus_equals,
                        _ => TokenType.minus
                    };
                    _pos += 1;
                    return token;
                
                case State.bang:
                    token.Type = Peek() switch
                    {
                        '=' => TokenType.bang_equals,
                        _ => TokenType.bang
                    };
                    _pos += 1;
                    return token;
                
                case State.equals:
                    token.Type = Peek() switch
                    {
                        '=' => TokenType.equals_equals,
                        _ => TokenType.equals,
                    };
                    _pos += 1;
                    return token;
                
                case State.angle_right:
                    token.Type = Peek() switch
                    {
                        '=' => TokenType.greater_equals,
                        _ => TokenType.angle_right
                    };
                    _pos += 1;
                    return token;
                
                case State.angle_left:
                    token.Type = c switch
                    {
                        '=' => TokenType.lesser_equals,
                        _ => TokenType.angle_left
                    };
                    _pos += 1;
                    return token;
                
                case State.colon:
                    switch (Peek())
                    {
                        case ':': token.Type = TokenType.colon_colon; _pos += 1; break;
                        default: token.Type = TokenType.colon; break;
                    }
                    _pos += 1;
                    return token;
                
                case State.identifier:
                    switch (c)
                    {
                        // Skip through letters
                        case var _ when char.IsLetter(c) || c == '_': _pos += 1; break;
                        default:
                            var lexeme = _src[token.Start.._pos];
                            var isKeyword = KeywordLookup.TryGetValue(lexeme, out var keyword);
                            if (isKeyword)
                            {
                                token.Type = keyword;
                            }
                            
                            return token;
                    }
                    break;
                
                case State.comment_start:
                    switch (c)
                    {
                        case '!': _state = State.comment_multiline; _pos += 1; break;
                        case '\n': _state = State.start; _pos += 1; _line += 1; break;
                        case var _ when char.IsWhiteSpace(c): _pos += 1; break;
                        default: _state = State.comment_line; break; 
                    }
                    break;
                
                case State.comment_line:
                    switch (c)
                    {
                        case var _ when IsNewLine():
                            _state = State.start;
                            token.Start = _pos + 1;
                            _line += 1;
                            break;
                        
                        case var _ when char.IsWhiteSpace(c):
                        default:
                            break;
                    }
                    break;
                
                case State.comment_multiline:
                    switch (c)
                    {
                        case '\0':
                        {
                            if (_pos != _len)
                            {
                                token.Type = TokenType.invalid;
                                _pos += 1;
                                return token;
                            }
                            break; 
                        }
                        case '!': _state = State.comment_multiline_saw_bang; break;
                        case var _ when c.ToString() == Environment.NewLine:
                        default:
                            break;
                    }
                    break;
            }
        }
        
        if (token.Type == TokenType.eof)
        {
            if (_pendingInvalid is not null)
            {
                var invalidToken = _pendingInvalid.Value;
                _pendingInvalid = null;
                return invalidToken;
            }
            token.Start = _pos;
        }
        
        token.End = _pos;
        return token; 
    }

    public bool IsNewLine()
    {
        var cur = _src[_pos];
        var isNewline = cur == '\n'
            || cur == '\r' && Peek() == '\n';

        if (!isNewline) 
        {
            throw new Exception("Invalid newline detected!");
        }

        return true;
    }
    
    public char? Peek() => _pos + 1 < _len ? _src[_pos] : null;
};