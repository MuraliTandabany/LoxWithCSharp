using NUnit.Framework;
using System;
using System.IO;

namespace LoxWithCSharp.Test
{
  internal class InterpreterTests : TestSetup
  {
    [Test]
    public void EvaluateExpression()
    {
      var stringWriter = new StringWriter();
      Console.SetOut(stringWriter);
      var expr = GetExpressions("print (5 - (3 - 1)) + -1;");
      new Interpreter().Interpret(expr);
      Assert.That(stringWriter.ToString(), Contains.Substring("2"));
    }

    [Test]
    public void NumberStringConcatenation()
    {
      var stringWriter = new StringWriter();
      Console.SetOut(stringWriter);
      var expr = GetExpressions("print 10 + \"string\" + 15;");
      new Interpreter().Interpret(expr);
      Assert.That(() => stringWriter.ToString(), Contains.Substring("10string15"));
    }

    [Test]
    public void NegateOperator()
    {
      var stringWriter = new StringWriter();
      Console.SetOut(stringWriter);
      var expr = GetExpressions("print !true;");
      new Interpreter().Interpret(expr);
      Assert.That(() => stringWriter.ToString(), Contains.Substring("False"));
    }

    [Test]
    public void EvaluateEqualityOperator()
    {
      var stringWriter = new StringWriter();
      Console.SetOut(stringWriter);
      var expr = GetExpressions("print 5 == 5;");
      new Interpreter().Interpret(expr);
      Assert.That(() => stringWriter.ToString(), Contains.Substring("True"));
    }

    [Test]
    public void EvaluateGreaterThanOperator()
    {
      var stringWriter = new StringWriter();
      Console.SetOut(stringWriter);
      var expr = GetExpressions("print 5 >= 5;");
      new Interpreter().Interpret(expr);
      Assert.That(() => stringWriter.ToString(), Contains.Substring("True"));
    }
  }
}
