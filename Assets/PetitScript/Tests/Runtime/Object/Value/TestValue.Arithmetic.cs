using NUnit.Framework;

namespace Petit.Runtime
{
    class TestValue_Arithmetic
    {
        [Test]
        public void TestUnaryPlus()
        {
            Assert.AreEqual(+Value.Of(true), 1);
            Assert.AreEqual(+Value.Of(false), 0);
            Assert.AreEqual(+Value.Of(1), 1);
            Assert.AreEqual(+Value.Of(1.0f), 1.0f);

            Assert.True((+Value.Of("a")).IsNaN);
            Assert.AreEqual(+Value.Of(""), 0);
            Assert.AreEqual(+Value.Of("1"), 1);
            Assert.AreEqual(+Value.Of("1.0"), 1.0f);
            Assert.AreEqual(+Value.Of("true"), 1);
            Assert.AreEqual(+Value.Of("false"), 0);


            Assert.AreEqual(+Value.ArrayOf(), 0);
            Assert.AreEqual(+Value.ArrayOf(1), 1);
            Assert.AreEqual(+Value.ArrayOf("1"), 1);
            Assert.True((+Value.ArrayOf(1, 2)).IsNaN);

            Assert.True((+Value.Of(Function.Bind(() => {}))).IsNaN);
        }
        [Test]
        public void TestUnaryMinus()
        {
            Assert.AreEqual(-Value.Of(true), -1);
            Assert.AreEqual(-Value.Of(false), 0);
            Assert.AreEqual(-Value.Of(1), -1);
            Assert.AreEqual(-Value.Of(1.0f), -1.0f);

            Assert.True((-Value.Of("a")).IsNaN);
            Assert.AreEqual(-Value.Of(""), 0);
            Assert.AreEqual(-Value.Of("1"), -1);
            Assert.AreEqual(-Value.Of("1.0"), -1.0f);
            Assert.AreEqual(-Value.Of("true"), -1);
            Assert.AreEqual(-Value.Of("false"), 0);


            Assert.AreEqual(-Value.ArrayOf(), 0);
            Assert.AreEqual(-Value.ArrayOf(1), -1);
            Assert.AreEqual(-Value.ArrayOf("1"), -1);
            Assert.True((-Value.ArrayOf(1, 2)).IsNaN);

            Assert.True((-Value.Of(Function.Bind(() => { }))).IsNaN);
        }
    }
}
