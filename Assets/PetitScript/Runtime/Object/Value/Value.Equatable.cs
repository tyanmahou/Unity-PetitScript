using System;
using System.Collections;

namespace Petit.Runtime
{
    public readonly partial struct Value
        : IEquatable<Value>
    {
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
                case ValueType.Invalid:
                    return true;
                case ValueType.Bool:
                    return _value.BoolValue == other._value.BoolValue;
                case ValueType.Int:
                    return _value.IntValue == other._value.IntValue;
                case ValueType.Float:
                    return _value.FloatValue == other._value.FloatValue;
                case ValueType.String:
                    return _value.StringValue == other._value.StringValue;
                case ValueType.Array:
                    if (_reference.ArrayValue.Count != other._reference.ArrayValue.Count)
                    {
                        return false;
                    }
                    for (int index = 0; index < _reference.ArrayValue.Count; ++index)
                    {
                        if (!_reference.ArrayValue[index].Equals(other._reference.ArrayValue[index]))
                        {
                            return false;
                        }
                    }
                    return true;
                case ValueType.Function:
                    return _reference.FuncValue == other._reference.FuncValue;
            }
            return false;
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
            else if (other is Function func)
            {
                return Equals(Value.Of(func));
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
                    if (_reference.ArrayValue.Count != otherList.Count)
                    {
                        return false;
                    }
                    for (int index = 0; index < _reference.ArrayValue.Count; ++index)
                    {
                        if (!_reference.ArrayValue[index].EqualsLooseSingly(otherList[index]))
                        {
                            return false;
                        }
                    }
                    return true;
                case ValueType.Function:
                    return _reference.FuncValue == other.ToFunction();
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
    }
}
