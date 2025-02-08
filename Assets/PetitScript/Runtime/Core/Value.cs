using System.Runtime.InteropServices;
using System;

namespace Petit.Core
{
    /// <summary>
    /// 変数
    /// </summary>
    public readonly struct Value
        : IEquatable<Value>
        , IComparable<Value>
    {
        public readonly static Value Invalid = default;
        public readonly static Value True = new Value(true);
        public readonly static Value False = new Value(false);
        public readonly static Value NaN = new Value(float.NaN);

        /// <summary>
        /// 文字列からパース
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Value Parse(string str)
        {
            if (str is null)
            {
                return new Value();
            }
            if (bool.TryParse(str, out bool b))
            {
                return new Value(b);
            }
            if (int.TryParse(str, out int i))
            {
                return new Value(i);
            }
            if (float.TryParse(str, out float f))
            {
                return new Value(f);
            }
            return new Value(str);
        }
        public Value(bool b)
        {
            _type = ValueType.Bool;
            _value = new ValueVariant();
            _value.BoolValue = b;
        }
        public Value(int i)
        {
            _type = ValueType.Int;
            _value = new ValueVariant();
            _value.IntValue = i;
        }
        public Value(float f)
        {
            if (!float.IsNaN(f))
            {
                _type = ValueType.Float;
                _value = new ValueVariant();
                _value.FloatValue = f;
            }
            else
            {
                _type = ValueType.NaN;
                _value = new ValueVariant();
                _value.FloatValue = float.NaN;
            }
        }
        public Value(string s)
        {
            _type = ValueType.String;
            _value = new ValueVariant();
            _value.StringValue = s;
        }
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
        /// NaNか
        /// </summary>
        public bool IsNaN => _type == ValueType.NaN;
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
                case ValueType.NaN:
                    return float.NaN;
            }
            return 0;
        }
        public override string ToString()
        {
            const string trueStr = "true";
            const string falseStr = "false";
            const string NaNStr = "NaN";
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
                case ValueType.NaN:
                    return NaNStr;
            }
            return string.Empty;
        }

        public static explicit operator bool(in Value v) => v.ToBool();
        public static explicit operator int(in Value v) => v.ToInt();
        public static explicit operator float(in Value v) => v.ToFloat();
        public static explicit operator string(in Value v) => v.ToString();
        public static bool operator true(in Value v) => v.ToBool();
        public static bool operator false(in Value v) => !v.ToBool();

        public bool Equals(Value other)
        {
            if (_type != other._type)
            {
                return false;
            }
            if (this.IsNaN || other.IsNaN)
            {
                return false;
            }
            switch (_type)
            {
                case ValueType.Bool:
                    return _value.BoolValue == other._value.BoolValue;
                case ValueType.Int:
                    return _value.IntValue == other._value.IntValue;
                case ValueType.Float:
                    return _value.FloatValue == other._value.FloatValue;
                case ValueType.String:
                    return _value.StringValue == other._value.StringValue;
            }
            return true;
        }
        public override bool Equals(object other)
        {
            if (other is Value v)
            {
                return Equals(v);
            }
            else if (other is int i)
            {
                return Equals(new Value(i));
            }
            else if (other is float f)
            {
                return Equals(new Value(f));
            }
            else if (other is string s)
            {
                return Equals(new Value(s));
            }
            else if (other is bool b)
            {
                return Equals(new Value(b));
            }
            else if (other is null)
            {
                return Equals(Invalid);
            }
            return false;
        }

        /// <summary>
        /// 同値判定
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Identical(in Value other) => Equals(other);
        public static bool Identical(in Value a, in Value b)
        {
            return a.Identical(b);
        }

        /// <summary>
        /// 非同値判定
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool NotIdentical(in Value other) => !Equals(other);
        public static bool NotIdentical(in Value a, in Value b)
        {
            return a.NotIdentical(b);
        }

        /// <summary>
        /// 単方向緩い一致判定
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool EqualsLooseSingly(in Value other)
        {
            if (this.IsNaN || other.IsNaN)
            {
                return false;
            }
            switch (_type)
            {
                case ValueType.Bool:
                    return _value.BoolValue == other.ToBool();
                case ValueType.Int:
                    return _value.IntValue == other.ToInt();
                case ValueType.Float:
                    return _value.FloatValue == other.ToFloat();
                case ValueType.String:
                    return _value.StringValue == other.ToString();
            }
            return _type == other._type;
        }

        /// <summary>
        /// 双方向の緩い一致判定
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool EqualsLoose(in Value other)
        {
            return this == other;
        }
        public static bool EqualsLoose(in Value a, in Value b)
        {
            return a.EqualsLoose(b);
        }

        public static bool operator ==(in Value a, in Value b)
        {
            if (a.IsNaN || b.IsNaN)
            {
                return false;
            }
            return Compare(a, b) == 0;
        }
        public static bool operator !=(in Value a, in Value b)
        {
            if (a.IsNaN || b.IsNaN)
            {
                return true;
            }
            return Compare(a, b) != 0;
        }
        public static int Compare(in Value a, in Value b)
        {
            if (a.IsNaN && b.IsNaN)
            {
                return 0;
            }
            else if (a.IsNaN)
            {
                return -1;
            }
            else if(b.IsNaN)
            {
                return 1;
            }
            if (a.IsBool && b.IsBool)
            {
                return a._value.BoolValue.CompareTo(b._value.BoolValue);
            }
            if (a.IsInt && b.IsInt)
            {
                return a._value.IntValue.CompareTo(b._value.IntValue);
            }
            if (a.IsFloat && b.IsFloat)
            {
                return a._value.FloatValue.CompareTo(b._value.FloatValue);
            }
            if (a.IsString && b.IsString)
            {
                return a._value.StringValue.CompareTo(b._value.StringValue);
            }
            bool stringCompare = false;
            float aValue = 0.0f;
            float bValue = 0.0f;
            if (a.IsString)
            {
                if (bool.TryParse(a._value.StringValue, out bool bo))
                {
                    aValue = bo ? 1.0f : 0.0f;
                }
                else if (float.TryParse(a._value.StringValue, out float f))
                {
                    aValue = f;
                }
                else
                {
                    stringCompare = true;
                }
            }
            else
            {
                aValue = a.ToFloat();
            }
            if (b.IsString)
            {
                if (bool.TryParse(b._value.StringValue, out bool bo))
                {
                    bValue = bo ? 1.0f : 0.0f;
                }
                else if (float.TryParse(b._value.StringValue, out float f))
                {
                    bValue = f;
                }
                else
                {
                    stringCompare = true;
                }
            }
            else
            {
                bValue = b.ToFloat();
            }
            if (stringCompare)
            {
                return a.ToString().CompareTo(b.ToString());
            }
            return aValue.CompareTo(bValue);
        }
        public int CompareTo(Value other)
        {
            return Compare(this, other);
        }
        public static bool operator >(in Value a, in Value b)
        {
            if (a.IsNaN || b.IsNaN)
            {
                return false;
            }
            return Compare(a, b) > 0;
        }
        public static bool operator <(in Value a, in Value b)
        {
            if (a.IsNaN || b.IsNaN)
            {
                return false;
            }
            return Compare(a, b) < 0;
        }
        public static bool operator >=(in Value a, in Value b)
        {
            if (a.IsNaN || b.IsNaN)
            {
                return false;
            }
            return Compare(a, b) >= 0;
        }
        public static bool operator <=(in Value a, in Value b)
        {
            if (a.IsNaN || b.IsNaN)
            {
                return false;
            }
            return Compare(a, b) <= 0;
        }
        public static Value operator !(in Value a)
        {
            return new Value(!a.ToBool());
        }
        public static Value operator +(in Value a)
        {
            switch (a._type)
            {
                case ValueType.Bool:
                    return new Value(a.ToInt());
                case ValueType.Int:
                    return a;
                case ValueType.Float:
                    return a;
                case ValueType.String:
                    if (bool.TryParse(a._value.StringValue, out bool bo))
                    {
                        return new Value(bo ? 1 : 0);
                    }
                    else if (int.TryParse(a._value.StringValue, out int i))
                    {
                        return new Value(i);
                    }
                    else if (float.TryParse(a._value.StringValue, out float f))
                    {
                        return new Value(f);
                    }
                    else
                    {
                        return Value.NaN;
                    }
            }
            return a;
        }
        public static Value operator -(in Value a)
        {
            switch (a._type)
            {
                case ValueType.Bool:
                    return new Value(-a.ToInt());
                case ValueType.Int:
                    return new Value(-a.ToInt());
                case ValueType.Float:
                    return new Value(-a.ToFloat());
                case ValueType.String:
                    if (bool.TryParse(a._value.StringValue, out bool bo))
                    {
                        return new Value(bo ? -1 : 0);
                    }
                    else if (int.TryParse(a._value.StringValue, out int i))
                    {
                        return new Value(-i);
                    }
                    else if (float.TryParse(a._value.StringValue, out float f))
                    {
                        return new Value(-f);
                    }
                    else
                    {
                        return Value.NaN;
                    }
            }
            return a;
        }
        public static Value operator +(in Value a, in Value b)
        {
            ValueType opType = OpValueGet(a, b,
                out int aiValue,
                out int biValue,
                out float afValue,
                out float bfValue,
                prioritizeString: true
                );
            if (opType == ValueType.Int)
            {
                return new Value(aiValue + biValue);
            }
            else if (opType == ValueType.Float)
            {
                return new Value(afValue + bfValue);
            }
            else if (opType == ValueType.String)
            {
                return new Value(a.ToString() + b.ToString());
            }
            else if (opType == ValueType.NaN)
            {
                return Value.NaN;
            }
            return Value.Invalid;
        }
        public static Value operator -(in Value a, in Value b)
        {
            ValueType opType = OpValueGet(a, b,
                out int aiValue,
                out int biValue,
                out float afValue,
                out float bfValue
                );
            if (opType == ValueType.Int)
            {
                return new Value(aiValue - biValue);
            }
            else if (opType == ValueType.Float)
            {
                return new Value(afValue - bfValue);
            }
            else if (opType == ValueType.String)
            {
                return Value.NaN;
            }
            else if (opType == ValueType.NaN)
            {
                return Value.NaN;
            }
            return Value.Invalid;
        }
        public static Value operator *(in Value a, in Value b)
        {
            ValueType opType = OpValueGet(a, b,
                out int aiValue,
                out int biValue,
                out float afValue,
                out float bfValue
                );
            if (opType == ValueType.Int)
            {
                return new Value(aiValue * biValue);
            }
            else if (opType == ValueType.Float)
            {
                return new Value(afValue * bfValue);
            }
            else if (opType == ValueType.String)
            {
                return Value.NaN;
            }
            else if (opType == ValueType.NaN)
            {
                return Value.NaN;
            }
            return Value.Invalid;
        }
        public static Value operator /(in Value a, in Value b)
        {
            ValueType opType = OpValueGet(a, b,
                out int aiValue,
                out int biValue,
                out float afValue,
                out float bfValue
                );
            if (opType == ValueType.Int)
            {
                return new Value(aiValue / biValue);
            }
            else if (opType == ValueType.Float)
            {
                return new Value(afValue / bfValue);
            }
            else if (opType == ValueType.String)
            {
                return Value.NaN;
            }
            else if (opType == ValueType.NaN)
            {
                return Value.NaN;
            }
            return Value.Invalid;
        }
        public static Value operator %(in Value a, in Value b)
        {
            ValueType opType = OpValueGet(a, b,
                out int aiValue,
                out int biValue,
                out float afValue,
                out float bfValue
                );
            if (opType == ValueType.Int)
            {
                return new Value(aiValue % biValue);
            }
            else if (opType == ValueType.Float)
            {
                return new Value(afValue % bfValue);
            }
            else if (opType == ValueType.String)
            {
                return Value.NaN;
            }
            else if (opType == ValueType.NaN)
            {
                return Value.NaN;
            }
            return Value.Invalid;
        }
        public static Value operator ~(in Value a)
        {
            return new Value(~a.ToInt());
        }
        public static Value operator &(in Value a, in Value b)
        {
            return new Value(a.ToInt() & b.ToInt());
        }
        public static Value operator |(in Value a, in Value b)
        {
            return new Value(a.ToInt() | b.ToInt());
        }
        public static Value operator ^(in Value a, in Value b)
        {
            return new Value(a.ToInt() ^ b.ToInt());
        }
        public static Value operator <<(in Value a, int b)
        {
            return new Value(a.ToInt() << b);
        }
        public static Value operator >>(in Value a, int b)
        {
            return new Value(a.ToInt() >> b);
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
                case ValueType.NaN:
                    return float.NaN.GetHashCode();
            }
            return 0;
        }

        private static ValueType OpValueGet(
            in Value a,
            in Value b,
            out int aiValue,
            out int biValue,
            out float afValue,
            out float bfValue,
            bool prioritizeString = false
            )
        {
            bool stringOp = false;
            bool floatOp = false;
            aiValue = 0;
            biValue = 0;
            afValue = 0.0f;
            bfValue = 0.0f;

            if (a.IsInvalid && b.IsInvalid)
            {
                return ValueType.Invalid;
            }
            if (a.IsNaN || b.IsNaN)
            {
                return ValueType.NaN;
            }
            if (prioritizeString && (a.IsString || b.IsString))
            {
                // 文字列優先
                stringOp = true;
            }
            else if (a.IsInvalid && b.IsString)
            {
                // aだけ無効
                stringOp = true;
            }
            else if (a.IsString && b.IsInvalid)
            {
                // bだけ無効
                stringOp = true;
            }
            if (stringOp)
            {
                // 既に文字列結合ならパース処理スキップ
                return ValueType.String;
            }
            if (a.IsString)
            {
                if (bool.TryParse(a._value.StringValue, out bool bo))
                {
                    aiValue = bo ? 1 : 0;
                    afValue = aiValue;
                }
                else if (int.TryParse(a._value.StringValue, out int i))
                {
                    aiValue = i;
                    afValue = aiValue;
                }
                else if (float.TryParse(a._value.StringValue, out float f))
                {
                    floatOp = true;
                    afValue = f;
                }
                else
                {
                    stringOp = true;
                }
            }
            else if (a.IsFloat)
            {
                floatOp = true;
                afValue = a.ToFloat();
            }
            else
            {
                aiValue = a.ToInt();
                afValue = aiValue;
            }
            if (b.IsString)
            {
                if (bool.TryParse(b._value.StringValue, out bool bo))
                {
                    biValue = bo ? 1 : 0;
                    bfValue = biValue;
                }
                else if (int.TryParse(b._value.StringValue, out int i))
                {
                    biValue = i;
                    bfValue = biValue;
                }
                else if (float.TryParse(b._value.StringValue, out float f))
                {
                    floatOp = true;
                    bfValue = f;
                }
                else
                {
                    stringOp = true;
                }
            }
            else if (b.IsFloat)
            {
                floatOp = true;
                bfValue = b.ToFloat();
            }
            else
            {
                biValue = b.ToInt();
                bfValue = biValue;
            }
            if (stringOp)
            {
                return ValueType.String;
            }
            if (floatOp)
            {
                return ValueType.Float;
            }
            return ValueType.Int;
        }

        enum ValueType
        {
            Invalid,
            Bool,
            Int,
            Float,
            String,

            NaN
        }

        [StructLayout(LayoutKind.Explicit)]
        struct ValueVariant
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

        private readonly ValueType _type;
        private readonly ValueVariant _value;
    }
}
