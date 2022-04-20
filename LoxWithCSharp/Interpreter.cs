using System.Globalization;
using LoxWithCSharp.Exception;
using static LoxWithCSharp.Token;

namespace LoxWithCSharp;

public class Interpreter : IVisitor<object>, Stmt.IVisitor<object>
{
  private readonly Environment _environment = new();

  public void Interpret(List<Stmt> statements)
  {
    try
    {
      foreach (var statement in statements)
        Execute(statement);
    }
    catch (RuntimeError error)
    {
      Lox.RuntimeError(error);
    }
  }

  private static string Stringify(object @object)
  {
    switch (@object)
    {
    case null or Null:
      return "nil";
    case double:
    {
      var text = @object.ToString()!;
      if (text.EndsWith(".0", StringComparison.Ordinal))
        text = text[..^2];
      return text;
    }
    default:
      return @object.ToString()!;
    }
  }

  public object VisitBinaryExpr(Expr.BinaryExpr expr)
  {
    var left = Evaluate(expr.left);
    var right = Evaluate(expr.right);
    switch (expr.operatorToken.type)
    {
    case TokenType.GreaterThan:
      CheckNumberOperand(expr.operatorToken, left, right);
      return (double) left > (double) right;
    case TokenType.GreaterThanEquals:
      CheckNumberOperand(expr.operatorToken, left, right);
      return (double) left >= (double) right;
    case TokenType.LessThan:
      CheckNumberOperand(expr.operatorToken, left, right);
      return (double) left < (double) right;
    case TokenType.LessThanEquals:
      CheckNumberOperand(expr.operatorToken, left, right);
      return (double) left <= (double) right;
    case TokenType.EqualsEquals:
      return IsEqual(left, right);
    case TokenType.ExclamationEquals:
      return !IsEqual(left, right);
    case TokenType.Minus:
      CheckNumberOperand(expr.operatorToken, left, right);
      return (double) left - (double) right;
    case TokenType.Plus:
      return left switch
      {
        double d when right is double right1 => d + right1,
        string s when right is string right1 => s + right1,
        double d when right is string s => d.ToString(CultureInfo.InvariantCulture) + s,
        string s when right is double d => s + d.ToString(CultureInfo.InvariantCulture),
        _ => throw new RuntimeError(expr.operatorToken,
          "Operands must be numbers or strings.")
      };
    case TokenType.ForwardSlash:
      CheckNumberOperand(expr.operatorToken, left, right);
      return (double) left / (double) right;
    case TokenType.Asterisk:
      CheckNumberOperand(expr.operatorToken, left, right);
      return (double) left * (double) right;
    }
    // Unreachable.
    return new object();
  }

  public object VisitGroupingExpr(Expr.Grouping expr) => Evaluate(expr.expression);
  public object VisitLiteralExpr(Expr.Literal expr) => expr.literal ?? new object();

  public object VisitUnaryExpr(Expr.UnaryExpr expr)
  {
    var right = Evaluate(expr.right);
    switch (expr.operatorToken.type)
    {
    case TokenType.Exclamation:
      return !IsTruthy(right);
    case TokenType.Minus:
      CheckNumberOperand(expr.operatorToken, right);
      return -(double) right;
    }
    // Unreachable.
    return null!;
  }

  public object VisitVariable(Expr.Variable expr) => _environment.Get(expr.name);
  private object Evaluate(Expr expr) => expr.Accept(this);
  private void Execute(Stmt stmt) => stmt.Accept(this);

  private static bool IsTruthy(object objectA) =>
    objectA switch
    {
      bool a => a,
      _ => true
    };

  private static bool IsEqual(object a, object b) =>
    a switch
    {
      Null when b is Null => true,
      Null => false,
      _ => a.Equals(b)
    };

  private static void CheckNumberOperand(Token @operator, object operand)
  {
    if (operand is double)
      return;
    throw new RuntimeError(@operator, "Operand must be a number.");
  }

  private static void CheckNumberOperand(Token @operator, object operand, object operandTwo)
  {
    if (operand is double && operandTwo is double)
      return;
    throw new RuntimeError(@operator, "Operand must be a number.");
  }

  public object VisitPrintStatement(Stmt.Print printStmt)
  {
    var value = Evaluate(printStmt.expression);
    Console.WriteLine(Stringify(value));
    return new object();
  }

  public object VisitVarStatement(Stmt.Var stmt)
  {
    var value = new object();
    if (stmt.initializer != null)
      value = Evaluate(stmt.initializer);
    _environment.Define(stmt.name.lexeme, value);
    return new object();
  }

  public object VisitAssignExpr(Expr.AssignExpr expr)
  {
    var value = Evaluate(expr.value);
    _environment.Assign(expr.name, value);
    return value;
  }

  public object VisitExprStatement(Stmt.Expression exprStmt)
  {
    Evaluate(exprStmt.expression);
    return new object();
  }
}

internal class Null { }