using NUnit.Framework;

namespace Petit.Runtime
{
    class TestValue_Comparable
	{
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
        public void TestCompareArray_Premitive()
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
    }
}
