using System.Text;

namespace Ivy.Interpreter;

public class AstPrinter : AstVisitor<string>
{
    public string Visit(Binary expr)
    {
        
    }

    public string Visit(Literal expr)
    {
        throw new NotImplementedException();
    }

    public string Visit(Grouping expr)
    {
        throw new NotImplementedException();
    }

    public string Visit(Unary expr)
    {
        throw new NotImplementedException();
    }

    private string Parenthesize(string name, params Expr[] exprs)
    {
        var sb = new StringBuilder("(");
        sb.Append(name);
        foreach (var expr in exprs)
        {
            sb.Append(' ');
            sb.Append(expr);
            
        }
        

    }
}