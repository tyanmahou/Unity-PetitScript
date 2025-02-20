using NUnit.Framework;

namespace Petit.Core
{
    class ValueTest
    {
        [Test]
        public void TestIdentical()
        {
            Value v = new Value(1);
            Assert.True(Value.Identical(v, new(1)));
            Assert.AreEqual(v, new Value(1));
            Assert.True(v.Equals(new Value(1)));
        }
        [Test]
        public void TestNotIdentical()
        {
            Value v = new Value(1);
            Assert.True(Value.NotIdentical(v, new("1")));
            Assert.AreNotEqual(v, new Value("1"));
            Assert.False(v.Equals(new Value("1")));

            Assert.True(Value.NotIdentical(v, new(2)));
            Assert.AreNotEqual(v, new Value(2));
            Assert.True(!v.Equals(new Value(2)));
        }
        [Test]
        public void TestEquals()
        {
            Value v = new Value(1);
            Assert.True(Value.EqualsLoose(v, new(1)));
            Assert.True(Value.EqualsLoose(v, new("1")));
            Assert.True(v == new Value(1));
            Assert.True(v == new Value("1"));
        }
        [Test]
        public void TestNotEquals()
        {
            Value v = new Value(1);
            Assert.True(!Value.EqualsLoose(v, new(2)));
            Assert.True(v != new Value(2));
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
                Assert.AreEqual(v, new Value(1));
            }
            {
                Value v = Value.Parse("1.1");
                Assert.True(v.IsFloat);
                Assert.AreEqual(v, new Value(1.1f));
            }
            {
                Value v = Value.Parse("NaN");
                Assert.True(v.IsNaN);
            }
            {
                {
                    Value v = Value.Parse("true");
                    Assert.True(v.IsBool);
                    Assert.AreEqual(v, new Value(true));
                }
                {
                    Value v = Value.Parse("false");
                    Assert.True(v.IsBool);
                    Assert.AreEqual(v, new Value(false));
                }
            }
            {
                Value v = Value.Parse("aaa");
                Assert.True(v.IsString);
                Assert.AreEqual(v, new Value("aaa"));
            }
        }
        [Test]
        public void TestPlus()
        {
            // int
            {
                Value v = new Value(1);
                Assert.AreEqual(+v, new Value(1));
            }
            // float
            {
                Value v = new Value(1.0f);
                Assert.AreEqual(+v, new Value(1.0f));
            }
            // bool
            {
                Value v = new Value(true);
                Assert.AreEqual(+v, new Value(1));
            }
            // string
            {
                {
                    Value v = new Value("1");
                    Assert.AreEqual(+v, new Value(1));
                }
                {
                    Value v = new Value("1.1");
                    Assert.AreEqual(+v, new Value(1.1f));
                }
                {
                    Value v = new Value("true");
                    Assert.AreEqual(+v, new Value(1));
                }
                {
                    Value v = new Value("aaa");
                    Assert.True((+v).IsNaN);
                }
            }
        }
        [Test]
        public void TestMinus()
        {
            // int
            {
                Value v = new Value(1);
                Assert.AreEqual(-v, new Value(-1));
            }
            // float
            {
                Value v = new Value(1.0f);
                Assert.AreEqual(-v, new Value(-1.0f));
            }
            // bool
            {
                Value v = new Value(true);
                Assert.AreEqual(-v, new Value(-1));
            }
            // string
            {
                {
                    Value v = new Value("1");
                    Assert.AreEqual(-v, new Value(-1));
                }
                {
                    Value v = new Value("1.1");
                    Assert.AreEqual(-v, new Value(-1.1f));
                }
                {
                    Value v = new Value("true");
                    Assert.AreEqual(-v, new Value(-1));
                }
                {
                    Value v = new Value("aaa");
                    Assert.True((-v).IsNaN);
                }
            }
        }
        [Test]
        public void TestAdd()
        {
            Assert.AreEqual(new Value(1) + new Value(1), new Value(2));
            Assert.AreEqual(new Value(1.025f) + new Value(0.5f), new Value(1.525f));
            Assert.AreEqual(new Value("aa") + new Value("bb"), new Value("aabb"));
            Assert.AreEqual(new Value(true) + new Value(true), new Value(2));

            Assert.AreEqual(new Value(1) + new Value(1.1f), new Value(2.1f));
            Assert.AreEqual(new Value(10) + new Value("10"), new Value("1010"));
            Assert.AreEqual(new Value("10") + new Value("10"), new Value("1010"));
            Assert.AreEqual(new Value(10.0f) + new Value("10"), new Value("1010"));
            Assert.AreEqual(new Value(10) + new Value("10.0"), new Value("1010.0"));
            Assert.AreEqual(Value.Invalid + new Value("10"), new Value("10"));

        }
        [Test]
        public void TestSub()
        {
            Assert.AreEqual(new Value(1) - new Value(1), new Value(0));
            Assert.AreEqual(new Value(1.025f) - new Value(0.5f), new Value(1.025f - 0.5f));
            Assert.True((new Value("aa") - new Value("bb")).IsNaN);
            Assert.AreEqual(new Value(true) - new Value(true), new Value(0));

            Assert.AreEqual(new Value(1) - new Value(1.1f), new Value(1-1.1f));
            Assert.AreEqual(new Value(10) - new Value("10"), new Value(0));
            Assert.AreEqual(new Value("10") - new Value("10"), new Value(0));
            Assert.AreEqual(new Value(10.0f) - new Value("10"), new Value(10.0f - 10));
            Assert.AreEqual(new Value(10) - new Value("10.0"), new Value(10 -10.0f));
            Assert.True((Value.Invalid - new Value("10")).IsNaN);
        }
        [Test]
        public void TestMul()
        {
            Assert.AreEqual(new Value(1) *  new Value(1), new Value(1 * 1));
            Assert.AreEqual(new Value(1.025f) * new Value(0.5f), new Value(1.025f * 0.5f));
            Assert.True((new Value("aa") * new Value("bb")).IsNaN);
            Assert.AreEqual(new Value(true) * new Value(true), new Value(1));

            Assert.AreEqual(new Value(1) * new Value(1.1f), new Value(1 * 1.1f));
            Assert.AreEqual(new Value(10) * new Value("10"), new Value(10 * 10));
            Assert.AreEqual(new Value("10") * new Value("10"), new Value(10 * 10));
            Assert.AreEqual(new Value(10.0f) * new Value("10"), new Value(10.0f *  10));
            Assert.AreEqual(new Value(10) * new Value("10.0"), new Value(10 * 10.0f));
            Assert.True((Value.Invalid * new Value("10")).IsNaN);
        }
        [Test]
        public void TestDiv()
        {
            Assert.AreEqual(new Value(1) / new Value(1), new Value(1 * 1));
            Assert.AreEqual(new Value(1.025f) / new Value(0.5f), new Value(1.025f / 0.5f));
            Assert.True((new Value("aa") / new Value("bb")).IsNaN);
            Assert.AreEqual(new Value(true) / new Value(true), new Value(1));

            Assert.AreEqual(new Value(1) / new Value(1.1f), new Value(1 / 1.1f));
            Assert.AreEqual(new Value(10) / new Value("10"), new Value(10 / 10));
            Assert.AreEqual(new Value("10") / new Value("10"), new Value(10 / 10));
            Assert.AreEqual(new Value(10.0f) / new Value("10"), new Value(10.0f / 10));
            Assert.AreEqual(new Value(10) / new Value("10.0"), new Value(10 / 10.0f));
            Assert.True((Value.Invalid / new Value("10")).IsNaN);
        }
        [Test]
        public void TestMod()
        {
            Assert.AreEqual(new Value(1) % new Value(1), new Value(1 % 1));
            Assert.AreEqual(new Value(1.025f) % new Value(0.5f), new Value(1.025f % 0.5f));
            Assert.True((new Value("aa") % new Value("bb")).IsNaN);
            Assert.AreEqual(new Value(true) % new Value(true), new Value(1 % 1));

            Assert.AreEqual(new Value(1) % new Value(1.1f), new Value(1 % 1.1f));
            Assert.AreEqual(new Value(10) % new Value("10"), new Value(10 % 10));
            Assert.AreEqual(new Value("10") % new Value("10"), new Value(10 % 10));
            Assert.AreEqual(new Value(10.0f) % new Value("10"), new Value(10.0f % 10));
            Assert.AreEqual(new Value(10) % new Value("10.0"), new Value(10 % 10.0f));
            Assert.True((Value.Invalid % new Value("10")).IsNaN);
        }

        [Test]
        public void TestCompare()
        {
            Assert.AreEqual(new Value(-1).CompareTo(new Value(1)), -1);
            Assert.AreEqual(new Value(0).CompareTo(new Value(0)), 0);
            Assert.AreEqual(new Value(1).CompareTo(new Value(-1)), 1);

            Assert.AreEqual(new Value(-1).CompareTo(new Value(1.0f)), -1);
            Assert.AreEqual(new Value(0).CompareTo(new Value(0.0f)), 0);
            Assert.AreEqual(new Value(1).CompareTo(new Value(-1.0f)), 1);

            Assert.AreEqual(new Value(-1).CompareTo(new Value("1")), -1);
            Assert.AreEqual(new Value(0).CompareTo(new Value("0")), 0);
            Assert.AreEqual(new Value(1).CompareTo(new Value("-1")), 1);

            Assert.AreEqual(new Value(-1).CompareTo(new Value("1.0")), -1);
            Assert.AreEqual(new Value(0).CompareTo(new Value("0.0")), 0);
            Assert.AreEqual(new Value(1).CompareTo(new Value("-1.0")), 1);

            Assert.AreEqual(new Value("0").CompareTo(new Value("0.0")), -1);
            Assert.AreEqual(new Value("0").CompareTo(new Value("0")), 0);
            Assert.AreEqual(new Value("0.0").CompareTo(new Value("0")), 1);

            Assert.AreEqual(new Value("aaa").CompareTo(new Value("bbb")), -1);
            Assert.AreEqual(new Value("ccc").CompareTo(new Value("ccc")), 0);
            Assert.AreEqual(new Value("bbb").CompareTo(new Value("aaa")), 1);

            Assert.AreEqual(new Value("111").CompareTo(new Value("aaa")), -1);
            Assert.AreEqual(new Value("aaa").CompareTo(new Value("111")), 1);
        }
        [Test]
        public void TestNaN()
        {
            var nan = Value.NaN;
            Assert.True(new Value(float.NaN).IsNaN);
            Assert.True((new Value(0.0f) / new Value(0.0f)).IsNaN);

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
            Assert.False(nan < new Value(0));
            Assert.False(nan > new Value(0));
            Assert.False(nan <= new Value(0));
            Assert.False(nan >= new Value(0));

            Assert.AreEqual(Value.NaN.CompareTo(Value.NaN), 0);
            Assert.AreEqual(Value.NaN.CompareTo(new(0)), -1);
            Assert.AreEqual(new Value(0).CompareTo(Value.NaN), 1);
        }
    }
}
