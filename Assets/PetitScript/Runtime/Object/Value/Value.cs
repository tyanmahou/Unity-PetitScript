using System;
using System.Collections;
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
        , IComparable<Value>
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
                    return _array.Count > 0;
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
                    if (_array.Count == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return _array[0].ToInt();
                    }
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
                    if (_array.Count == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return _array[0].ToFloat();
                    }
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
                        foreach (Value item in _array)
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
            }
            return string.Empty;
        }
        public List<Value> ToList()=> ToArray();
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
                    return _array;
            }
            return new List<Value>();
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
                case ValueType.Array:
                    if (_array.Count != other._array.Count)
                    {
                        return false;
                    }
                    for (int index = 0; index <_array.Count; ++index)
                    {
                        if (!_array[index].Equals(other._array[index]))
                        {
                            return false;
                        }
                    }
                    return true;
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
                return Equals(Value.Of(i));
            }
            else if (other is float f)
            {
                return Equals(Value.Of(f));
            }
            else if (other is string s)
            {
                return Equals(Value.Of(s));
            }
            else if (other is bool b)
            {
                return Equals(Value.Of(b));
            }
            else if (other is IEnumerable enumrable)
            {
                return Equals(Value.Of(enumrable));
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
                case ValueType.Array:
                    var otherList = other.ToArray();
                    if (_array.Count != otherList.Count)
                    {
                        return false;
                    }
                    for (int index = 0; index <_array.Count; ++index)
                    {
                        if (!_array[index].EqualsLooseSingly(otherList[index]))
                        {
                            return false;
                        }
                    }
                    return true;
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
            if (a.IsArray && b.IsArray)
            {
                int min = Math.Min(a._array.Count, b._array.Count);
                for (int index = 0; index < min; ++index)
                {
                    int comp = a._array[index].CompareTo(b._array[index]);
                    if (comp != 0)
                    {
                        return comp;
                    }
                }
                return a._array.Count.CompareTo(b._array.Count);
            }
            if (a.IsArray && b.IsString)
            {
                return a.ToString().CompareTo(b.ToString());
            }
            else if (a.IsString && b.IsArray)
            {
                return a.ToString().CompareTo(b.ToString());
            }
            if (a.IsArray) 
            {
                return -1;
            }
            if (b.IsArray)
            {
                return 1;
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
            return Value.Of(!a.ToBool());
        }
        public static Value operator +(in Value a)
        {
            switch (a._type)
            {
                case ValueType.Bool:
                    return Value.Of(a.ToInt());
                case ValueType.Int:
                    return a;
                case ValueType.Float:
                    return a;
                case ValueType.String:
                    if (bool.TryParse(a._value.StringValue, out bool bo))
                    {
                        return Value.Of(bo ? 1 : 0);
                    }
                    else if (int.TryParse(a._value.StringValue, out int i))
                    {
                        return Value.Of(i);
                    }
                    else if (float.TryParse(a._value.StringValue, out float f))
                    {
                        return Value.Of(f);
                    }
                    else
                    {
                        return Value.NaN;
                    }
                case ValueType.Array:
                    return Value.NaN;
            }
            return a;
        }
        public static Value operator -(in Value a)
        {
            switch (a._type)
            {
                case ValueType.Bool:
                    return Value.Of(-a.ToInt());
                case ValueType.Int:
                    return Value.Of(-a.ToInt());
                case ValueType.Float:
                    return Value.Of(-a.ToFloat());
                case ValueType.String:
                    if (bool.TryParse(a._value.StringValue, out bool bo))
                    {
                        return Value.Of(bo ? -1 : 0);
                    }
                    else if (int.TryParse(a._value.StringValue, out int i))
                    {
                        return Value.Of(-i);
                    }
                    else if (float.TryParse(a._value.StringValue, out float f))
                    {
                        return Value.Of(-f);
                    }
                    else
                    {
                        return Value.NaN;
                    }
                case ValueType.Array:
                    return Value.NaN;
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
                return Value.Of(aiValue + biValue);
            }
            else if (opType == ValueType.Float)
            {
                return Value.Of(afValue + bfValue);
            }
            else if (opType == ValueType.String)
            {
                return Value.Of(a.ToString() + b.ToString());
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
                return Value.Of(aiValue - biValue);
            }
            else if (opType == ValueType.Float)
            {
                return Value.Of(afValue - bfValue);
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
                return Value.Of(aiValue * biValue);
            }
            else if (opType == ValueType.Float)
            {
                return Value.Of(afValue * bfValue);
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
                if (biValue == 0)
                {
                    return Value.Of(aiValue / (float)biValue);
                }
                return Value.Of(aiValue / biValue);
            }
            else if (opType == ValueType.Float)
            {
                return Value.Of(afValue / bfValue);
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
                if (biValue == 0)
                {
                    return Value.Of(aiValue / (float)biValue);
                }

                return Value.Of(aiValue % biValue);
            }
            else if (opType == ValueType.Float)
            {
                return Value.Of(afValue % bfValue);
            }
            else if (opType == ValueType.String)
            {
                return Value.NaN;
            }
            return Value.Invalid;
        }
        // &&, || のためにビット演算は別で定義する
        public static Value operator &(in Value a, in Value b)
        {
            if (!a.ToBool())
            {
                return a;
            }
            return b;
        }
        public static Value operator |(in Value a, in Value b)
        {
            if (a.ToBool())
            {
                return a;
            }
            return b;
        }
        public static Value operator <<(in Value a, int b)
        {
            return Value.Of(a.ToInt() << b);
        }
        public static Value operator >>(in Value a, int b)
        {
            return Value.Of(a.ToInt() >> b);
        }
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
                    if (i < _array.Count)
                    {
                        return _array[i];
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
                    if (i < _array.Count)
                    {
                        _array[i] = value;
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
            if (prioritizeString && (a.IsString || b.IsString))
            {
                // 文字列優先
                stringOp = true;
            }
            if (prioritizeString && (a.IsArray || b.IsArray))
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
            Array,
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
        private readonly List<Value> _array;
    }
}
