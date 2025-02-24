using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;

namespace Petit.Runtime
{
    /// <summary>
    /// 変数
    /// </summary>
    public readonly partial struct Value
        : IEquatable<Value>
    {
        public readonly static Value Invalid = default;
        public readonly static Value True = Value.Of(true);
        public readonly static Value False = Value.Of(false);
        public readonly static Value NaN = Value.Of(float.NaN);
        public readonly static Value Inf = Value.Of(float.PositiveInfinity);

        /// <summary>
        /// 無効値か
        /// </summary>
        public bool IsInvalid => _type == ValueType.Invalid;

        /// <summary>
        /// bool型か
        /// </summary>
        public bool IsBool => _type == ValueType.Bool;

        /// <summary>
        /// int型か
        /// </summary>
        public bool IsInt => _type == ValueType.Int;

        /// <summary>
        /// Float型か
        /// </summary>
        public bool IsFloat => _type == ValueType.Float;

        /// <summary>
        /// string型か
        /// </summary>
        public bool IsString => _type == ValueType.String;

        /// <summary>
        /// Array型か
        /// </summary>
        public bool IsArray => _type == ValueType.Array;

        /// <summary>
        /// 関数型か
        /// </summary>
        public bool IsFunction => _type == ValueType.Function;

        /// <summary>
        /// NaNか
        /// </summary>
        public bool IsNaN => _type == ValueType.Float && float.IsNaN(_value.FloatValue);

        /// <summary>
        /// Infか
        /// </summary>
        public bool IsInf => _type == ValueType.Float && float.IsInfinity(_value.FloatValue);

        public readonly bool ToBool()
        {
            switch (_type)
            {
                case ValueType.Bool:
                    return _value.BoolValue;
                case ValueType.Int:
                    return _value.IntValue != 0;
                case ValueType.Float:
                    return _value.FloatValue != 0;
                case ValueType.String:
                    return !string.IsNullOrEmpty(_value.StringValue);
                case ValueType.Array:
                    return _reference.ArrayValue.Count > 0;
                case ValueType.Function:
                    return _reference.FuncValue.Invoke().ToBool();
            }
            return false;
        }
        public readonly int ToInt()
        {
            switch (_type)
            {
                case ValueType.Bool:
                    return _value.BoolValue ? 1 : 0;
                case ValueType.Int:
                    return _value.IntValue;
                case ValueType.Float:
                    return (int)_value.FloatValue;
                case ValueType.String:
                    if (float.TryParse(_value.StringValue, out float strParseValue))
                    {
                        return (int)strParseValue;
                    }
                    break;
                case ValueType.Array:
                    if (_reference.ArrayValue.Count == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return _reference.ArrayValue[0].ToInt();
                    }
                case ValueType.Function:
                    return _reference.FuncValue.Invoke().ToInt();
            }
            return 0;
        }
        public readonly float ToFloat()
        {
            switch (_type)
            {
                case ValueType.Bool:
                    return _value.BoolValue ? 1.0f : 0.0f;
                case ValueType.Int:
                    return (float)_value.IntValue;
                case ValueType.Float:
                    return _value.FloatValue;
                case ValueType.String:
                    if (float.TryParse(_value.StringValue, out float strParseValue))
                    {
                        return strParseValue;
                    }
                    break;
                case ValueType.Array:
                    if (_reference.ArrayValue.Count == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return _reference.ArrayValue[0].ToFloat();
                    }
                case ValueType.Function:
                    return _reference.FuncValue.Invoke().ToFloat();
            }
            return 0;
        }
        public override string ToString()
        {
            const string trueStr = "true";
            const string falseStr = "false";
            switch (_type)
            {
                case ValueType.Bool:
                    return _value.BoolValue ? trueStr : falseStr;
                case ValueType.Int:
                    return _value.IntValue.ToString();
                case ValueType.Float:
                    return _value.FloatValue.ToString();
                case ValueType.String:
                    return _value.StringValue;
                case ValueType.Array:
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append('[');
                        bool isFirst = true;
                        foreach (Value item in _reference.ArrayValue)
                        {
                            if (!isFirst)
                            {
                                sb.Append(", ");
                            }
                            sb.Append(item.ToString());
                            isFirst = false;
                        }
                        sb.Append(']');
                        return sb.ToString();
                    }
                case ValueType.Function:
                    return _reference.FuncValue.Invoke().ToString();
            }
            return string.Empty;
        }
        public List<Value> ToArray()
        {
            switch (_type)
            {
                case ValueType.Bool:
                    return new List<Value>() { this };
                case ValueType.Int:
                    return new List<Value>() { this };
                case ValueType.Float:
                    return new List<Value>() { this };
                case ValueType.String:
                    return _value.StringValue.Select(x => Value.Of(x)).ToList();
                case ValueType.Array:
                    return _reference.ArrayValue;
                case ValueType.Function:
                    return _reference.FuncValue.Invoke().ToArray();
            }
            return new List<Value>();
        }

        public Function ToFunction()
        {
            switch (_type)
            {
                case ValueType.Function:
                    return _reference.FuncValue;
            }
            return Function.Empty;
        }

        public static explicit operator bool(in Value v) => v.ToBool();
        public static explicit operator int(in Value v) => v.ToInt();
        public static explicit operator float(in Value v) => v.ToFloat();
        public static explicit operator string(in Value v) => v.ToString();
      
        public Value this [in Value i]
        {
            get
            {
                int index = i.ToInt();
                return this[index];
            }
            set
            {
                int index = i.ToInt();
                this[index] = value;
            }
        }
        public Value this[int i]
        {
            get
            {
                if (IsArray)
                {
                    if (i < _reference.ArrayValue.Count)
                    {
                        return _reference.ArrayValue[i];
                    }
                }
                else if (IsString)
                {
                    if (i < _value.StringValue.Length)
                    {
                        return Value.Of(_value.StringValue[i]);
                    }
                }
                return Value.Invalid;
            }
            set
            {
                if (IsArray)
                {
                    if (i < _reference.ArrayValue.Count)
                    {
                        _reference.ArrayValue[i] = value;
                    }
                }
            }
        }
        public override int GetHashCode()
        {
            switch (_type)
            {
                case ValueType.Bool:
                    return _value.BoolValue.GetHashCode();
                case ValueType.Int:
                    return _value.IntValue.GetHashCode();
                case ValueType.Float:
                    return _value.FloatValue.GetHashCode();
                case ValueType.String:
                    return _value.StringValue.GetHashCode();
                case ValueType.Array:
                    return ToString().GetHashCode();
                case ValueType.Function:
                    return _reference.FuncValue.GetHashCode();
            }
            return 0;
        }
       
        enum ValueType
        {
            Invalid,
            Bool,
            Int,
            Float,
            String,
            Array,
            Function,
        }

        [StructLayout(LayoutKind.Explicit)]
        struct Primitive
        {
            [FieldOffset(0)]
            public bool BoolValue;
            [FieldOffset(0)]
            public int IntValue;
            [FieldOffset(0)]
            public float FloatValue;
            [FieldOffset(0)]
            public string StringValue;
        }
        [StructLayout(LayoutKind.Explicit)]
        struct Reference
        {
            [FieldOffset(0)]
            public List<Value> ArrayValue;

            [FieldOffset(0)]
            public Function FuncValue;
        }
        private readonly ValueType _type;
        private readonly Primitive _value;
        private readonly Reference _reference;
    }
}
