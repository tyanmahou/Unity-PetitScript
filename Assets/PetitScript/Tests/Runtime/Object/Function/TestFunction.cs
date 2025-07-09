using NUnit.Framework;
using System.Linq;
using System.Collections.Generic;

namespace Petit.Runtime
{
    class TestFunction
    {
        [Test]
        public void TestNormal()
        {
            Value Add(Value a1, Value b1)
            {
                return a1 + b1;
            }
            Function func = Function.Bind(Add);
            Assert.AreEqual(func.Parameters[0].Name, "a1");
            Assert.AreEqual(func.Parameters[1].Name, "b1");
            Assert.AreEqual(func.Invoke(1, 2), 3);
        }
        [Test]
        public void TestNormalPure()
        {
            {
                int Add(int a1, int b1)
                {
                    return a1 + b1;
                }
                Function func = Function.Bind<int, int, int>(Add);
                Assert.AreEqual(func.Parameters[0].Name, "a1");
                Assert.AreEqual(func.Parameters[1].Name, "b1");
                Assert.AreEqual(func.Invoke(1, 2), 3);
            }
            {
                void Check<T>()
                    where T : IEnumerable<int>
                {
                    int Sum(T ar)
                    {
                        return ar.Sum();
                    }
                    Function func = Function.Bind<T, int>(Sum);
                    Assert.AreEqual(func.Invoke(Value.ArrayOf(1, "2", 3.0f)), 6);
                }
                Check<int[]>();
                Check<List<int>>();
                Check<IList<int>>();
                Check<IReadOnlyList<int>>();
                Check<IReadOnlyCollection<int>>();
                Check<IEnumerable<int>>();
            }
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
            Assert.AreEqual(func.Invoke(1, 2), 3);
        }
        [Test]
        public void TestComposit()
        {
            Function plus1 = Function.Bind(x => x + 1);
            Function twice = Function.Bind(x => x + x);

            Function composit = plus1.Composite(twice);

            Assert.AreEqual(plus1.Invoke(2), 3);
            Assert.AreEqual(twice.Invoke(2), 4);
            Assert.AreEqual(composit.Invoke(2), 5);
        }
        [Test]
        public void TestPartial()
        {
            Function add = Function.Bind((x, y) => x + y);

            Function add5 = add.Partial(5);

            Assert.AreEqual(add.Invoke(2, 3), 5);
            Assert.AreEqual(add5.Invoke(2), 7);
        }
    }
}
