using System.Collections.Generic;

namespace Petit.Runtime
{
    public class Environment
    {
        public static Environment Global => _global;
        public static Environment New => new(Global);

        static Environment _global = new();
        Environment()
        {
        }
        public Environment(Environment parent)
        {
            _parent = parent ?? _global;
        }
        public Environment Stack()
        {
            return new Environment(this);
        }

        /// <summary>
        /// 変数をセット
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
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

        public Value this[string key]
        {
            get => Get(key);
            set => Set(key, value);
        }

        Dictionary<string, Value> _variables = new();
        readonly Environment _parent = null;
    }
}
