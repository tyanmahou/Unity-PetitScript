using NUnit.Framework;

namespace Petit.Runtime
{
    class TestInterpreter_String : TestInterpreterBase
    {
        [Test]
        public void TestStringLiteral()
        {
            string code = @"""abcde""";
            RunString(code, "abcde");
        }
        [Test]
        public void TestStringLiteralEmpty()
        {
            string code = @"""""";
            RunString(code, string.Empty);
        }
        [Test]
        public void TestStringInterpolation()
        {
            string code = @"""abc{1}def""";
            RunString(code, "abc1def");
        }
        [Test]
        public void TestStringNoneInterpolation()
        {
            {
                string code = @"""abc{{1}}def""";
                RunString(code, "abc{1}def");
            }
            {
                string code = @"""abc{{1}def""";
                RunString(code, "abc{1}def");
            }
        }
        [Test]
        public void TestStringInterpolationNest()
        {
            string code = @"""abc{""x{""y""}z""}def""";
            RunString(code, "abcxyzdef");
        }
    }
}
