using NUnit.Framework;
using System;
using System.IO;

namespace LoxWithCSharp.Test
{
  internal class ParserTests : TestSetup
  {
    //[Test]
    //public void GenerateValidExpressions()
    //{
    //  List<Token> tokens = GetTokens("(5 - (3 - 1)) + -1");
    //  Assert.That(() => new AstPrinter().print(new Parser(tokens).parse()), Is.EqualTo("(+ (group (- 5 (group (- 3 1)))) (- 1))"));
    //}

    [Test]
    public void MissingClosingParanthesis()
    {
      var stringWriter = new StringWriter();
      Console.SetOut(stringWriter);
      GetExpressions("(10 + \"string\" + 15");
      Assert.That(() => stringWriter.ToString(), Contains.Substring("Error at end: Expect ')' after expression"));
    }

    //[Test]

    //public void EvaluateEqualityOperator()
    //{
    //  var stringWriter = new StringWriter();
    //  Console.SetOut(stringWriter);
    //  var tokens = GetTokens("5 == 5");
    //  Assert.That(() => new AstPrinter().print(new Parser(tokens).parse()), Is.EqualTo("(== 5 5)"));
    //}
  }
}
