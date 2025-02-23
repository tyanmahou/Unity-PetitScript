using NUnit.Framework;

namespace Petit.Runtime
{
    class TestRunBase
    {
        protected static Interpreter RunInt(string code, int actual, Environment env = null)
        {
            Interpreter interpreter = new Interpreter();
            var result = interpreter.Run(code, env);
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
        protected static Interpreter RunString(string code, string actual, Environment env = null)
        {
            Interpreter interpreter = new Interpreter();
            var result = interpreter.Run(code, env);
            Assert.True(result.IsString);
            Assert.AreEqual(result.ToString(), actual);
            return interpreter;
        }
        protected static Interpreter RunNaN(string code, Environment env = null)
        {
            Interpreter interpreter = new Interpreter();
            var result = interpreter.Run(code, env);
            Assert.True(result.IsNaN);
            return interpreter;
        }
    }
}
