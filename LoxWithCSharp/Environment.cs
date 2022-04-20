using LoxWithCSharp.Exception;

namespace LoxWithCSharp;

public class Environment
{
  private readonly Dictionary<string, object> _values = new();

  public object Get(Token name)
  {
    if (_values.ContainsKey(name.lexeme))
      return _values[name.lexeme];
    throw new RuntimeError(name, "Undefined variable '" + name.lexeme + "'.");
  }

  public void Assign(Token name, object value)
  {
    if (_values.ContainsKey(name.lexeme))
    {
      _values[name.lexeme] = value;
      return;
    }
    throw new RuntimeError(name, "Undefined variable '" + name.lexeme + "'.");
  }

  public void Define(string name, object value) => _values.Add(name, value);
}