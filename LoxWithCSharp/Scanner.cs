using static LoxWithCSharp.Token;

namespace LoxWithCSharp;

public class Scanner
{
  private readonly string _source;
  private readonly List<Token> _tokens = new();
  private static readonly Dictionary<string, TokenType> Keywords =
    new()
    {
      {"and", TokenType.And},
      {"break", TokenType.Break},
      {"continue", TokenType.Continue},
      {"class", TokenType.Class},
      {"do", TokenType.Do},
      {"else", TokenType.Else},
      {"false", TokenType.False},
      {"for", TokenType.For},
      {"fun", TokenType.Func},
      {"if", TokenType.If},
      {"nil", TokenType.Nil},
      {"or", TokenType.Or},
      {"print", TokenType.Print},
      {"return", TokenType.Return},
      {"super", TokenType.SuperClass},
      {"this", TokenType.ThisObject},
      {"true", TokenType.True},
      {"var", TokenType.Var},
      {"while", TokenType.While}
    };
  private int _start;
  private int _current;
  private int _line = 1;
  private int _lineCharCounter;
  public Scanner(string source) => this._source = source;

  public List<Token> ScanTokens()
  {
    while (!IsAtEnd())
    {
      _start = _current;
      ScanToken();
    }
    _tokens.Add(new Token(TokenType.Eof, "", null, _line, _lineCharCounter));
    return _tokens;
  }

  private bool IsAtEnd() => _current >= _source.Length;

  private void ScanToken()
  {
    var c = Advance();
    switch (c)
    {
    case '(':
      AddToken(TokenType.LeftParen);
      break;
    case ')':
      AddToken(TokenType.RightParen);
      break;
    case '{':
      AddToken(TokenType.LeftBrace);
      break;
    case '}':
      AddToken(TokenType.RightBrace);
      break;
    case ',':
      AddToken(TokenType.Comma);
      break;
    case '.':
      AddToken(Match('.')
        ? TokenType.DotDot
        : TokenType.Dot);
      break;
    case '-':
      AddToken(Match('-')
        ? TokenType.MinusMinus
        : TokenType.Minus);
      break;
    case '+':
      AddToken(Match('+')
        ? TokenType.PlusPlus
        : TokenType.Plus);
      break;
    case ';':
      AddToken(TokenType.Semicolon);
      break;
    case '*':
      AddToken(TokenType.Asterisk);
      break;
    case '!':
      AddToken(Match('=')
        ? TokenType.ExclamationEquals
        : TokenType.Exclamation);
      break;
    case '=':
      AddToken(Match('=')
        ? TokenType.EqualsEquals
        : TokenType.Equals);
      break;
    case '<':
      AddToken(Match('=')
        ? TokenType.LessThanEquals
        : TokenType.LessThan);
      break;
    case '>':
      AddToken(Match('=')
        ? TokenType.GreaterThanEquals
        : TokenType.GreaterThan);
      break;
    case '?':
      AddToken(Match('?')
        ? TokenType.QuestionQuestion
        : Match('.')
          ? TokenType.QuestionDot
          : TokenType.TernaryQuestion);
      break;
    case ':':
      AddToken(TokenType.TernaryColon);
      break;
    case '/':
      if (Match('/'))
      { //Comments
        while (Peek() != '\n' && !IsAtEnd())
          Advance();
        _lineCharCounter = 0; // Reset at the end of lines.
      }
      else if (Match('*'))
      {
        do
        {
          if (Peek() == '\n')
          {
            _lineCharCounter = 0;
            _line++;
          }
          else if (Peek() == '*' && PeekNext() == '/')
          {
            Advance();
            Advance();
            break;
          }
          Advance();
        } while (!IsAtEnd());
      }
      else
      {
        AddToken(TokenType.ForwardSlash);
      }
      break;
    case ' ':
    case '\r':
    case '\t':
      break; //Whitespace characters
    case '\n': //Newline character
      _line++;
      _lineCharCounter = 0;
      break;
    case '"':
      StringScanner();
      break;
    case 'o': //'or' but not 'orchid'
      if (Peek() == 'r')
      {
        if (PeekNext() == ' ' || PeekNext() == '\t' || PeekNext() == '\r' ||
            PeekNext() == '\n')
        {
          AddToken(TokenType.Or);
          Advance();
        }
        else
        {
          _current--;
          Identifier();
        }
      }
      break;
    default:
      if (IsDigit(c))
        Number();
      else if (IsAlpha(c))
        Identifier();
      else
        Lox.Error(_line, _lineCharCounter, "Unexpected character: " + c);
      break;
    }
  }

  private void Identifier()
  {
    while (IsAlphaNumeric(Peek()))
      Advance();
    var text = _source.Substring(_start, _current - _start);
    Keywords.TryGetValue(text, out var type);
    if (type == 0) //0 = TokenType.LEFT_PAREN, which would not reach this.
      type = TokenType.Identifier;
    AddToken(type);
  }

  private static bool IsAlpha(char c) => c is >= 'a' and <= 'z' or >= 'A' and <= 'Z' or '_';
  private static bool IsAlphaNumeric(char c) => IsAlpha(c) || IsDigit(c);

  private void Number()
  {
    while (IsDigit(Peek()))
      Advance();
    if (Peek() == '.' && IsDigit(PeekNext()))
    {
      Advance(); //Consume the .
      while (IsDigit(Peek()))
        Advance();
    }
    AddToken(TokenType.Number, double.Parse(_source.Substring(_start, _current - _start)));
  }

  private static bool IsDigit(char c) => c is >= '0' and <= '9';

  private void StringScanner()
  {
    while
      (Peek() != '"' && !IsAtEnd()) //Checks if "" is given, in which case we do nothing.
    {
      if (Peek() == '\n')
        _line++;
      Advance(); //Advance until next quotation mark
    }
    if (IsAtEnd()) //Never got to the next quotation mark
      // Lox.error(line, "Unterminated string.");
      return;
    Advance(); //Last "

    //Trim quotes, add token
    var value = _source.Substring(_start + 1, _current - _start - 2);
    AddToken(TokenType.String, value);
  }

  private char Peek() =>
    IsAtEnd()
      ? '\0'
      : _source.ToCharArray()[_current];

  private char PeekNext() =>
    _current + 1 >= _source.Length
      ? '\0'
      : _source.ToCharArray()[_current + 1];

  private bool Match(char expected) //Conditional advance()
  {
    if (IsAtEnd())
      return false;
    if (_source.ToCharArray()[_current] != expected)
      return false;
    _lineCharCounter++;
    _current++;
    return true;
  }

  private char Advance()
  {
    _lineCharCounter++;
    _current++;
    var c = _source.ToCharArray();
    return c[_current - 1];
  }

  private void AddToken(TokenType type) => AddToken(type, null);

  private void AddToken(TokenType type, object? literal)
  {
    var text = _source.Substring(_start, _current - _start);
    _tokens.Add(new Token(type, text, literal, _line, _lineCharCounter));
  }
}