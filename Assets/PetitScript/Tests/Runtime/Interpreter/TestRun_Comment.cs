using NUnit.Framework;

namespace Petit.Runtime
{
    class TestRun_Comment : TestRunBase
    {
        [Test]
        public void TestComment()
        {
            var result = Interpreter.RunScript(@"
a = 1; // a = 2;
");
            Assert.True(result.IsInt);
            Assert.AreEqual(result.ToInt(), 1);
        }
        [Test]
        public void TestBlockComment()
        {
            var result = Interpreter.RunScript(@"
a = 1; /* a = 2;
a = 3; */
");
            Assert.True(result.IsInt);
            Assert.AreEqual(result.ToInt(), 1);
        }
    }
}
