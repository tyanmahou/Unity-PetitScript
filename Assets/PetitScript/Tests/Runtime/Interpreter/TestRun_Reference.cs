using NUnit.Framework;

namespace Petit.Runtime
{
    class TestRun_Reference : TestRunBase
    {
        [Test]
        public void TestReference()
        {
            string code = @"
a=1;b=a;a=2;
c=3;d=&c;c=4;
";
            Interpreter interpreter = new Interpreter();
            interpreter.Run(code);

            Assert.AreEqual(interpreter.Environment["a"], 2);
            Assert.True(interpreter.Environment["a"].IsInt);
            Assert.AreEqual(interpreter.Environment["b"], 1);
            Assert.True(interpreter.Environment["b"].IsInt);

            Assert.AreEqual(interpreter.Environment["c"], 4);
            Assert.True(interpreter.Environment["c"].IsInt);
            Assert.AreEqual(interpreter.Environment["d"], 4);
            Assert.True(interpreter.Environment["d"].IsReference);
        }

        [Test]
        public void TestFunctionReferenceArg()
        {
            string code = @"
fn swap(a, b)
{
  tmp = a;
  a = b;
  b = tmp;
}
x=-1;
y=1;
swap(x, y);

u=-1;
v=1;
swap(&u, &v);
";
            Interpreter interpreter = new Interpreter();
            interpreter.Run(code);

            Assert.AreEqual(interpreter.Environment["x"], -1);
            Assert.AreEqual(interpreter.Environment["y"], 1);
            Assert.AreEqual(interpreter.Environment["u"], 1);
            Assert.AreEqual(interpreter.Environment["v"], -1);
        }
    }
}
