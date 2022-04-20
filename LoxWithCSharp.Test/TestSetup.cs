using System.Collections.Generic;

namespace LoxWithCSharp.Test;

internal class TestSetup
{
  public static List<Token> GetTokens(string source) => new Scanner(source).ScanTokens();
  public static List<Stmt> GetExpressions(string source) => new Parser(new Scanner(source).ScanTokens()).Parse();
}