using System;
using System.Collections.Generic;
using System.Linq;

namespace Petit.Core
{
    public readonly struct Argument
    {
        public Argument(string name, Value value = default)
        {
            Name = name;
            Value = value;
        }
        public readonly string Name;
        public readonly Value Value;
    }
    /// <summary>
    /// 関数
    /// </summary>
    public partial class Function
    {
        public Function(Func<IReadOnlyList<Value>, Value> func, params Argument[] parameters)
        {
            _func = func;
            _params = parameters;
        }
        public Value Invoke(IReadOnlyList<Argument> args)
        {
            List<Value> values;
            if (_params != null && _params.Count > 0)
            {
                // 名前付き引数を考慮して引数をソートする
                values = new List<Value>(_params.Count);

                Dictionary<string, int> nameIndexMap = args
                    .Select((a, i) => (a.Name, i))
                    .Where(kv => !string.IsNullOrEmpty(kv.Name))
                    .ToDictionary(kv => kv.Name, kv => kv.i);
                if (nameIndexMap.Count <= 0)
                {
                    // 名前付き引数なしなので順番にれるだけ
                    for (int i = 0; i < _params.Count; ++i)
                    {
                        if (i < args.Count)
                        {
                            values.Add(args[i].Value);
                        }
                        else
                        {
                            // デフォルト引数
                            values.Add(_params[i].Value);
                        }
                    }
                }
                else 
                {
                    // 1つでも名前付き引数がある
                    HashSet<int> useMap = args
                        .Select((a, i) => i)
                        .ToHashSet();

                    for (int i = 0; i < _params.Count; ++i)
                    {
                        int useIndex = -1;
                        if (nameIndexMap.TryGetValue(_params[i].Name ?? string.Empty, out int argIndex))
                        {
                            values.Add(args[argIndex].Value);
                            useIndex = argIndex;
                        }
                        else
                        {
                            foreach (var argIndex2 in useMap)
                            {
                                if (string.IsNullOrEmpty(args[argIndex2].Name) || args[argIndex2].Name == _params[i].Name)
                                {
                                    values.Add(args[argIndex2].Value);
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
                            values.Add(_params[i].Value);
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
        readonly Func<IReadOnlyList<Value>, Value> _func;
        readonly IReadOnlyList<Argument> _params;
    }
}
