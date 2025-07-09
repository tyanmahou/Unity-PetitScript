using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Petit.Runtime
{
    class TestValue_Cast
    {
        [Test]
        public void TestCastToInt()
        {
            Assert.AreEqual(Value.Of(2).Cast<int>(), 2);
        }
        [Test]
        public void TestCastToFloat()
        {
            Assert.AreEqual(Value.Of(1.23f).Cast<float>(), 1.23f);
        }
        [Test]
        public void TestCastToString()
        {
            Assert.AreEqual(Value.Of("Test").Cast<string>(), "Test");
        }
        [Test]
        public void TestCastToBool()
        {
            Assert.AreEqual(Value.Of(false).Cast<bool>(), false);
            Assert.AreEqual(Value.Of(true).Cast<bool>(), true);
        }
        [Test]
        public void TestCastToValue()
        {
            Assert.AreEqual(Value.Of(123).Cast<Value>(), Value.Of(123));
        }
        [Test]
        public void TestCastToArray()
        {
            void Check<T>()
                where T : IEnumerable<int>
            {
                T ar = Value.ArrayOf(1, 2, 3).Cast<T>();
                Assert.AreEqual(ar.Count(), 3);
                Assert.AreEqual(ar.ElementAt(0), 1);
                Assert.AreEqual(ar.ElementAt(1), 2);
                Assert.AreEqual(ar.ElementAt(2), 3);
            }
            Check<int[]>();
            Check<List<int>>();
            Check<IList<int>>();
            Check<IReadOnlyList<int>>();
            Check<IReadOnlyCollection<int>>();
            Check<IEnumerable<int>>();
        }
    }
}
