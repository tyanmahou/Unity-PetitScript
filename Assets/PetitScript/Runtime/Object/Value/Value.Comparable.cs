using System;
using System.Collections.Generic;
using System.Text;

namespace Petit.Runtime
{
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
                case (ValueType.Int, ValueType.Int):
                    return a._value.IntValue.CompareTo(b._value.IntValue);
                case (ValueType.Float, ValueType.Float):
                    return a._value.FloatValue.CompareTo(b._value.FloatValue);
                case (ValueType.String, ValueType.String):
                    return a._value.StringValue.CompareTo(b._value.StringValue);
                case (ValueType.Array, ValueType.Array):
                    return Compare(a._reference.ArrayValue, b._reference.ArrayValue);
                case (ValueType.Array, ValueType.Int):
                case (ValueType.Array, ValueType.Float):
                case (ValueType.Array, ValueType.Bool):
                    return Compare(CompareValue(a._reference.ArrayValue), b);
                case (ValueType.Array, ValueType.String):
                    return CompareString(a._reference.ArrayValue).CompareTo(b._value.StringValue);
                case (ValueType.Int, ValueType.Array):
                case (ValueType.Float, ValueType.Array):
                case (ValueType.Bool, ValueType.Array):
                    return Compare(a, CompareValue(b._reference.ArrayValue));
                case (ValueType.String, ValueType.Array):
                    return a._value.StringValue.CompareTo(CompareString(b._reference.ArrayValue));
            }
            return CompareNumeric(a, b);
        }
        static int CompareNumeric(in Value a, in Value b)
        {
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
