using LoxWithCSharp.Exception;

namespace LoxWithCSharp;

public class Environment
{
  private readonly Environment? _enclosing;
  private readonly Dictionary<string, object> _values = new();

  public Environment()
  {
    this._enclosing = null;
  }

  public Environment(Environment enclosing)
  {
    this._enclosing = enclosing;
  }

  public object Get(Token name)
  {
    if (_values.ContainsKey(name.lexeme)) return _values[name.lexeme];
    if (_enclosing != null) return _enclosing.Get(name);
    throw new RuntimeError(name, "Undefined variable '" + name.lexeme + "'.");
  }

  public void Assign(Token name, object value)
  {
    if (_values.ContainsKey(name.lexeme))
    {
      _values[name.lexeme] = value;
      return;
    }

    if (_enclosing != null)
    {
      _enclosing.Assign(name, value);
      return;
    }

    throw new RuntimeError(name, "Undefined variable '" + name.lexeme + "'.");
  }

  public void Define(string name, object value, Token? token = null)
  {
    if (!_values.ContainsKey(name))
    {
      _values.Add(name, value);
      return;
    }

    if (token == null)
      throw new RuntimeError(new Token(Token.TokenType.Var, name, value, -1, -1),
        "The var " + name + " has already been defined.");
    throw new RuntimeError(token, "The var " + name + " has already been defined.");
  }
}