using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Petit.Runtime
{
    /// <summary>
    /// Value
    /// 生成に関する処理
    /// </summary>
    public readonly partial struct Value
    {
        /// <summary>
        /// 文字列からパース
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Value Parse(string str)
        {
            if (str is null)
            {
                return Value.Invalid;
            }
            if (bool.TryParse(str, out bool b))
            {
                return Value.Of(b);
            }
            if (int.TryParse(str, NumberStyles.Integer, CultureInfo.InvariantCulture, out int i))
            {
                return Value.Of(i);
            }
            if (float.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out float f))
            {
                return Value.Of(f);
            }
            return Value.Of(str);
        }
        public static Value Of(bool b)
        {
            return new Value(b);
        }
        public static Value Of(int i)
        {
            return new Value(i);
        }
        public static Value Of(float f)
        {
            return new Value(f);
        }
        public static Value Of(string str)
        {
            return new Value(str);
        }
        public static Value Of(char c)
        {
            return new Value(c);
        }
        public static Value Of(Function func)
        {
            return new Value(func);
        }
        public static Value Of(IEnumerable enumerable)
        {
            return new Value(enumerable);
        }
        public static Value Of(IEnumerable<Value> enumerable)
        {
            return new Value(enumerable);
        }
        public static Value Of<T>(IEnumerable<T> enumerable)
        {
            return new Value(enumerable.Select(v => Value.Of(v)));
        }
        public static Value Of(IEnumerable<object> enumerable)
        {
            return new Value(enumerable);
        }
        public static Value ArrayOf()
        {
            return Value.Of(new List<Value>());
        }
        public static Value ArrayOf(params Value[] values)
        {
            return Value.Of(values);
        }
        public static Value ArrayOf<T>(params T[] values)
        {
            return Value.Of(values);
        }
        public static Value ArrayOf(params object[] values)
        {
            return Value.Of(values);
        }
        public static Value Of(object o)
        {
            if (o is Value v)
            {
                return v;
            }
            else if(o is bool b)
            {
                return Value.Of(b);
            }
            else if (o is int i)
            {
                return Value.Of(i);
            }
            else if (o is float f)
            {
                return Value.Of(f);
            }
            else if (o is string s)
            {
                return Value.Of(s);
            }
            else if (o is char c)
            {
                return Value.Of(c);
            }
            else if (o is Function func)
            {
                return Value.Of(func);
            }
            else if (o is IEnumerable enumerable)
            {
                return Value.Of(enumerable);
            }
            else
            {
                return default;
            }
        }
        public static implicit operator Value(bool v) => Value.Of(v);
        public static implicit operator Value(int v) => Value.Of(v);
        public static implicit operator Value(float v) => Value.Of(v);
        public static implicit operator Value(string v) => Value.Of(v);
        public static implicit operator Value(char v) => Value.Of(v);
        public static implicit operator Value(Function v) => Value.Of(v);

        Value(bool b)
        {
            _type = ValueType.Bool;
            _value = default;
            _value.BoolValue = b;
            _reference = default;
        }
        Value(int i)
        {
            _type = ValueType.Int;
            _value = default;
            _value.IntValue = i;
            _reference = default;
        }
        Value(float f)
        {
            _type = ValueType.Float;
            _value = default;
            _value.FloatValue = f;
            _reference = default;
        }
        Value(string s)
        {
            _type = ValueType.String;
            _value = default;
            _value.StringValue = s;
            _reference = default;
        }
        Value(char c)
            :this(c.ToString())
        {
        }
        Value(Function func)
        {
            _type = ValueType.Function;
            _value = default;
            _reference = default;
            _reference.FuncValue = func;
        }
        Value(IEnumerable<Value> collection)
        {
            _type = ValueType.Array;
            _value = default;
            _reference = default;
            _reference.ArrayValue = collection.ToList();
        }
        Value(IEnumerable<object> collection)
            :this(collection.Select(Value.Of))
        {
        }
        Value(IEnumerable collection)
        {
            _type = ValueType.Array;
            _value = default;
            _reference = default;
            _reference.ArrayValue = new List<Value>();
            foreach (object item in collection)
            {
                _reference.ArrayValue.Add(Value.Of(item));
            }
        }
    }
}
