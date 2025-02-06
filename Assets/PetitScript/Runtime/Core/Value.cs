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
        public readonly static Value NaN = new Value("NaN");

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
            type = ValueType.Bool;
            value = new ValueVariant();
            value.BoolValue = b;
        }
        public Value(int i)
        {
            type = ValueType.Int;
            value = new ValueVariant();
            value.IntValue = i;
        }
        public Value(float f)
        {
            type = ValueType.Float;
            value = new ValueVariant();
            value.FloatValue = f;
        }
        public Value(string s)
        {
            type = ValueType.String;
            value = new ValueVariant();
            value.StringValue = s;
        }

        /// <summary>
        /// 無効値か
        /// </summary>
        public bool IsInvalid => type == ValueType.Invalid;

        /// <summary>
        /// bool型か
        /// </summary>
        public bool IsBool => type == ValueType.Bool;

        /// <summary>
        /// int型か
        /// </summary>
        public bool IsInt => type == ValueType.Int;

        /// <summary>
        /// Float型か
        /// </summary>
        public bool IsFloat => type == ValueType.Float;

        /// <summary>
        /// string型か
        /// </summary>
        public bool IsString => type == ValueType.String;

        public readonly bool ToBool()
        {
            switch (type)
            {
                case ValueType.Bool:
                    return value.BoolValue;
                case ValueType.Int:
                    return value.IntValue != 0;
                case ValueType.Float:
                    return value.FloatValue != 0;
                case ValueType.String:
                    return !string.IsNullOrEmpty(value.StringValue);
            }
            return false;
        }
        public readonly int ToInt()
        {
            switch (type)
            {
                case ValueType.Bool:
                    return value.BoolValue ? 1 : 0;
                case ValueType.Int:
                    return value.IntValue;
                case ValueType.Float:
                    return (int)value.FloatValue;
                case ValueType.String:
                    if (float.TryParse(value.StringValue, out float strParseValue))
                    {
                        return (int)strParseValue;
                    }
                    break;
            }
            return 0;
        }
        public readonly float ToFloat()
        {
            switch (type)
            {
                case ValueType.Bool:
                    return value.BoolValue ? 1.0f : 0.0f;
                case ValueType.Int:
                    return (float)value.IntValue;
                case ValueType.Float:
                    return value.FloatValue;
                case ValueType.String:
                    if (float.TryParse(value.StringValue, out float strParseValue))
                    {
                        return strParseValue;
                    }
                    break;
            }
            return 0;
        }
        public override string ToString()
        {
            const string trueStr = "true";
            const string falseStr = "false";

            switch (type)
            {
                case ValueType.Bool:
                    return value.BoolValue ? trueStr : falseStr;
                case ValueType.Int:
                    return value.IntValue.ToString();
                case ValueType.Float:
                    return value.FloatValue.ToString();
                case ValueType.String:
                    return value.StringValue;
            }
            return string.Empty;
        }
        public bool Equals(Value other)
        {
            if (type != other.type)
            {
                return false;
            }
            switch (type)
            {
                case ValueType.Bool:
                    return value.BoolValue == other.value.BoolValue;
                case ValueType.Int:
                    return value.IntValue == other.value.IntValue;
                case ValueType.Float:
                    return value.FloatValue == other.value.FloatValue;
                case ValueType.String:
                    return value.StringValue == other.value.StringValue;
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
            switch (type)
            {
                case ValueType.Bool:
                    return value.BoolValue == other.ToBool();
                case ValueType.Int:
                    return value.IntValue == other.ToInt();
                case ValueType.Float:
                    return value.FloatValue == other.ToFloat();
                case ValueType.String:
                    return value.StringValue == other.ToString();
            }
            return type == other.type;
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
        public override int GetHashCode()
        {
            switch (type)
            {
                case ValueType.Bool:
                    return value.BoolValue.GetHashCode();
                case ValueType.Int:
                    return value.IntValue.GetHashCode();
                case ValueType.Float:
                    return value.FloatValue.GetHashCode();
                case ValueType.String:
                    return value.StringValue.GetHashCode();
            }
            return 0;
        }

        public static explicit operator bool(in Value v) => v.ToBool();
        public static explicit operator int(in Value v) => v.ToInt();
        public static explicit operator float(in Value v) => v.ToFloat();
        public static explicit operator string(in Value v) => v.ToString();

        public static bool operator ==(in Value a, in Value b)
        {
            return Compare(a, b) == 0;
        }
        public static bool operator !=(in Value a, in Value b)
        {
            return Compare(a, b) != 0;
        }

        public static bool operator true(in Value v) => v.ToBool();
        public static bool operator false(in Value v) => !v.ToBool();
        public static Value operator &(in Value a, in Value b)
        {
            if (a.IsInt || b.IsInt)
            {
                return new Value(a.ToInt() & b.ToInt());
            }
            return new Value(a.ToBool() & b.ToBool());
        }
        public static Value operator |(in Value a, in Value b)
        {
            if (a.IsInt || b.IsInt)
            {
                return new Value(a.ToInt() | b.ToInt());
            }
            return new Value(a.ToBool() | b.ToBool());
        }
        public static int Compare(in Value a, in Value b)
        {
            if (a.IsBool && b.IsBool)
            {
                return a.value.BoolValue.CompareTo(b.value.BoolValue);
            }
            if (a.IsInt && b.IsInt)
            {
                return a.value.IntValue.CompareTo(b.value.IntValue);
            }
            if (a.IsFloat && b.IsFloat)
            {
                return a.value.FloatValue.CompareTo(b.value.FloatValue);
            }
            if (a.IsString && b.IsString)
            {
                return a.value.StringValue.CompareTo(b.value.StringValue);
            }
            bool stringCompare = false;
            float aValue = 0.0f;
            float bValue = 0.0f;
            if (a.IsString)
            {
                if (bool.TryParse(a.value.StringValue, out bool bo))
                {
                    aValue = bo ? 1.0f : 0.0f;
                }
                else if (float.TryParse(a.value.StringValue, out float f))
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
                if (bool.TryParse(b.value.StringValue, out bool bo))
                {
                    bValue = bo ? 1.0f : 0.0f;
                }
                else if (float.TryParse(b.value.StringValue, out float f))
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
            return Compare(a, b) > 0;
        }
        public static bool operator <(in Value a, in Value b)
        {
            return Compare(a, b) < 0;
        }
        public static bool operator >=(in Value a, in Value b)
        {
            return Compare(a, b) >= 0;
        }
        public static bool operator <=(in Value a, in Value b)
        {
            return Compare(a, b) <= 0;
        }
        public static Value operator !(in Value a)
        {
            return new Value(!a.ToBool());
        }
        public static Value operator +(in Value a)
        {
            return a;
        }
        public static Value operator -(in Value a)
        {
            switch (a.type)
            {
                case ValueType.Bool:
                    return new Value(-a.ToInt());
                case ValueType.Int:
                    return new Value(-a.ToInt());
                case ValueType.Float:
                    return new Value(-a.ToFloat());
                case ValueType.String:
                    if (bool.TryParse(a.value.StringValue, out bool bo))
                    {
                        return new Value(bo ? -1 : 0);
                    }
                    else if (int.TryParse(a.value.StringValue, out int i))
                    {
                        return new Value(-i);
                    }
                    else if (float.TryParse(a.value.StringValue, out float f))
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
        private static ValueType OpValueGet(
            in Value a,
            in Value b,
            out int aiValue,
            out int biValue,
            out float afValue,
            out float bfValue
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
            if (a.IsString)
            {
                if (bool.TryParse(a.value.StringValue, out bool bo))
                {
                    aiValue = bo ? 1 : 0;
                    afValue = aiValue;
                }
                else if (int.TryParse(a.value.StringValue, out int i))
                {
                    aiValue = i;
                    afValue = aiValue;
                }
                else if (float.TryParse(a.value.StringValue, out float f))
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
                if (bool.TryParse(b.value.StringValue, out bool bo))
                {
                    biValue = bo ? 1 : 0;
                    bfValue = biValue;
                }
                else if (int.TryParse(b.value.StringValue, out int i))
                {
                    biValue = i;
                    bfValue = biValue;
                }
                else if (float.TryParse(b.value.StringValue, out float f))
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
        public static Value operator +(in Value a, in Value b)
        {
            ValueType opType = OpValueGet(a, b,
                out int aiValue,
                out int biValue,
                out float afValue,
                out float bfValue
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
            return Value.Invalid;
        }
        enum ValueType
        {
            Invalid,
            Bool,
            Int,
            Float,
            String,
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

        private readonly ValueType type;
        private readonly ValueVariant value;
    }
}
