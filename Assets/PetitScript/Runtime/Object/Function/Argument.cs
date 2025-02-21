using System;

namespace Petit.Runtime
{
    /// <summary>
    /// 引数
    /// </summary>
    public readonly struct Argument
    {
        public Argument(string name, Func<Value> value = default)
        {
            Name = name;
            Value = value;
        }
        public Argument(string name, Value value = default)
        {
            Name = name;
            Value = () => value;
        }
        public Argument(string name, bool value)
            : this(name, Runtime.Value.Of(value))
        {
        }
        public Argument(string name, int value)
            : this(name, Runtime.Value.Of(value))
        {
        }
        public Argument(string name, float value)
            : this(name, Runtime.Value.Of(value))
        {
        }

        public Argument(string name, string value)
            : this(name, Runtime.Value.Of(value))
        {
        }

        public readonly string Name;
        public readonly Func<Value> Value;
    }
}
