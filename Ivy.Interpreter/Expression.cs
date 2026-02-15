namespace Ivy.Interpreter;

public abstract class Expr
{
    public abstract T Accept<T>(AstVisitor<T> astVisitor);
};

public interface AstVisitor<T>
{
    T Visit(Binary expr);
    T Visit(Literal expr);
    T Visit(Grouping expr);
    T Visit(Unary expr);
}

public class Literal(Token value) : Expr
{
    public override T Accept<T>(AstVisitor<T> astVisitor) => astVisitor.Visit(this);
}

public class Binary(Expr Left, Token Operator, Expr Right) : Expr
{
    public override T Accept<T>(AstVisitor<T> astVisitor) => astVisitor.Visit(this);
}

public class Unary(Token Operator, Expr Right) : Expr
{
   public override T Accept<T>(AstVisitor<T> astVisitor) => astVisitor.Visit(this);
}

public class Grouping(Token Operator, Expr Right) : Expr
{
    public override T Accept<T>(AstVisitor<T> astVisitor) => astVisitor.Visit(this);
}

