﻿using NUnit.Framework;

namespace Petit.Runtime
{
    class TestInterpreter : TestInterpreterBase
    {
        [Test]
        public void TestLogic()
        {
            RunBool("true", true);
            RunBool("false", false);
            RunBool("!true", false);
            RunBool("!false", true);
            RunBool("true && true", true);
            RunBool("true && false", false);
            RunBool("false && true", false);
            RunBool("false && false", false);
            RunBool("true || true", true);
            RunBool("true || false", true);
            RunBool("false || true", true);
            RunBool("false || false", false);

            RunBool("!false && !!true", true);

            RunInt("true && 1", 1);
            RunInt("true && 0", 0);
            RunBool("false && 1", false);
            RunBool("false && 0", false);
            RunString("true && \"a\"", "a");

            RunInt("1 || true", 1);
            RunInt("1 || false", 1);
            RunBool("0 || true", true);
            RunBool("false || false", false);
            RunString("\"a\" || truew", "a");
        }
        [Test]
        public void TestAssign()
        {
            {
                Interpreter interpreter = RunInt("a = 10", 10);
                Assert.AreEqual(interpreter.Enviroment.Get("a"), 10);
            }
            {
                Interpreter interpreter = RunInt("a = b = 10", 10);
                Assert.AreEqual(interpreter.Enviroment.Get("a"), 10);
                Assert.AreEqual(interpreter.Enviroment.Get("b"), 10);
            }
            {
                Interpreter interpreter = RunInt("a = b = 1 + 2 * 3", 7);
                Assert.AreEqual(interpreter.Enviroment.Get("a"), 7);
                Assert.AreEqual(interpreter.Enviroment.Get("b"), 7);
            }
            {
                Interpreter interpreter = RunInt("a=10; a+=2", 12);
                Assert.AreEqual(interpreter.Enviroment.Get("a"), 12);
            }
            {
                Interpreter interpreter = RunInt("a=10; b=2; b+=a+a;", 22);
                Assert.AreEqual(interpreter.Enviroment.Get("a"), 10);
                Assert.AreEqual(interpreter.Enviroment.Get("b"), 22);
            }
        }
        [Test]
        public void TestCond()
        {
            RunInt("true ? 1 : 2", 1);
            RunInt("false ? 1 : 2", 2);
            {
                Interpreter interpreter = RunInt("(true ? a : b) = 10", 10);
                Assert.AreEqual(interpreter.Enviroment.Get("a"), 10);
                Assert.AreEqual(interpreter.Enviroment.Get("b"), Value.Invalid);
            }
            {
                Interpreter interpreter = RunInt("(false ? a : b) = 10", 10);
                Assert.AreEqual(interpreter.Enviroment.Get("a"), Value.Invalid);
                Assert.AreEqual(interpreter.Enviroment.Get("b"), 10);
            }
        }
        [Test]
        public void TestIf()
        {
            string code = @"
if (a > 0)
{
   ""plus"";
}
else if (a == 0)
{
   ""zero"";
}
else
{
   ""minus"";
}
";
            {
                var vars = new Enviroment(null);
                vars.Set("a", 1);
                RunString(code, "plus", vars);
            }
            {
                var vars = new Enviroment(null);
                vars.Set("a", 0);
                RunString(code, "zero", vars);
            }
            {
                var vars = new Enviroment(null);
                vars.Set("a", -1);
                RunString(code, "minus", vars);
            }
        }
        [Test]
        public void TestSwitch()
        {
            string code = @"
text="""";
switch(a) {
case 0:
   text += ""0"";
case  1:
   text += ""1"";
case  2:
case  3:
   text += ""23"";
　break;    
default:
   text += ""d"";
case  4:
   text += ""4"";
   break;
}
return text;
";
            {
                var vars = new Enviroment(null);
                vars.Set("a", 0);
                RunString(code, "0123", vars);
            }
            {
                var vars = new Enviroment(null);
                vars.Set("a", 1);
                RunString(code, "123", vars);
            }
            {
                var vars = new Enviroment(null);
                vars.Set("a", 2);
                RunString(code, "23", vars);
            }
            {
                var vars = new Enviroment(null);
                vars.Set("a", 3);
                RunString(code, "23", vars);
            }
            {
                var vars = new Enviroment(null);
                vars.Set("a", 4);
                RunString(code, "4", vars);
            }
            {
                var vars = new Enviroment(null);
                vars.Set("a", 5);
                RunString(code, "d4", vars);
            }
        }
    }
}
