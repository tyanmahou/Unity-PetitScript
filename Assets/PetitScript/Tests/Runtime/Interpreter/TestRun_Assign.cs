using NUnit.Framework;

namespace Petit.Runtime
{
    class TestRun_Assign : TestRunBase
    {
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
        public void TestCondAssign()
        {
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
        public void TestPrefixIncrement()
        {
            Interpreter interpreter = RunInt("a=1;++a;", 2);
            Assert.AreEqual(interpreter.Enviroment.Get("a"), 2);
        }
        [Test]
        public void TestPrefixDecrement()
        {
            Interpreter interpreter = RunInt("a=1;--a;", 0);
            Assert.AreEqual(interpreter.Enviroment.Get("a"), 0);
        }
        [Test]
        public void TestPostfixIncrement()
        {
            Interpreter interpreter = RunInt("a=1;a++;", 1);
            Assert.AreEqual(interpreter.Enviroment.Get("a"), 2);
        }
        [Test]
        public void TestPostfixDecrement()
        {
            Interpreter interpreter = RunInt("a=1;a--;", 1);
            Assert.AreEqual(interpreter.Enviroment.Get("a"), 0);
        }
    }
}
