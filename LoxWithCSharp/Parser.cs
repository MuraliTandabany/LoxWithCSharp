using static LoxWithCSharp.Token;

namespace LoxWithCSharp;

public class Parser
{
  private sealed class ParseError : System.Exception
  {
  }

  private readonly List<Token> _tokens;
  private int _current;
  public Parser(List<Token> tokens) => this._tokens = tokens;

  public List<Stmt> Parse()
  {
    var statements = new List<Stmt>();
    while (!IsAtEnd())
    {
      var stmt = Declaration();
      if (stmt != null) statements.Add(stmt);
    }

    return statements;
  }

  private Stmt? Declaration()
  {
    try
    {
      return Match(TokenType.Var) ? VarDeclaration() : Statement();
    }
    catch (ParseError)
    {
      Synchronize();
      return null;
    }
  }

  private Expr Expression() => Assignment();

  private Stmt Statement() =>
    Match(TokenType.Print) ? PrintStatement() :
    Match(TokenType.LeftBrace) ? new Stmt.Block(Block()) : ExpressionStatement();

  private Stmt PrintStatement()
  {
    var value = Expression();
    Consume(TokenType.Semicolon, "Expect ';' after value.");
    return new Stmt.Print(value);
  }

  private Stmt ExpressionStatement()
  {
    var expr = Expression();
    Consume(TokenType.Semicolon, "Expect ';' after expression.");
    return new Stmt.Expression(expr);
  }

  private List<Stmt> Block()
  {
    List<Stmt> statements = new();

    while (!Check(TokenType.RightBrace) && !IsAtEnd())
    {
      var stmt = Declaration();
      if (stmt != null) statements.Add(stmt);
    }

    Consume(TokenType.RightBrace, "Expect '}' after block.");
    return statements;
  }

  private Expr Assignment()
  {
    var expr = Equality();
    if (Match(TokenType.Equals))
    {
      var equals = Previous();
      var value = Assignment();
      if (expr is Expr.Variable variable)
      {
        var name = variable.name;
        return new Expr.AssignExpr(name, value);
      }

      Error(equals, "Invalid assignment target.");
    }

    return expr;
  }

  private Stmt VarDeclaration()
  {
    var name = Consume(TokenType.Identifier, "Expect variable name.");
    Expr? initializer = null;
    if (Match(TokenType.Equals)) initializer = Expression();
    Consume(TokenType.Semicolon, "Expect ';' after variable declaration.");
    return new Stmt.Var(name, initializer);
  }

  private Expr Equality()
  {
    var expr = Comparison();
    while (Match(TokenType.ExclamationEquals, TokenType.EqualsEquals))
    {
      var @operator = Previous();
      var right = Comparison();
      expr = new Expr.BinaryExpr(expr, @operator, right);
    }

    return expr;
  }

  private bool Match(params TokenType[] types)
  {
    foreach (var type in types)
      if (Check(type))
      {
        Advance();
        return true;
      }

    return false;
  }

  private bool Check(TokenType type)
  {
    if (IsAtEnd()) return false;
    return Peek().type == type;
  }

  private Token Advance()
  {
    if (!IsAtEnd()) _current++;
    return Previous();
  }

  private bool IsAtEnd() => Peek().type == TokenType.Eof;
  private Token Peek() => _tokens.ElementAt(_current);
  private Token Previous() => _tokens.ElementAt(_current - 1);

  private Expr Comparison()
  {
    var expr = Term();
    while (Match(TokenType.GreaterThan, TokenType.GreaterThanEquals, TokenType.LessThan, TokenType.LessThanEquals))
    {
      var @operator = Previous();
      var right = Term();
      expr = new Expr.BinaryExpr(expr, @operator, right);
    }

    return expr;
  }

  private Expr Term()
  {
    var expr = Factor();
    while (Match(TokenType.Minus, TokenType.Plus))
    {
      var @operator = Previous();
      var right = Factor();
      expr = new Expr.BinaryExpr(expr, @operator, right);
    }

    return expr;
  }

  private Expr Factor()
  {
    var expr = Unary();
    while (Match(TokenType.ForwardSlash, TokenType.Asterisk))
    {
      var @operator = Previous();
      var right = Unary();
      expr = new Expr.BinaryExpr(expr, @operator, right);
    }

    return expr;
  }

  private Expr Unary()
  {
    if (Match(TokenType.Exclamation, TokenType.Minus))
    {
      var @operator = Previous();
      var right = Unary();
      return new Expr.UnaryExpr(@operator, right);
    }

    return Primary();
  }

  private Expr Primary()
  {
    if (Match(TokenType.False)) return new Expr.Literal(false, Previous());
    if (Match(TokenType.True)) return new Expr.Literal(true, Previous());
    if (Match(TokenType.Nil)) return new Expr.Literal(null, Previous());
    if (Match(TokenType.Number, TokenType.String)) return new Expr.Literal(Previous().literal, Previous());
    if (Match(TokenType.Identifier)) return new Expr.Variable(Previous());
    if (Match(TokenType.LeftParen))
    {
      var expr = Expression();
      Consume(TokenType.RightParen, "Expect ')' after expression.");
      return new Expr.Grouping(expr);
    }

    throw Error(Peek(), "Expect expression.");
  }

  private Token Consume(TokenType type, string message)
  {
    if (Check(type)) return Advance();
    throw Error(Peek(), message);
  }

  private static ParseError Error(Token token, string message)
  {
    Lox.Error(token, message);
    return new ParseError();
  }

  private void Synchronize()
  {
    Advance();
    while (!IsAtEnd())
    {
      if (Previous().type == TokenType.Semicolon) return;
      switch (Peek().type)
      {
        case TokenType.Class:
        case TokenType.Func:
        case TokenType.Var:
        case TokenType.For:
        case TokenType.If:
        case TokenType.While:
        case TokenType.Print:
        case TokenType.Return:
          return;
      }

      Advance();
    }
  }
}