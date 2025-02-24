using System;
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
                    var funcResult = a._reference.FuncValue?.Invoke() ?? Value.Invalid;
                    return +funcResult;
            }
            return Value.NaN;
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
                    var funcResult = a._reference.FuncValue?.Invoke() ?? Value.Invalid;
                    return -funcResult;
            }
            return Value.NaN;
        }
        public static Value operator +(in Value a, in Value b)
        {
            switch((a._type, b._type))
            {
                case (ValueType.String, ValueType.String):
                    return a._value.StringValue + b._value.StringValue;
                case (ValueType.String, _):
                    return a._value.StringValue + b.ToString();
                case (_, ValueType.String):
                    return a.ToString() + b._value.StringValue;
                case (ValueType.Array, ValueType.Array):
                    return CompareString(a._reference.ArrayValue) + CompareString(b._reference.ArrayValue);
                case (ValueType.Array, _):
                    return CompareString(a._reference.ArrayValue) + b.ToString();
                case (_, ValueType.Array):
                    return a.ToString() + CompareString(b._reference.ArrayValue);
            }
            ValueType opType = TryGetArithmeticType(a, b,
                out int aiValue,
                out int biValue,
                out float afValue,
                out float bfValue
                );
            switch (opType)
            {
                case ValueType.Int:
                    return aiValue + biValue;
                case ValueType.Float:
                    return afValue + bfValue;
            }
            return a.ToString() + b.ToString();
        }
        public static Value operator -(in Value a, in Value b)
        {
            ValueType opType = TryGetArithmeticType(a, b,
                out int aiValue,
                out int biValue,
                out float afValue,
                out float bfValue
                );
            switch (opType)
            {
                case ValueType.Int:
                    return aiValue - biValue;
                case ValueType.Float:
                    return afValue - bfValue;
            }
            return Value.NaN;
        }
        public static Value operator *(in Value a, in Value b)
        {
            ValueType opType = TryGetArithmeticType(a, b,
                out int aiValue,
                out int biValue,
                out float afValue,
                out float bfValue
                );
            switch (opType)
            {
                case ValueType.Int:
                    return aiValue * biValue;
                case ValueType.Float:
                    return afValue * bfValue;
            }
            return Value.NaN;
        }
        public static Value operator /(in Value a, in Value b)
        {
            ValueType opType = TryGetArithmeticType(a, b,
                out int aiValue,
                out int biValue,
                out float afValue,
                out float bfValue
                );
            switch (opType)
            {
                case ValueType.Int when biValue != 0:
                    return aiValue / biValue;
                case ValueType.Int when biValue == 0:
                case ValueType.Float:
                    return afValue / bfValue;
            }
            return Value.NaN;
        }
        public static Value operator %(in Value a, in Value b)
        {
            ValueType opType = TryGetArithmeticType(a, b,
                out int aiValue,
                out int biValue,
                out float afValue,
                out float bfValue
                );
            switch (opType)
            {
                case ValueType.Int when biValue != 0:
                    return aiValue % biValue;
                case ValueType.Int when biValue == 0:
                case ValueType.Float:
                    return afValue % bfValue;
            }
            return Value.NaN;
        }

        private static ValueType TryGetArithmeticType(
            in Value a, 
            in Value b,
            out int aiValue,
            out int biValue,
            out float afValue,
            out float bfValue
            )
        {
            ValueType aNumricType = a.TryGetNumericWithType(out aiValue, out afValue);
            ValueType bNumricType = b.TryGetNumericWithType(out biValue, out bfValue);
            switch ((aNumricType, bNumricType))
            {
                case (ValueType.Int, ValueType.Int):
                    return ValueType.Int;
                case (ValueType.Int, ValueType.Float):
                    return ValueType.Float;
                case (ValueType.Float, ValueType.Int):
                    return ValueType.Float;
                case (ValueType.Float, ValueType.Float):
                    return ValueType.Float;
            }
            return ValueType.Invalid;
        }
        bool TryGetNumeric(out float num)
        {
            if (IsBool || IsInt || IsFloat)
            {
                num = ToFloat();
                return true;
            }
            else if (IsString)
            {
                if (bool.TryParse(_value.StringValue, out bool b))
                {
                    num = b ? 1.0f : 0.0f;
                    return true;
                }
                else if (float.TryParse(_value.StringValue, out float f))
                {
                    num = f;
                    return true;
                }
            }
            else if (IsFunction)
            {
                var funcResult = _reference.FuncValue?.Invoke() ?? Value.Invalid;
                return funcResult.TryGetNumeric(out num);
            }
            num = 0;
            return false;
        }
        ValueType TryGetNumericWithType(out int intValue, out float floatValue)
        {
            intValue = 0;
            floatValue = 0.0f;
            switch (_type)
            {
                case ValueType.Bool:
                    intValue = ToInt();
                    floatValue = (float)intValue;
                    return ValueType.Int;
                case ValueType.Int:
                    intValue = _value.IntValue;
                    floatValue = (float)intValue;
                    return ValueType.Int;
                case ValueType.Float:
                    floatValue = _value.FloatValue;
                    intValue = (int)floatValue;
                    return ValueType.Float;
                case ValueType.String:
                    if (bool.TryParse(_value.StringValue, out bool bo))
                    {
                        intValue = bo ? 1 : 0;
                        floatValue = (float)intValue;
                        return ValueType.Int;
                    }
                    else if (int.TryParse(_value.StringValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out int i))
                    {
                        intValue = i;
                        floatValue = (float)intValue;
                        return ValueType.Int;
                    }
                    else if (float.TryParse(_value.StringValue, NumberStyles.Float, CultureInfo.InvariantCulture, out float f))
                    {
                        floatValue = f;
                        intValue = (int)f;
                        return ValueType.Float;
                    }
                    break;
                case ValueType.Array:
                    return CompareValue(_reference.ArrayValue).TryGetNumericWithType(out intValue, out floatValue);
                case ValueType.Function:
                    {
                        var funcResult = _reference.FuncValue?.Invoke() ?? Value.Invalid;
                        return funcResult.TryGetNumericWithType(out intValue, out floatValue);
                    }
            }
            return ValueType.Invalid;
        }
    }
}
