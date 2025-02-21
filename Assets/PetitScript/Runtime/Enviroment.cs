using System.Collections.Generic;

namespace Petit.Runtime
{
    public class Enviroment
    {
        public static Enviroment Global => _global;
        public static Enviroment New => new(Global);

        static Enviroment _global = new();
        Enviroment()
        {
        }
        public Enviroment(Enviroment parent)
        {
            _parent = parent ?? _global;
        }
        public Enviroment Stack()
        {
            return new Enviroment(this);
        }

        /// <summary>
        /// 変数をセット
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Set(string key, bool value)
        {
            _variables[key] = Value.Of(value);
        }
        public void Set(string key, int value)
        {
            _variables[key] = Value.Of(value);
        }
        public void Set(string key, float value)
        {
            _variables[key] = Value.Of(value);
        }
        public void Set(string key, string value)
        {
            _variables[key] = Value.Of(value);
        }
        public void Set(string key, Function func)
        {
            _variables[key] = Value.Of(func);
        }
        public void Set(string key, in Value value)
        {
            _variables[key] = value;
        }
        public Value Get(string key)
        {
            if (_variables.TryGetValue(key, out Value value))
            {
                return value;
            }
            return _parent?.Get(key) ?? Value.Invalid;
        }

        Dictionary<string, Value> _variables = new();
        readonly Enviroment _parent = null;
    }
}
