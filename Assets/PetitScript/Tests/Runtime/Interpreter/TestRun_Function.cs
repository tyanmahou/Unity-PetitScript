﻿using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Petit.Runtime
{
    class TestRun_Function : TestRunBase
    {
        [Test]
        public void TestFunc()
        {
            string code = @"
fn Add(a, b)
{
  return a + b;
}
Add(1, 2 * 2)
";
            RunInt(code, 5);
        }
        [Test]
        public void TestFuncBind()
        {
            Environment env = Environment.New();
            env["Add"] = Function.Bind((a1, a2) => a1 + a2);
            RunInt("Add(1, 2 * 2)", 5, env);
        }
        [Test]
        public void TestFuncRaw()
        {
            Environment env = Environment.New();
            Value Sum(IReadOnlyList<Value> args)
            {
                return args.Sum(a => a.ToInt());
            }
            env["Sum"] = Function.Raw(Sum);
            RunInt("Sum(1, 2)", 3, env);
            RunInt("Sum(1, 2, 3)", 6, env);
        }
        [Test]
        public void TestNamedArgs()
        {
            Interpreter interpreter = new();
            interpreter.Environment["Sub"] = Function.Bind((a, b) => a - b);
            {
                var result = interpreter.Run("Sub(a: 1, b: 2)");
                Assert.AreEqual(result, -1);
            }
            {
                var result = interpreter.Run("Sub(b: 2, a: 1)");
                Assert.AreEqual(result, -1);
            }
            {
                var result = interpreter.Run("Sub(2, a: 1)");
                Assert.AreEqual(result, -1);
            }
            {
                var result = interpreter.Run("Sub(a: 1, 2)");
                Assert.AreEqual(result, -1);
            }
            {
                var result = interpreter.Run("Sub(b: 2, 1)");
                Assert.AreEqual(result, -1);
            }
            {
                var result = interpreter.Run("Sub(1, b: 2)");
                Assert.AreEqual(result, -1);
            }
            {
                var result = interpreter.Run("Sub(1, 2)");
                Assert.AreEqual(result, -1);
            }
            {
                var result = interpreter.Run("Sub(2, 1)");
                Assert.AreEqual(result, 1);
            }
        }
        [Test]
        public void TestRecursion()
        {
            string code = @"
fn fib(n) {
    if (n <= 1) {
        return n;
    }
    return fib(n - 1) + fib(n - 2);
}
return fib(10);
";
            RunInt(code, 55);
        }
        [Test]
        public void TestDefaultArgBind()
        {
            Interpreter interpreter = new();
            interpreter.Environment["Add"] = 
                Function.Bind((a, b) => a + b)
                .SetDefaultValue(0, 1)
                .SetDefaultValue(1, 2);
            {
                var result = interpreter.Run("Add()");
                Assert.AreEqual(result, 3);
            }
            {
                var result = interpreter.Run("Add(10)");
                Assert.AreEqual(result, 12);
            }
            {
                var result = interpreter.Run("Add(b:10)");
                Assert.AreEqual(result, 11);
            }
        }
        [Test]
        public void TestDefaultArg()
        {
            {
                string code = @"
fn myfunc(a = 2,  b = a * a) {
    return a + b;
}
myfunc();
";
                RunInt(code, 6);
                Interpreter interpreter = new();
                var result = interpreter.Run(code);
                Assert.AreEqual(result, 6);
            }
            {
                string code = @"
fn myfunc(a = 2,  b = a * a) {
    return a + b;
}
myfunc(a);
";
                Environment env = Environment.New();
                env["a"] = 5;
                RunInt(code, 30, env);
            }
        }
    }
}
