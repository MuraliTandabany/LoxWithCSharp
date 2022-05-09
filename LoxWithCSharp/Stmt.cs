namespace LoxWithCSharp;

public abstract class Stmt
{
  public abstract T Accept<T>(IVisitor<T> visitor);

  public class Expression : Stmt
  {
    public readonly Expr expression;
    public Expression(Expr expression) => this.expression = expression;
    public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitExprStatement(this);
  }

  public class Print : Stmt
  {
    public readonly Expr expression;
    public Print(Expr expression) => this.expression = expression;
    public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitPrintStatement(this);
  }

  public class Var : Stmt
  {
    public readonly Token name;
    public readonly Expr? initializer;

    public Var(Token name, Expr? initializer)
    {
      this.name = name;
      if (initializer != null) this.initializer = initializer;
    }

    public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitVarStatement(this);
  }

  public class Block : Stmt
  {
    public readonly List<Stmt> statements;
    public Block(List<Stmt> statements)
    {
      this.statements = statements;
    }
    public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitBlockStatement(this);
  }

  public interface IVisitor<out T>
  {
    T VisitPrintStatement(Print printStmt);
    T VisitExprStatement(Expression exprStmt);
    T VisitVarStatement(Var varStmt);
    T VisitBlockStatement(Block blockStatement);
  }
}