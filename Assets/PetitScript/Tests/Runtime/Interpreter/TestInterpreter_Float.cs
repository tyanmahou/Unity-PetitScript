using NUnit.Framework;

namespace Petit.Runtime
{
    class TestInterpreter_Float : TestInterpreterBase
    {
        [Test]
        public void TestFloat()
        {
            RunFloat("123456.7890", 123456.7890f);
        }
        [Test]
        public void TestCalcFloat()
        {
            RunFloat("1.0 + 0.5", 1.5f);
            RunFloat("1.5 - 2", -0.5f);
            RunFloat("1.5 * 2", 3.0f);
            RunFloat("1.0 / 2", 0.5f);
            RunFloat("1.0 % 0.5", 0.0f);
            RunFloat("1.0 % 0.7", 0.3f);
        }
    }
}
