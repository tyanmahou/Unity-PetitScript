using NUnit.Framework;

namespace Petit.Runtime
{
    class TestInterpreter_Int : TestInterpreterBase
    {
        [Test]
        public void TestInt()
        {
            RunInt("1234567890", 1234567890);
        }
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
    }
}
