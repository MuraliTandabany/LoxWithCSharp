using System.Text;

namespace LoxWithCSharp
{
  public class AstPrinter : IVisitor<string>
  {
    public string Print(Expr expr) => expr.Accept(this);

    public string VisitAssignExpr(Expr.AssignExpr assignExpr) =>
      Parenthesize("= " + assignExpr.name.lexeme, new[] {assignExpr.value});

    public string VisitBinaryExpr(Expr.BinaryExpr expr) =>
      Parenthesize(expr.operatorToken.lexeme, new[] {expr.left, expr.right});

    public string VisitGroupingExpr(Expr.Grouping expr) =>
      Parenthesize("group", new[] {expr.expression});

    public string VisitLiteralExpr(Expr.Literal expr)
    {
      if (expr.literal == null)
        return "nil";
      return expr.literal.ToString() ?? "nil";
    }

    public string VisitUnaryExpr(Expr.UnaryExpr expr) =>
      Parenthesize(expr.operatorToken.lexeme, new[] {expr.right});

    public string VisitVariable(Expr.Variable variable) =>
      Parenthesize(variable.name.lexeme, new Expr[] { });

    private string Parenthesize(string name, Expr[] expressions)
    {
      var builder = new StringBuilder();
      builder.Append("(").Append(name);
      foreach (var expr in expressions)
      {
        builder.Append(" ");
        builder.Append(expr.Accept(this));
      }
      builder.Append(")");
      return builder.ToString();
    }
  }
}