namespace Ivy.Interpreter;

public enum TokenType : byte
{
    paren_left,
    paren_right,
    brace_left,
    brace_right,
    bracket_left,
    bracket_right,
    angle_right,
    angle_left,
    dot,
    star,
    equals,
    plus,
    minus,
    semicolon,
    bang,
    colon,
    quote,
    quote_double,
    slash_forward,

    identifier,
    literal_string,
    literal_number,

    bang_equals,
    dot_dot,
    thin_arrow_right,
    thin_arrow_left,
    fat_arrow_right,
    fat_arrow_left,
    greater_equals,
    plus_equals,
    minus_equals,
    lesser_equals,
    equals_equals,
    comment_line,
    comment_block_start,
    comment_block_end,
    colon_colon,
    

    keyword_fn,
    keyword_if,
    keyword_else,
    keyword_return,
    keyword_for,
    keyword_var,

    invalid,
    eof
}