using NUnit.Framework;

namespace Petit.Runtime
{
    class TestValue_Comparable
    {
        [Test]
        public void TestCompareInt()
        {
            Assert.AreEqual(Value.Of(-1).CompareTo(Value.Of(1)), -1);
            Assert.AreEqual(Value.Of(0).CompareTo(Value.Of(0)), 0);
            Assert.AreEqual(Value.Of(1).CompareTo(Value.Of(-1)), 1);
        }
        [Test]
        public void TestCompareFloat()
        {
            Assert.AreEqual(Value.Of(-1).CompareTo(Value.Of(1.0f)), -1);
            Assert.AreEqual(Value.Of(0).CompareTo(Value.Of(0.0f)), 0);
            Assert.AreEqual(Value.Of(1).CompareTo(Value.Of(-1.0f)), 1);
        }
        [Test]
        public void TestCompareString()
        {
            Assert.AreEqual(Value.Of("aaa").CompareTo(Value.Of("bbb")), -1);
            Assert.AreEqual(Value.Of("ccc").CompareTo(Value.Of("ccc")), 0);
            Assert.AreEqual(Value.Of("bbb").CompareTo(Value.Of("aaa")), 1);

            Assert.AreEqual(Value.Of("111").CompareTo(Value.Of("aaa")), -1);
            Assert.AreEqual(Value.Of("aaa").CompareTo(Value.Of("111")), 1);
        }

        [Test]
        public void TestCompareNumeric()
        {
            Assert.AreEqual(Value.Of(-1).CompareTo(Value.Of("1")), -1);
            Assert.AreEqual(Value.Of(0).CompareTo(Value.Of("0")), 0);
            Assert.AreEqual(Value.Of(1).CompareTo(Value.Of("-1")), 1);

            Assert.AreEqual(Value.Of(-1).CompareTo(Value.Of("1.0")), -1);
            Assert.AreEqual(Value.Of(0).CompareTo(Value.Of("0.0")), 0);
            Assert.AreEqual(Value.Of(1).CompareTo(Value.Of("-1.0")), 1);

            Assert.AreEqual(Value.Of("0").CompareTo(Value.Of("0.0")), -1);
            Assert.AreEqual(Value.Of("0").CompareTo(Value.Of("0")), 0);
            Assert.AreEqual(Value.Of("0.0").CompareTo(Value.Of("0")), 1);

            Assert.AreEqual(Value.Of(-1).CompareTo(Value.Of(true)), -1);
            Assert.AreEqual(Value.Of(0).CompareTo(Value.Of(false)), 0);
        }
        [Test]
        public void TestCompareNaN()
        {
            Assert.AreEqual(Value.Compare(Value.NaN, Value.NaN), 0);
            Assert.AreEqual(Value.Compare(Value.NaN, Value.Of(99999)), -1);
            Assert.AreEqual(Value.Compare(Value.NaN, Value.Of(-99999)), -1);
            Assert.AreEqual(Value.Compare(Value.NaN, Value.Of("aaaa")), -1);
            Assert.AreEqual(Value.Compare(Value.Of(99999), Value.NaN), 1);
            Assert.AreEqual(Value.Compare(Value.Of(-99999), Value.NaN), 1);
            Assert.AreEqual(Value.Compare(Value.Of("aaaa"), Value.NaN), 1);
        }
        [Test]
        public void TestCompareInf()
        {
            Assert.AreEqual(Value.Compare(Value.Inf, Value.Inf), 0);
            Assert.AreEqual(Value.Compare(Value.Inf, -Value.Inf), 1);
            Assert.AreEqual(Value.Compare(Value.Inf, Value.Of(int.MaxValue)), 1);
            Assert.AreEqual(Value.Compare(Value.Inf, Value.Of(float.MaxValue)), 1);
            Assert.AreEqual(Value.Compare(-Value.Inf, Value.Inf), -1);
            Assert.AreEqual(Value.Compare(-Value.Inf, -Value.Inf), 0);
            Assert.AreEqual(Value.Compare(-Value.Inf, Value.Of(int.MinValue)), -1);
            Assert.AreEqual(Value.Compare(-Value.Inf, Value.Of(float.MinValue)), -1);
        }
        [Test]
        public void TestCompareInf_NaN()
        {
            Assert.AreEqual(Value.Compare(Value.NaN, -Value.Inf), -1);
            Assert.AreEqual(Value.Compare(Value.NaN, Value.Inf), -1);
            Assert.AreEqual(Value.Compare(-Value.Inf, Value.NaN), 1);
            Assert.AreEqual(Value.Compare(Value.Inf, Value.NaN), 1);
        }
        [Test]
        public void TestCompareArrayPremitive()
        {
            Assert.AreEqual(Value.Compare(Value.ArrayOf(9), Value.Of(10)), -1);
            Assert.AreEqual(Value.Compare(Value.ArrayOf(9), Value.Of(9)), 0);
            Assert.AreEqual(Value.Compare(Value.ArrayOf(9), Value.Of(8)), 1);

            Assert.AreEqual(Value.Compare(Value.ArrayOf(9), Value.Of("10")), 1);
            Assert.AreEqual(Value.Compare(Value.ArrayOf(9), Value.Of("9")), 0);
            Assert.AreEqual(Value.Compare(Value.ArrayOf(9), Value.Of("8")), 1);

            Assert.AreEqual(Value.Compare(Value.ArrayOf(1, 2, 3, 4, 5), Value.Of("1,2,3,4,5")), 0);
            Assert.AreEqual(Value.Compare(Value.ArrayOf(1, 2, 3, Value.ArrayOf(4, 5)), Value.Of("1,2,3,4,5")), 0);

            Assert.AreEqual(Value.Compare(Value.ArrayOf(float.NaN), Value.NaN), 0);
            Assert.AreEqual(Value.Compare(Value.ArrayOf(float.PositiveInfinity), Value.Inf), 0);
            Assert.AreEqual(Value.Compare(Value.ArrayOf(float.NegativeInfinity), -Value.Inf), 0);
        }
        static void A() { }
        static void B() { }
        [Test]
        public void TestCompareFunction()
        {
            Assert.AreEqual(Value.Compare(Value.Of(Function.Bind(A)), Value.Of(Function.Bind(B))), -1);
            Assert.AreEqual(Value.Compare(Value.Of(Function.Bind(A)), Value.Of(Function.Bind(A))), 0);
            Assert.AreEqual(Value.Compare(Value.Of(Function.Bind(B)), Value.Of(Function.Bind(A))), 1);

            Assert.AreEqual(Value.Compare(Value.Of(Function.Bind(A)), Value.Of("fn A()")), 0);
        }
    }
}
