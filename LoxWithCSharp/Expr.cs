namespace LoxWithCSharp;

public abstract class Expr
{
  public abstract T Accept<T>(IVisitor<T> visitor);

  public class BinaryExpr : Expr
  {
    public readonly Expr left;
    public readonly Token operatorToken;
    public readonly Expr right;

    public BinaryExpr(Expr left, Token operatorToken, Expr right) //operator is a keyword in C#
    {
      this.left = left;
      this.operatorToken = operatorToken;
      this.right = right;
    }

    public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitBinaryExpr(this);
  }

  public class UnaryExpr : Expr
  {
    public readonly Token operatorToken;
    public readonly Expr right;

    public UnaryExpr(Token operatorToken, Expr expression)
    {
      this.operatorToken = operatorToken;
      right = expression;
    }

    public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitUnaryExpr(this);
  }

  public class Grouping : Expr
  {
    public readonly Expr expression; // "(" expr ")"
    public Grouping(Expr expression) => this.expression = expression;
    public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitGroupingExpr(this);
  }

  public class Literal : Expr
  {
    public readonly object? literal;
    public readonly Token type;

    public Literal(object? value, Token t)
    {
      literal = value;
      type = t;
    }

    public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitLiteralExpr(this);
  }

  public class Variable : Expr
  {
    public readonly Token name;
    public Variable(Token name) => this.name = name;
    public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitVariable(this);
  }

  public class AssignExpr : Expr
  {
    public readonly Token name;
    public readonly Expr value;

    public AssignExpr(Token name, Expr value)
    {
      this.name = name;
      this.value = value;
    }

    public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitAssignExpr(this);
  }
}

public interface IVisitor<out T>
{
  T VisitBinaryExpr(Expr.BinaryExpr binaryExpr);
  T VisitUnaryExpr(Expr.UnaryExpr unaryExpr);
  T VisitGroupingExpr(Expr.Grouping grouping);
  T VisitLiteralExpr(Expr.Literal literal);
  T VisitVariable(Expr.Variable variable);
  T VisitAssignExpr(Expr.AssignExpr assignExpr);
}