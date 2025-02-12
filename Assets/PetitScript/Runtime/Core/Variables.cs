using System.Collections.Generic;

namespace Petit.Core
{
    /// <summary>
    /// 変数コンテナ
    /// </summary>
    public class Variables
    {
        /// <summary>
        /// グローバル変数としてセット
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SetGlobal(string key, bool value)
        {
            _globalVariables[key] = new Value(value);
        }
        public static void SetGlobal(string key, int value)
        {
            _globalVariables[key] = new Value(value);
        }
        public static void SetGlobal(string key, float value)
        {
            _globalVariables[key] = new Value(value);
        }
        public static void SetGlobal(string key, string value)
        {
            _globalVariables[key] = new Value(value);
        }
        public static void SetGlobal(string key, in Value value)
        {
            _globalVariables[key] = value;
        }

        public Variables()
        {
        }

        /// <summary>
        /// 変数をセット
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Set(string key, bool value)
        {
            _variables[key] = new Value(value);
        }
        public void Set(string key, int value)
        {
            _variables[key] = new Value(value);
        }
        public void Set(string key, float value)
        {
            _variables[key] = new Value(value);
        }
        public void Set(string key, string value)
        {
            _variables[key] = new Value(value);
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
            if (_globalVariables.TryGetValue(key, out Value globalValue))
            {
                return globalValue;
            }
            return Value.Invalid;
        }
        public Function GetFunc(string key)
        {
            if (_functions.TryGetValue(key, out Function func))
            {
                return func;
            }
            return null;
        }
        public void SetFunc(string key, Function func)
        {
            _functions.TryAdd(key, func);
        }
        public Value this[string key]
        {
            get => Get(key);
            set => Set(key, value);
        }
        public void ClearLocal()
        {
            _variables.Clear();
            _functions.Clear();
        }
        Dictionary<string, Value> _variables = new();
        Dictionary<string, Function> _functions = new();
        static Dictionary<string, Value> _globalVariables = new();
    }
}
