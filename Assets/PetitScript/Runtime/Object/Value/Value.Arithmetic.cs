using System.Globalization;

namespace Petit.Runtime
{
    /// <summary>
    /// Value
    /// 算術に関する処理
    /// </summary>
    public readonly partial struct Value
    {
        public static Value operator +(in Value a)
        {
            switch (a._type)
            {
                case ValueType.Bool:
                    return a.ToInt();
                case ValueType.Int:
                    return a;
                case ValueType.Float:
                    return a;
                case ValueType.String:
                    if (bool.TryParse(a._value.StringValue, out bool bo))
                    {
                       return bo ? 1 : 0;
                    }
                    else if (int.TryParse(a._value.StringValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out int i))
                    {
                        return i;
                    }
                    else if (float.TryParse(a._value.StringValue, NumberStyles.Float, CultureInfo.InvariantCulture, out float f))
                    {
                        return f;
                    }
                    else if (string.IsNullOrEmpty(a._value.StringValue))
                    {
                        return 0;
                    }
                    else
                    {
                        return Value.NaN;
                    }
                case ValueType.Array:
                    return +CompareValue(a._reference.ArrayValue);
                case ValueType.Function:
                    return Value.NaN;
            }
            return a;
        }
        public static Value operator -(in Value a)
        {
            switch (a._type)
            {
                case ValueType.Bool:
                    return -a.ToInt();
                case ValueType.Int:
                    return -a._value.IntValue;
                case ValueType.Float:
                    return -a._value.FloatValue;
                case ValueType.String:
                    if (bool.TryParse(a._value.StringValue, out bool bo))
                    {
                        return bo ? -1 : 0;
                    }
                    else if (int.TryParse(a._value.StringValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out int i))
                    {
                        return -i;
                    }
                    else if (float.TryParse(a._value.StringValue, NumberStyles.Float, CultureInfo.InvariantCulture, out float f))
                    {
                        return -f;
                    }
                    else if (string.IsNullOrEmpty(a._value.StringValue))
                    {
                        return 0;
                    }
                    else
                    {
                        return Value.NaN;
                    }
                case ValueType.Array:
                    return -CompareValue(a._reference.ArrayValue);
                case ValueType.Function:
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

    }
}
