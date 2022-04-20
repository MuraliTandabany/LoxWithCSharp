namespace LoxWithCSharp;

public class Token
{
  public readonly TokenType type;
  public readonly string lexeme;
  public readonly object? literal;
  public readonly int line;
  public readonly int characters;

  public Token(TokenType type, string lexeme, object? literal, int line,
    int characters)
  {
    this.type = type;
    this.lexeme = lexeme;
    this.literal = literal;
    this.line = line;
    this.characters = characters;
  }

  public static List<Token> Tokenize(string input)
  {
    var scanner = new Scanner(input);
    return scanner.ScanTokens();
  }

  public enum TokenType
  {
    //1 char tokens
    LeftParen,
    RightParen,
    LeftBrace,
    RightBrace,
    Comma,
    Dot,
    Minus,
    Plus,
    ForwardSlash,
    Asterisk,
    Semicolon,

    //1-2 char tokens
    Exclamation,
    ExclamationEquals,
    Equals,
    EqualsEquals,
    GreaterThan,
    GreaterThanEquals,
    LessThan,
    LessThanEquals,
    TernaryQuestion,
    TernaryColon,
    PlusPlus,
    MinusMinus,
    QuestionQuestion,
    QuestionDot,
    DotDot,

    //Literals, arbitrary char length
    Identifier,
    String,
    Number,

    //Keywords
    And,
    Or,
    Class,
    If,
    Else,
    Func,
    For,
    Nil,
    False,
    Print,
    Return,
    SuperClass,
    ThisObject,
    True,
    Var,
    While,
    Break,
    Continue,
    Do,
    Eof
  }
}