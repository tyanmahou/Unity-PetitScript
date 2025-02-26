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
        public bool IsInvalid => _type == ValueType.Invalid
            || _type == ValueType.Reference && _value.Reference.Indirection.IsInvalid;

        /// <summary>
        /// bool型か
        /// </summary>
        public bool IsBool => _type == ValueType.Bool
            || _type == ValueType.Reference && _value.Reference.Indirection.IsBool;


        /// <summary>
        /// int型か
        /// </summary>
        public bool IsInt => _type == ValueType.Int
            || _type == ValueType.Reference && _value.Reference.Indirection.IsInt;

        /// <summary>
        /// Float型か
        /// </summary>
        public bool IsFloat => _type == ValueType.Float
            || _type == ValueType.Reference && _value.Reference.Indirection.IsFloat;

        /// <summary>
        /// string型か
        /// </summary>
        public bool IsString => _type == ValueType.String
            || _type == ValueType.Reference && _value.Reference.Indirection.IsString;

        /// <summary>
        /// Array型か
        /// </summary>
        public bool IsArray => _type == ValueType.Array
            || _type == ValueType.Reference && _value.Reference.Indirection.IsArray;

        /// <summary>
        /// 関数型か
        /// </summary>
        public bool IsFunction => _type == ValueType.Function
            || _type == ValueType.Reference && _value.Reference.Indirection.IsFunction;

        /// <summary>
        /// 参照型か
        /// </summary>
        public bool IsReference => _type == ValueType.Reference;

        /// <summary>
        /// NaNか
        /// </summary>
        public bool IsNaN => _type == ValueType.Float && float.IsNaN(_value.FloatValue)
            || _type == ValueType.Reference && _value.Reference.Indirection.IsNaN;

        /// <summary>
        /// Infか
        /// </summary>
        public bool IsInf => _type == ValueType.Float && float.IsInfinity(_value.FloatValue)
            || _type == ValueType.Reference && _value.Reference.Indirection.IsInf;

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
                    return _value.ArrayValue.Count > 0;
                case ValueType.Function:
                    return _value.FuncValue.Invoke().ToBool();
                case ValueType.Reference:
                    return _value.Reference.Indirection.ToBool();
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
                    if (_value.ArrayValue.Count == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return _value.ArrayValue[0].ToInt();
                    }
                case ValueType.Function:
                    return _value.FuncValue.Invoke().ToInt();
                case ValueType.Reference:
                    return _value.Reference.Indirection.ToInt();
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
                    if (_value.ArrayValue.Count == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return _value.ArrayValue[0].ToFloat();
                    }
                case ValueType.Function:
                    return _value.FuncValue.Invoke().ToFloat();
                case ValueType.Reference:
                    return _value.Reference.Indirection.ToFloat();
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
                        foreach (Value item in _value.ArrayValue)
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
                    return _value.FuncValue.Invoke().ToString();
                case ValueType.Reference:
                    return _value.Reference.Indirection.ToString();
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
                    return _value.ArrayValue;
                case ValueType.Function:
                    return _value.FuncValue.Invoke().ToArray();
                case ValueType.Reference:
                    return _value.Reference.Indirection.ToArray();
            }
            return new List<Value>();
        }

        public Function ToFunction()
        {
            switch (_type)
            {
                case ValueType.Array:
                    if (_value.ArrayValue.Count == 0)
                    {
                        return Function.Empty;
                    }
                    else
                    {
                        return _value.ArrayValue[0].ToFunction();
                    }
                case ValueType.Function:
                    return _value.FuncValue;
                case ValueType.Reference:
                    return _value.Reference.Indirection.ToFunction();
            }
            return Function.Empty;
        }
        public Reference ToReference()
        {
            switch (_type)
            {
                case ValueType.Reference:
                    return _value.Reference;
            }
            return new Reference(this);
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
                if (IsReference)
                {
                    return _value.Reference.Indirection[i];
                }
                else if (IsArray)
                {
                    if (i < _value.ArrayValue.Count)
                    {
                        return _value.ArrayValue[i];
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
                if (IsReference)
                {
                    Value ind = _value.Reference.Indirection;
                    ind[i] = value;
                }
                else if (IsArray)
                {
                    if (i < _value.ArrayValue.Count)
                    {
                        var v = _value.ArrayValue[i];
                        if (v.IsReference)
                        {
                            v.ToReference().Indirection = value;
                        }
                        else
                        {
                            _value.ArrayValue[i] = v;
                        }
                    }
                }
            }
        }
        public Value SetIndirect(in Value value)
        {
            if (IsReference)
            {
                _value.Reference.Indirection = value;
            }
            return this;
        }
        public bool TrySetIndirect(in Value value)
        {
            if (IsReference)
            {
                _value.Reference.Indirection = value;
                return true;
            }
            return false;
        }
        public Value Copy()
        {
            if (IsReference)
            {
                return _value.Reference.Indirection.Copy();
            }
            return this;
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
                    return _value.FuncValue.GetHashCode();
                case ValueType.Reference:
                    return _value.Reference.Indirection.GetHashCode();
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
            Reference,
        }

        [StructLayout(LayoutKind.Explicit)]
        struct Variant
        {
            [FieldOffset(0)]
            public bool BoolValue;
            [FieldOffset(0)]
            public int IntValue;
            [FieldOffset(0)]
            public float FloatValue;
            [FieldOffset(0)]
            public string StringValue;

            // class
            [FieldOffset(8)]
            public List<Value> ArrayValue;
            [FieldOffset(8)]
            public Function FuncValue;
            [FieldOffset(8)]
            public Reference Reference;
        }
        private readonly ValueType _type;
        private readonly Variant _value;
    }
}
