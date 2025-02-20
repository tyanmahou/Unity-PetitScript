﻿using NUnit.Framework;

namespace Petit.Runtime
{
    class TestInterpreterBase
    {
        protected static Interpreter RunInt(string code, int actual)
        {
            Interpreter interpreter = new Interpreter();
            var result = interpreter.Run(code);
            Assert.True(result.IsInt);
            Assert.AreEqual(result.ToInt(), actual);
            return interpreter;
        }
        protected static Interpreter RunFloat(string code, float actual)
        {
            Interpreter interpreter = new Interpreter();
            var result = interpreter.Run(code);
            Assert.True(result.IsFloat);
            Assert.AreEqual(result.ToFloat(), actual);
            return interpreter;
        }
        protected static Interpreter RunBool(string code, bool actual)
        {
            Interpreter interpreter = new Interpreter();
            var result = interpreter.Run(code);
            Assert.True(result.IsBool);
            Assert.AreEqual(result.ToBool(), actual);
            return interpreter;
        }
        protected static Interpreter RunString(string code, string actual, Enviroment env = null)
        {
            Interpreter interpreter = new Interpreter();
            var result = interpreter.Run(code, env);
            Assert.True(result.IsString);
            Assert.AreEqual(result.ToString(), actual);
            return interpreter;
        }
    }
}
