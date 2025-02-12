using NUnit.Framework;

namespace Petit.Core
{
    class InterpreterTest
    {
        [Test]
        public void TestCalcInt()
        {
            RunInt("1 + 2", 3);
            RunInt("1 - 2", -1);
            RunInt("1 * 2", 2);
            RunInt("1 / 2", 0);
            RunInt("49 / 7", 7);
            RunInt("7 % 5", 2);
            RunInt("(12 + 3) * 3 / 4 - 20 % 18", 9);
        }
        [Test]
        public void TestCalcFloatt()
        {
            RunFloat("1.0 + 0.5", 1.5f);
            RunFloat("1.5 - 2", -0.5f);
            RunFloat("1.5 * 2", 3.0f);
            RunFloat("1.0 / 2", 0.5f);
            RunFloat("1.0 % 0.5", 0.0f);
            RunFloat("1.0 % 0.7", 0.3f);
        }
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
                Assert.AreEqual(interpreter.Variables.Get("a"), 10);
            }
            {
                Interpreter interpreter = RunInt("a = b = 10", 10);
                Assert.AreEqual(interpreter.Variables.Get("a"), 10);
                Assert.AreEqual(interpreter.Variables.Get("b"), 10);
            }
            {
                Interpreter interpreter = RunInt("a = b = 1 + 2 * 3", 7);
                Assert.AreEqual(interpreter.Variables.Get("a"), 7);
                Assert.AreEqual(interpreter.Variables.Get("b"), 7);
            }
            {
                Interpreter interpreter = RunInt("a=10; a+=2", 12);
                Assert.AreEqual(interpreter.Variables.Get("a"), 12);
            }
            {
                Interpreter interpreter = RunInt("a=10; b=2; b+=a+a;", 22);
                Assert.AreEqual(interpreter.Variables.Get("a"), 10);
                Assert.AreEqual(interpreter.Variables.Get("b"), 22);
            }
        }
        [Test]
        public void TestCond()
        {
            RunInt("true ? 1 : 2", 1);
            RunInt("false ? 1 : 2", 2);
            {
                Interpreter interpreter = RunInt("(true ? a : b) = 10", 10);
                Assert.AreEqual(interpreter.Variables.Get("a"), 10);
                Assert.AreEqual(interpreter.Variables.Get("b"), Value.Invalid);
            }
            {
                Interpreter interpreter = RunInt("(false ? a : b) = 10", 10);
                Assert.AreEqual(interpreter.Variables.Get("a"), Value.Invalid);
                Assert.AreEqual(interpreter.Variables.Get("b"), 10);
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
                var vars = new Variables();
                vars.Set("a", 1);
                RunString(code, "plus", vars);
            }
            {
                var vars = new Variables();
                vars.Set("a", 0);
                RunString(code, "zero", vars);
            }
            {
                var vars = new Variables();
                vars.Set("a", -1);
                RunString(code, "minus", vars);
            }
        }
        Interpreter RunInt(string code, int actual)
        {
            Interpreter interpreter = new Interpreter();
            var result = interpreter.Run(code);
            Assert.True(result.IsInt);
            Assert.AreEqual(result.ToInt(), actual);
            return interpreter;
        }
        Interpreter RunFloat(string code, float actual)
        {
            Interpreter interpreter = new Interpreter();
            var result = interpreter.Run(code);
            Assert.True(result.IsFloat);
            Assert.AreEqual(result.ToFloat(), actual);
            return interpreter;
        }
        Interpreter RunBool(string code, bool actual)
        {
            Interpreter interpreter = new Interpreter();
            var result = interpreter.Run(code);
            Assert.True(result.IsBool);
            Assert.AreEqual(result.ToBool(), actual);
            return interpreter;
        }
        Interpreter RunString(string code, string actual, Variables vars = null)
        {
            Interpreter interpreter = new Interpreter();
            if (vars != null)
            {
                interpreter.Variables = vars;
            }
            var result = interpreter.Run(code);
            Assert.True(result.IsString);
            Assert.AreEqual(result.ToString(), actual);
            return interpreter;
        }
    }
}
