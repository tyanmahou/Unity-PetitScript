using NUnit;
using NUnit.Framework;

namespace Petit.Runtime
{
    class TestRun_If : TestRunBase
    {
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
                var env = Environment.New;
                env["a"] = 1;
                RunString(code, "plus", env);
            }
            {
                var env = Environment.New;
                env["a"] = 0;
                RunString(code, "zero", env);
            }
            {
                var env = Environment.New;
                env["a"] = -1;
                RunString(code, "minus", env);
            }
        }
    }
}
