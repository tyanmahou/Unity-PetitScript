﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Petit.Runtime
{
    /// <summary>
    /// Value 
    /// IComparable<Value>の実装
    /// 比較系処理
    /// </summary>
    public readonly partial struct Value : IComparable<Value>
    {
        public int CompareTo(Value other)
        {
            return Compare(this, other);
        }
        public static int Compare(in Value a, in Value b)
        {
            if (a.IsNaN || b.IsNaN)
            {
                return a.ToFloat().CompareTo(b.ToFloat());
            }
            switch ((a._type, b._type))
            {
                case (ValueType.Bool, ValueType.Bool):
                    return a._value.BoolValue.CompareTo(b._value.BoolValue);
                case (ValueType.Bool, ValueType.Int):
                    return a.ToInt().CompareTo(b._value.IntValue);
                case (ValueType.Bool, ValueType.Float):
                    return a.ToFloat().CompareTo(b._value.FloatValue);
                case (ValueType.Bool, ValueType.Array):
                    return Compare(a, CompareValue(b._reference.ArrayValue));

                case (ValueType.Int, ValueType.Bool):
                    return a._value.IntValue.CompareTo(b.ToInt());
                case (ValueType.Int, ValueType.Int):
                    return a._value.IntValue.CompareTo(b._value.IntValue);
                case (ValueType.Int, ValueType.Float):
                    return a.ToFloat().CompareTo(b._value.FloatValue);
                case (ValueType.Int, ValueType.Array):
                    return Compare(a, CompareValue(b._reference.ArrayValue));

                case (ValueType.Float, ValueType.Bool):
                    return a._value.FloatValue.CompareTo(b.ToFloat());
                case (ValueType.Float, ValueType.Int):
                    return a._value.FloatValue.CompareTo(b.ToFloat());
                case (ValueType.Float, ValueType.Float):
                    return a._value.FloatValue.CompareTo(b._value.FloatValue);
                case (ValueType.Float, ValueType.Array):
                    return Compare(a, CompareValue(b._reference.ArrayValue));

                case (ValueType.String, ValueType.String):
                    return a._value.StringValue.CompareTo(b._value.StringValue);
                case (ValueType.String, ValueType.Array):
                    return a._value.StringValue.CompareTo(CompareString(b._reference.ArrayValue));

                case (ValueType.Array, ValueType.Bool):
                case (ValueType.Array, ValueType.Int):
                case (ValueType.Array, ValueType.Float):
                    return Compare(CompareValue(a._reference.ArrayValue), b);
                case (ValueType.Array, ValueType.String):
                    return CompareString(a._reference.ArrayValue).CompareTo(b._value.StringValue);
                case (ValueType.Array, ValueType.Array):
                    return Compare(a._reference.ArrayValue, b._reference.ArrayValue);

                case (ValueType.Function, ValueType.Function):
                    return a._reference.FuncValue.Invoke().CompareTo(b._reference.FuncValue.Invoke());
            }
            if (TryCompareNumeric(a, b, out int comp))
            {
                return comp;
            }
            return a.ToString().CompareTo(b.ToString());
        }
        static bool TryCompareNumeric(in Value a, in Value b, out int result)
        {
            bool isNumricA = a.TryGetNumeric(out float valueA);
            bool isNumricB = b.TryGetNumeric(out float valueB);
            if (isNumricA && isNumricB)
            {
                result = valueA.CompareTo(valueB);
                return true;
            }
            result = 0;
            return false;
        }
        static int Compare(in List<Value> a, in List<Value> b)
        {
            int min = Math.Min(a.Count, b.Count);
            for (int index = 0; index < min; ++index)
            {
                int comp = a[index].CompareTo(b[index]);
                if (comp != 0)
                {
                    return comp;
                }
            }
            return a.Count.CompareTo(b.Count);
        }
        static string CompareString(in Value v)
        {
            if (v.IsArray)
            {
                return CompareString(v._reference.ArrayValue);
            }
            return v.ToString();
        }
        static string CompareString(List<Value> array)
        {
            StringBuilder sb = new StringBuilder();
            bool isFirst = true;
            foreach (Value item in array)
            {
                if (!isFirst)
                {
                    sb.Append(",");
                }
                sb.Append(CompareString(item));
                isFirst = false;
            }
            return sb.ToString();
        }
        static Value CompareValue(List<Value> array)=> Value.Parse(CompareString(array));
    }
}
