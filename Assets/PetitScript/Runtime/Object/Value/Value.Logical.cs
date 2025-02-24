

namespace Petit.Runtime
{
    public readonly partial struct Value
    {
        public static bool operator true(in Value v) => v.ToBool();
        public static bool operator false(in Value v) => !v.ToBool();

        public static Value operator !(in Value a)
        {
            return Value.Of(!a.ToBool());
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
    }
}
