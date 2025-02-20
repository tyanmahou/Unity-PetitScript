using NUnit.Framework;
using System;

namespace Petit.Runtime
{
    class FunctionTest
    {
        Value Add(Value a1, Value b1)
        {
            return a1 + b1;
        }
        [Test]
        public void TestNormal()
        {
            Function func = Function.Bind(Add);
            Assert.AreEqual(func.Parameters[0].Name, "a1");
            Assert.AreEqual(func.Parameters[1].Name, "b1");
        }
        [Test]
        public void TestLambda()
        {
            Function func = Function.Bind((a2, b2) =>
            {
                return a2 + b2;
            });
            Assert.AreEqual(func.Parameters[0].Name, "a2");
            Assert.AreEqual(func.Parameters[1].Name, "b2");
        }
    }
}
