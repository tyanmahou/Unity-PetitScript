using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Petit.Runtime
{
    /// <summary>
    /// 関数
    /// </summary>
    public partial class Function
    {
        internal Function(
            string name,
            Func<IReadOnlyList<Value>, Value> func,
            params Argument[] parameters
            )
        {
            _name = name ?? string.Empty;
            _func = func;
            _params = parameters;
        }
        public Value Invoke(IReadOnlyList<Argument> args)
        {
            List<Value> values;
            if (_params != null && _params.Length > 0)
            {
                // 名前付き引数を考慮して引数をソートする
                values = new List<Value>(_params.Length);

                Dictionary<string, int> nameIndexMap = args
                    .Select((a, i) => (a.Name, i))
                    .Where(kv => !string.IsNullOrEmpty(kv.Name))
                    .ToDictionary(kv => kv.Name, kv => kv.i);
                if (nameIndexMap.Count <= 0)
                {
                    // 名前付き引数なしなので順番にれるだけ
                    for (int i = 0; i < _params.Length; ++i)
                    {
                        if (i < args.Count)
                        {
                            values.Add(args[i].Value?.Invoke() ?? default);
                        }
                        else
                        {
                            // デフォルト引数
                            values.Add(_params[i].Value?.Invoke() ?? default);
                        }
                    }
                }
                else
                {
                    // 1つでも名前付き引数がある
                    HashSet<int> useMap = args
                        .Select((a, i) => i)
                        .ToHashSet();

                    for (int i = 0; i < _params.Length; ++i)
                    {
                        int useIndex = -1;
                        if (nameIndexMap.TryGetValue(_params[i].Name ?? string.Empty, out int argIndex))
                        {
                            values.Add(args[argIndex].Value?.Invoke() ?? default);
                            useIndex = argIndex;
                        }
                        else
                        {
                            foreach (var argIndex2 in useMap)
                            {
                                if (string.IsNullOrEmpty(args[argIndex2].Name) || args[argIndex2].Name == _params[i].Name)
                                {
                                    values.Add(args[argIndex2].Value?.Invoke() ?? default);
                                    useIndex = argIndex2;
                                    break;
                                }
                            }
                        }
                        if (useIndex >= 0)
                        {
                            useMap.Remove(useIndex);
                        }
                        else
                        {
                            // デフォルト引数
                            values.Add(_params[i].Value?.Invoke() ?? default);
                        }
                    }
                }
            }
            else
            {
                values = new();
            }
            return Invoke(values);
        }
        public Value Invoke(IReadOnlyList<Value> values)
        {
            return _func?.Invoke(values) ?? default;
        }
        public Function SetArgument(int index, Argument arg)
        {
            _params[index] = arg;
            return this;
        }

        public Function SetArgument(int index, string name, in Value defaultValue = default)
        {
            return SetArgument(index, new Argument(name, defaultValue));
        }
        public Function SetDefaultValue(int index, in Value value)
        {
            return SetArgument(index, new Argument(_params[index].Name, value));
        }
        public Function SetDefaultValue(int index, bool value)
        {
            return SetArgument(index, new Argument(_params[index].Name, value));
        }
        public Function SetDefaultValue(int index, int value)
        {
            return SetArgument(index, new Argument(_params[index].Name, value));
        }
        public Function SetDefaultValue(int index, float value)
        {
            return SetArgument(index, new Argument(_params[index].Name, value));
        }
        public Function SetDefaultValue(int index, string value)
        {
            return SetArgument(index, new Argument(_params[index].Name, value));
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("fn ");
            sb.Append(_name);
            sb.Append('(');
            for (int i = 0; i < _params.Length; ++i) 
            {
                if (i != 0)
                {
                    sb.Append(',');
                }
                sb.Append(_params[i].Name);
            }
            sb.Append(')');
            return sb.ToString();
        }
        internal IReadOnlyList<Argument> Parameters => _params;
        readonly string _name;
        readonly Func<IReadOnlyList<Value>, Value> _func;
        readonly Argument[] _params;
    }
}
