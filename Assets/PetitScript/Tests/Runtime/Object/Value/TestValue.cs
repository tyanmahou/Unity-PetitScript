using NUnit.Framework;

namespace Petit.Runtime
{
    class TestValue
    {
        [Test]
        public void TestIdentical()
        {
            Value v = Value.Of(1);
            Assert.True(Value.Identical(v, Value.Of(1)));
            Assert.AreEqual(v, Value.Of(1));
            Assert.True(v.Equals(Value.Of(1)));
        }
        [Test]
        public void TestNotIdentical()
        {
            Value v = Value.Of(1);
            Assert.True(Value.NotIdentical(v, Value.Of("1")));
            Assert.AreNotEqual(v, Value.Of("1"));
            Assert.False(v.Equals(Value.Of("1")));

            Assert.True(Value.NotIdentical(v, Value.Of(2)));
            Assert.AreNotEqual(v, Value.Of(2));
            Assert.True(!v.Equals(Value.Of(2)));
        }
        [Test]
        public void TestEquals()
        {
            Value v = Value.Of(1);
            Assert.True(Value.EqualsLoose(v, Value.Of(1)));
            Assert.True(Value.EqualsLoose(v, Value.Of("1")));
            Assert.True(v == Value.Of(1));
            Assert.True(v == Value.Of("1"));
        }
        [Test]
        public void TestNotEquals()
        {
            Value v = Value.Of(1);
            Assert.True(!Value.EqualsLoose(v, Value.Of(2)));
            Assert.True(v != Value.Of(2));
        }
        [Test]
        public void TestParse()
        {
            {
                Value v = Value.Parse(null);
                Assert.True(v.IsInvalid);
                Assert.AreEqual(v, Value.Invalid);
            }
            {
                Value v = Value.Parse("1");
                Assert.True(v.IsInt);
                Assert.AreEqual(v, Value.Of(1));
            }
            {
                Value v = Value.Parse("1.1");
                Assert.True(v.IsFloat);
                Assert.AreEqual(v, Value.Of(1.1f));
            }
            {
                Value v = Value.Parse("NaN");
                Assert.True(v.IsNaN);
            }
            {
                {
                    Value v = Value.Parse("true");
                    Assert.True(v.IsBool);
                    Assert.AreEqual(v, Value.Of(true));
                }
                {
                    Value v = Value.Parse("false");
                    Assert.True(v.IsBool);
                    Assert.AreEqual(v, Value.Of(false));
                }
            }
            {
                Value v = Value.Parse("aaa");
                Assert.True(v.IsString);
                Assert.AreEqual(v, Value.Of("aaa"));
            }
        }      

        [Test]
        public void TestNaN()
        {
            var nan = Value.NaN;
            Assert.True(Value.Of(float.NaN).IsNaN);
            Assert.True((Value.Of(0.0f) / Value.Of(0.0f)).IsNaN);

            Assert.True((+Value.NaN).IsNaN);
            Assert.True((-Value.NaN).IsNaN);

            Assert.False(Value.Identical(Value.NaN, Value.NaN));
            Assert.True(Value.NotIdentical(Value.NaN, Value.NaN));
            Assert.False(nan == Value.NaN);
            Assert.True(nan != Value.NaN);
            Assert.False(nan < Value.NaN);
            Assert.False(nan > Value.NaN);
            Assert.False(nan <= Value.NaN);
            Assert.False(nan >= Value.NaN);
            Assert.False(nan < Value.Of(0));
            Assert.False(nan > Value.Of(0));
            Assert.False(nan <= Value.Of(0));
            Assert.False(nan >= Value.Of(0));

            Assert.AreEqual(Value.NaN.CompareTo(Value.NaN), 0);
            Assert.AreEqual(Value.NaN.CompareTo(Value.Of(0)), -1);
            Assert.AreEqual(Value.Of(0).CompareTo(Value.NaN), 1);
        }
    }
}
