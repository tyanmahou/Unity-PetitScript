using NUnit.Framework;

namespace Petit.Runtime
{
    class TestRun_Float : TestRunBase
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
        [Test]
        public void TestZeroDivide()
        {
            //RunFloat("1.0 / 0.0", float.PositiveInfinity);
            //RunFloat("0.0 / 0.0", float.NaN);
            //RunFloat("-1.0 / 0.0", float.NegativeInfinity);
            //RunNaN("1.0 % 0.0");
        }
    }
}
