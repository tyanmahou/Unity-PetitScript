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
        public void TestPlus()
        {
            // int
            {
                Value v = Value.Of(1);
                Assert.AreEqual(+v, Value.Of(1));
            }
            // float
            {
                Value v = Value.Of(1.0f);
                Assert.AreEqual(+v, Value.Of(1.0f));
            }
            // bool
            {
                Value v = Value.Of(true);
                Assert.AreEqual(+v, Value.Of(1));
            }
            // string
            {
                {
                    Value v = Value.Of("1");
                    Assert.AreEqual(+v, Value.Of(1));
                }
                {
                    Value v = Value.Of("1.1");
                    Assert.AreEqual(+v, Value.Of(1.1f));
                }
                {
                    Value v = Value.Of("true");
                    Assert.AreEqual(+v, Value.Of(1));
                }
                {
                    Value v = Value.Of("aaa");
                    Assert.True((+v).IsNaN);
                }
            }
        }
        [Test]
        public void TestMinus()
        {
            // int
            {
                Value v = Value.Of(1);
                Assert.AreEqual(-v, Value.Of(-1));
            }
            // float
            {
                Value v = Value.Of(1.0f);
                Assert.AreEqual(-v, Value.Of(-1.0f));
            }
            // bool
            {
                Value v = Value.Of(true);
                Assert.AreEqual(-v, Value.Of(-1));
            }
            // string
            {
                {
                    Value v = Value.Of("1");
                    Assert.AreEqual(-v, Value.Of(-1));
                }
                {
                    Value v = Value.Of("1.1");
                    Assert.AreEqual(-v, Value.Of(-1.1f));
                }
                {
                    Value v = Value.Of("true");
                    Assert.AreEqual(-v, Value.Of(-1));
                }
                {
                    Value v = Value.Of("aaa");
                    Assert.True((-v).IsNaN);
                }
            }
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

        }
        [Test]
        public void TestSub()
        {
            Assert.AreEqual(Value.Of(1) - Value.Of(1), Value.Of(0));
            Assert.AreEqual(Value.Of(1.025f) - Value.Of(0.5f), Value.Of(1.025f - 0.5f));
            Assert.True((Value.Of("aa") - Value.Of("bb")).IsNaN);
            Assert.AreEqual(Value.Of(true) - Value.Of(true), Value.Of(0));

            Assert.AreEqual(Value.Of(1) - Value.Of(1.1f), Value.Of(1-1.1f));
            Assert.AreEqual(Value.Of(10) - Value.Of("10"), Value.Of(0));
            Assert.AreEqual(Value.Of("10") - Value.Of("10"), Value.Of(0));
            Assert.AreEqual(Value.Of(10.0f) - Value.Of("10"), Value.Of(10.0f - 10));
            Assert.AreEqual(Value.Of(10) - Value.Of("10.0"), Value.Of(10 -10.0f));
            Assert.True((Value.Invalid - Value.Of("10")).IsNaN);
        }
        [Test]
        public void TestMul()
        {
            Assert.AreEqual(Value.Of(1) *  Value.Of(1), Value.Of(1 * 1));
            Assert.AreEqual(Value.Of(1.025f) * Value.Of(0.5f), Value.Of(1.025f * 0.5f));
            Assert.True((Value.Of("aa") * Value.Of("bb")).IsNaN);
            Assert.AreEqual(Value.Of(true) * Value.Of(true), Value.Of(1));

            Assert.AreEqual(Value.Of(1) * Value.Of(1.1f), Value.Of(1 * 1.1f));
            Assert.AreEqual(Value.Of(10) * Value.Of("10"), Value.Of(10 * 10));
            Assert.AreEqual(Value.Of("10") * Value.Of("10"), Value.Of(10 * 10));
            Assert.AreEqual(Value.Of(10.0f) * Value.Of("10"), Value.Of(10.0f *  10));
            Assert.AreEqual(Value.Of(10) * Value.Of("10.0"), Value.Of(10 * 10.0f));
            Assert.True((Value.Invalid * Value.Of("10")).IsNaN);
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
