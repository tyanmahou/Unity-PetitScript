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
        [Test]
        public void TestAdd()
        {
            Assert.AreEqual(Value.Of(1) + Value.Of(1), Value.Of(2));
            Assert.AreEqual(Value.Of(1.025f) + Value.Of(0.5f), Value.Of(1.525f));
            Assert.AreEqual(Value.Of("aa") + Value.Of("bb"), Value.Of("aabb"));
            Assert.AreEqual(Value.Of(true) + Value.Of(true), Value.Of(2));

            Assert.AreEqual(Value.Of(1) + Value.Of(1.1f), Value.Of(2.1f));
            Assert.AreEqual(Value.Of(10) + Value.Of("10"), Value.Of("1010"));
            Assert.AreEqual(Value.Of("10") + Value.Of("10"), Value.Of("1010"));
            Assert.AreEqual(Value.Of(10.0f) + Value.Of("10"), Value.Of("1010"));
            Assert.AreEqual(Value.Of(10) + Value.Of("10.0"), Value.Of("1010.0"));
            Assert.AreEqual(Value.Invalid + Value.Of("10"), Value.Of("10"));

            Assert.AreEqual(Value.Of(1) + Value.ArrayOf(3), Value.Of("13"));
            Assert.AreEqual(Value.Of(1) + Value.ArrayOf(3, 4), Value.Of("13,4"));
        }
        [Test]
        public void TestSub()
        {
            Assert.AreEqual(Value.Of(1) - Value.Of(1), Value.Of(0));
            Assert.AreEqual(Value.Of(1.025f) - Value.Of(0.5f), Value.Of(1.025f - 0.5f));
            Assert.True((Value.Of("aa") - Value.Of("bb")).IsNaN);
            Assert.AreEqual(Value.Of(true) - Value.Of(true), Value.Of(0));

            Assert.AreEqual(Value.Of(1) - Value.Of(1.1f), Value.Of(1 - 1.1f));
            Assert.AreEqual(Value.Of(10) - Value.Of("10"), Value.Of(0));
            Assert.AreEqual(Value.Of("10") - Value.Of("10"), Value.Of(0));
            Assert.AreEqual(Value.Of(10.0f) - Value.Of("10"), Value.Of(10.0f - 10));
            Assert.AreEqual(Value.Of(10) - Value.Of("10.0"), Value.Of(10 - 10.0f));
            Assert.True((Value.Invalid - Value.Of("10")).IsNaN);

            Assert.AreEqual(Value.Of(1) - Value.ArrayOf(3), Value.Of(-2));
            Assert.True((Value.Of(1) - Value.ArrayOf(3, 4)).IsNaN);
        }
        [Test]
        public void TestMul()
        {
            Assert.AreEqual(Value.Of(1) * Value.Of(1), Value.Of(1 * 1));
            Assert.AreEqual(Value.Of(1.025f) * Value.Of(0.5f), Value.Of(1.025f * 0.5f));
            Assert.True((Value.Of("aa") * Value.Of("bb")).IsNaN);
            Assert.AreEqual(Value.Of(true) * Value.Of(true), Value.Of(1));

            Assert.AreEqual(Value.Of(1) * Value.Of(1.1f), Value.Of(1 * 1.1f));
            Assert.AreEqual(Value.Of(10) * Value.Of("10"), Value.Of(10 * 10));
            Assert.AreEqual(Value.Of("10") * Value.Of("10"), Value.Of(10 * 10));
            Assert.AreEqual(Value.Of(10.0f) * Value.Of("10"), Value.Of(10.0f * 10));
            Assert.AreEqual(Value.Of(10) * Value.Of("10.0"), Value.Of(10 * 10.0f));
            Assert.True((Value.Invalid * Value.Of("10")).IsNaN);

            Assert.AreEqual(Value.Of(1) * Value.ArrayOf(3), Value.Of(3));
            Assert.True((Value.Of(1) * Value.ArrayOf(3, 4)).IsNaN);
        }
        [Test]
        public void TestDiv()
        {
            Assert.AreEqual(Value.Of(1) / Value.Of(1), Value.Of(1 * 1));
            Assert.AreEqual(Value.Of(1.025f) / Value.Of(0.5f), Value.Of(1.025f / 0.5f));
            Assert.True((Value.Of("aa") / Value.Of("bb")).IsNaN);
            Assert.AreEqual(Value.Of(true) / Value.Of(true), Value.Of(1));

            Assert.AreEqual(Value.Of(1) / Value.Of(1.1f), Value.Of(1 / 1.1f));
            Assert.AreEqual(Value.Of(10) / Value.Of("10"), Value.Of(10 / 10));
            Assert.AreEqual(Value.Of("10") / Value.Of("10"), Value.Of(10 / 10));
            Assert.AreEqual(Value.Of(10.0f) / Value.Of("10"), Value.Of(10.0f / 10));
            Assert.AreEqual(Value.Of(10) / Value.Of("10.0"), Value.Of(10 / 10.0f));
            Assert.True((Value.Invalid / Value.Of("10")).IsNaN);

            Assert.AreEqual(Value.Of(1) / Value.ArrayOf(3), Value.Of(0));
            Assert.True((Value.Of(1) / Value.ArrayOf(3, 4)).IsNaN);

            // Zero Divide
            Assert.AreEqual(Value.Of(10) / Value.Of(0), Value.Inf);
            Assert.IsTrue((Value.Of(0) / Value.Of(0)).IsNaN);
            Assert.AreEqual(Value.Of(-10) / Value.Of(0), -Value.Inf);
        }
        [Test]
        public void TestMod()
        {
            Assert.AreEqual(Value.Of(1) % Value.Of(1), Value.Of(1 % 1));
            Assert.AreEqual(Value.Of(1.025f) % Value.Of(0.5f), Value.Of(1.025f % 0.5f));
            Assert.True((Value.Of("aa") % Value.Of("bb")).IsNaN);
            Assert.AreEqual(Value.Of(true) % Value.Of(true), Value.Of(1 % 1));

            Assert.AreEqual(Value.Of(1) % Value.Of(1.1f), Value.Of(1 % 1.1f));
            Assert.AreEqual(Value.Of(10) % Value.Of("10"), Value.Of(10 % 10));
            Assert.AreEqual(Value.Of("10") % Value.Of("10"), Value.Of(10 % 10));
            Assert.AreEqual(Value.Of(10.0f) % Value.Of("10"), Value.Of(10.0f % 10));
            Assert.AreEqual(Value.Of(10) % Value.Of("10.0"), Value.Of(10 % 10.0f));
            Assert.True((Value.Invalid % Value.Of("10")).IsNaN);

            Assert.AreEqual(Value.Of(1) % Value.ArrayOf(3), Value.Of(1));
            Assert.True((Value.Of(1) % Value.ArrayOf(3, 4)).IsNaN);

            // Zero Divide
            Assert.IsTrue((Value.Of(10) % Value.Of(0)).IsNaN);
            Assert.IsTrue((Value.Of(0) % Value.Of(0)).IsNaN);
            Assert.IsTrue((Value.Of(-10) % Value.Of(0)).IsNaN);
        }
    }
}
