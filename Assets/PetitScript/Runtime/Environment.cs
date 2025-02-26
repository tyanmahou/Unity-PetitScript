using System;
using System.Collections.Generic;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;

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
            if (!TrySet(key, value)) 
            {
                _variables[key] = value;
            }
        }
        /// <summary>
        /// このスコープに変数をセット
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetScope(string key, in Value value)
        {
            _variables[key] = value;
        }
        public bool TrySet(string key, in Value value)
        {
            if (_variables.ContainsKey(key))
            {
                _variables[key] = value;
                return true;
            }
            return _parent?.TrySet(key, value) ?? false;
        }

        /// <summary>
        /// 変数の取得
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Value Get(string key)
        {
            if (TryGet(key, out var result))
            {
                return result;
            }
            return Value.Invalid;
        }
        public bool TryGet(string key, out Value result)
        {
            if (_variables.TryGetValue(key, out result))
            {
                return true;
            }
            return _parent?.TryGet(key, out result) ?? false;
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
