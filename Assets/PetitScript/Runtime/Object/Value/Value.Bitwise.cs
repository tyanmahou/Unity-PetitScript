

namespace Petit.Runtime
{
    public readonly partial struct Value
    {
        public static Value operator <<(in Value a, int b)
        {
            return Value.Of(a.ToInt() << b);
        }
        public static Value operator >>(in Value a, int b)
        {
            return Value.Of(a.ToInt() >> b);
        }

        public static Value BitwiseNot(in Value a)
        {
            return Value.Of(~a.ToInt());
        }
        public static Value BitwiseAnd(in Value a, in Value b)
        {
            return Value.Of(a.ToInt() & b.ToInt());
        }
        public static Value BitwiseOr(in Value a, in Value b)
        {
            return Value.Of(a.ToInt() | b.ToInt());
        }
        public static Value BitwiseXor(in Value a, in Value b)
        {
            return Value.Of(a.ToInt() ^ b.ToInt());
        }
    }
}
