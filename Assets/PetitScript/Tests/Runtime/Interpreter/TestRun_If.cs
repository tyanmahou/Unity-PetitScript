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
    }
}
