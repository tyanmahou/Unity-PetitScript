﻿using NUnit.Framework;

namespace Petit.Runtime
{
    class TestRun_List : TestRunBase
    {
        [Test]
        public void TestEmpty()
        {
            var result = Interpreter.RunScript("[]");
            Assert.True(result.IsList);
            Assert.AreEqual(result.ToList().Count, 0);
        }
        [Test]
        public void TestUnit()
        {
            var result = Interpreter.RunScript("[1]");
            Assert.True(result.IsList);
            Assert.AreEqual(result.ToList().Count, 1);
            Assert.AreEqual(result[0], 1);
        }
        [Test]
        public void TestList()
        {
            var result = Interpreter.RunScript("[1, 2]");
            Assert.True(result.IsList);
            Assert.AreEqual(result.ToList().Count, 2);
            Assert.AreEqual(result[0], 1);
            Assert.AreEqual(result[1], 2);
        }
        [Test]
        public void TestLastCamma()
        {
            var result = Interpreter.RunScript("[1, 2, ]");
            Assert.True(result.IsList);
            Assert.AreEqual(result.ToList().Count, 2);
            Assert.AreEqual(result[0], 1);
            Assert.AreEqual(result[1], 2);
        }
        [Test]
        public void TestBool()
        {
            {
                var result = Interpreter.RunScript("[]");
                Assert.False((bool)result);
            }
            {
                var result = Interpreter.RunScript("[0]");
                Assert.True((bool)result);
            }
        }
        [Test]
        public void TestString()
        {
            var result = Interpreter.RunScript("[1, 2, 3, [4, 5]]");
            Assert.AreEqual(result.ToString(), "[1, 2, 3, [4, 5]]");
        }
    }
}
