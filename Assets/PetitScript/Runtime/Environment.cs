using System;
using System.Collections.Generic;

namespace Petit.Runtime
{
    public class Environment
    {
        public static Environment Global => _global;
        public static Environment New() => new(Global);

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
            if (_variables.TryGetValue(key, out Value v))
            {
                v.SetIndirect(value);
            }
            else
            {
                _variables.Add(key, new Reference(value));
            }
        }
        public Value Get(string key)
        {
            return GetOpt(key) ?? Value.Invalid;
        }
        public Value? GetOpt(string key)
        {
            if (_variables.TryGetValue(key, out Value value))
            {
                return value;
            }
            return _parent?.GetOpt(key);
        }
        public Value GetOrSet(string key)
        {
            Value? result = GetOpt(key);
            if (result is null)
            {
                Set(key, Value.Invalid);
                return Get(key);
            }
            else
            {
                return result.Value;
            }
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
