using LoxWithCSharp.Exception;

namespace LoxWithCSharp
{
  public class Lox
  {
    private static readonly Interpreter Interpreter = new();
    private static bool _hadError;
    private static bool _hadRuntimeError;

    public static void Main(string[] args)
    {
      switch (args.Length)
      {
      case > 1:
        Console.WriteLine("Usage: Lox [script]");
        System.Environment.Exit(64);
        break;
      case 1:
        RunFile(args[0]);
        break;
      default:
        RunPrompt();
        break;
      }
    }

    private static void RunFile(string path)
    {
      if (!File.Exists(path))
        return;
      var sourceFile = File.ReadAllText(path);
      Run(sourceFile);
      if (_hadError)
        System.Environment.Exit(65);
      if (_hadRuntimeError)
        System.Environment.Exit(70);
      System.Environment.Exit(0);
    }

    private static void RunPrompt()
    {
      for (;;)
      {
        Console.Write("> ");
        var line = Console.ReadLine();
        if (line == null)
          break;
        Run(line);
        _hadError = false;
        _hadRuntimeError = false;
      }
    }

    private static void Run(string source)
    {
      var scanner = new Scanner(source);
      var tokens = scanner.ScanTokens();
      var parser = new Parser(tokens);
      var statements = parser.Parse();

      // Stop if there was a syntax error.
      if (_hadError)
        return;
      Interpreter.Interpret(statements);
    }

    public static void Error(int line, string message) => Report(line, -1, "", message);
    public static void Error(int line, int character, string message) => Report(line, character, "", message);

    private static void Report(int line, int character, string where, string message)
    {
      var build = "[line " + line;
      if (character != -1)
        build += ":" + character;
      build += "] Error" + where + ": " + message;
      Console.WriteLine(build);
      _hadError = true;
    }

    public static void Error(Token token, string message)
    {
      if (token.type == Token.TokenType.Eof)
        Report(token.line, -1, " at end", message);
      else
        Report(token.line, token.characters, " at '" + token.lexeme + "'", message);
    }

    public static void RuntimeError(RuntimeError error)
    {
      var err = error.Message;
      if (error.token.line != -1)
      {
        err += "\n[line " + error.token.line;
        if (error.token.characters != -1)
          err += ":" + error.token.characters;
        err += "]";
      }
      Console.WriteLine(err);
      _hadRuntimeError = true;
    }
  }
}