using NUnit.Framework;
using System;
using System.IO;
using static LoxWithCSharp.Token;

namespace LoxWithCSharp.Test
{
  internal class ScannerTests : TestSetup
  {
    [Test]
    public void GenerateValidTokens()
    {
      var tokens = GetTokens("var a = 10;");
      Assert.That(() => tokens.Count, Is.EqualTo(6));
      Assert.That(() => tokens[0].lexeme, Is.EqualTo("var"));
      Assert.That(() => tokens[1].lexeme, Is.EqualTo("a"));
      Assert.That(() => tokens[2].lexeme, Is.EqualTo("="));
      Assert.That(() => tokens[3].lexeme, Is.EqualTo("10"));
      Assert.That(() => tokens[4].lexeme, Is.EqualTo(";"));
    }

    [Test]
    public void InvalidCharacterShouldLogError()
    {
      var stringWriter = new StringWriter();
      Console.SetOut(stringWriter);
      GetTokens("var @#$%^^&");
      Assert.That(stringWriter.ToString(), Contains.Substring("Error: Unexpected character: @"));
    }

    [Test]
    public void ValidateTokenTypes()
    {
      var tokens = GetTokens("25 hallo25 = == var * \"string\" ? text : 12");
      Assert.That(() => tokens[0].type, Is.EqualTo(TokenType.Number));
      Assert.That(() => tokens[1].type, Is.EqualTo(TokenType.Identifier));
      Assert.That(() => tokens[2].type, Is.EqualTo(TokenType.Equals));
      Assert.That(() => tokens[3].type, Is.EqualTo(TokenType.EqualsEquals));
      Assert.That(() => tokens[4].type, Is.EqualTo(TokenType.Var));
      Assert.That(() => tokens[5].type, Is.EqualTo(TokenType.Asterisk));
      Assert.That(() => tokens[6].type, Is.EqualTo(TokenType.String));
      Assert.That(() => tokens[7].type, Is.EqualTo(TokenType.TernaryQuestion));
    }
  }
}
