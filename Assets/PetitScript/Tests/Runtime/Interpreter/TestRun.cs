using NUnit.Framework;

namespace Petit.Runtime
{
    class TestRun : TestRunBase
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
            RunString("\"a\" || true", "a");
        }
        [Test]
        public void TestCond()
        {
            RunInt("true ? 1 : 2", 1);
            RunInt("false ? 1 : 2", 2);
        }
    }
}
