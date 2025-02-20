using System;
using System.Linq;

namespace Petit.Runtime
{
    /// <summary>
    /// 関数バインド
    /// </summary>
    public partial class Function
    {
        public static Function Bind(Action action)
        {
            return new Function(args =>
            {
                action?.Invoke();
                return Value.Invalid;
            });
        }
        public static Function Bind(Action<Value> action)
        {
            return new Function(args =>
            {
                action?.Invoke(args[0]);
                return Value.Invalid;
            }, GetArgs(action));
        }
        public static Function Bind(Action<Value, Value> action)
        {
            return new Function(args =>
            {
                action?.Invoke(args[0], args[1]);
                return Value.Invalid;
            }, GetArgs(action));
        }
        public static Function Bind(Action<Value, Value, Value> action)
        {
            return new Function(args =>
            {
                action?.Invoke(args[0], args[1], args[2]);
                return Value.Invalid;
            }, GetArgs(action));
        }
        public static Function Bind(Action<Value, Value, Value, Value> action)
        {
            return new Function(args =>
            {
                action?.Invoke(args[0], args[1], args[2], args[3]);
                return Value.Invalid;
            }, GetArgs(action));
        }
        public static Function Bind(Action<Value, Value, Value, Value, Value> action)
        {
            return new Function(args =>
            {
                action?.Invoke(args[0], args[1], args[2], args[3], args[4]);
                return Value.Invalid;
            }, GetArgs(action));
        }

        public static Function Bind(Func<Value> action)
        {
            return new Function(args =>
            {
                return action?.Invoke() ?? Value.Invalid;
            });
        }
        public static Function Bind(Func<Value, Value> action)
        {
            return new Function(args =>
            {
                action?.Invoke(args[0]);
                return Value.Invalid;
            }, GetArgs(action));
        }
        public static Function Bind(Func<Value, Value, Value> action)
        {
            return new Function(args =>
            {
                return action?.Invoke(args[0], args[1]) ?? Value.Invalid;
            }, GetArgs(action));
        }
        public static Function Bind(Func<Value, Value, Value, Value> action)
        {
            return new Function(args =>
            {
                return action?.Invoke(args[0], args[1], args[2]) ?? Value.Invalid;
            }, GetArgs(action));
        }
        public static Function Bind(Func<Value, Value, Value, Value, Value> action)
        {
            return new Function(args =>
            {
                return action?.Invoke(args[0], args[1], args[2], args[3]) ?? Value.Invalid;
            }, GetArgs(action));
        }
        public static Function Bind(Func<Value, Value, Value, Value, Value, Value> action)
        {
            return new Function(args =>
            {                
                return action?.Invoke(args[0], args[1], args[2], args[3], args[4]) ?? Value.Invalid;
            }, GetArgs(action));
        }
        internal static Argument[] GetArgs(Delegate del)
        {
            // メソッド情報を取得
            System.Reflection.MethodInfo methodInfo = del.Method;

            // 引数名を取得
            return methodInfo.GetParameters().Select(p => new Argument(p.Name, new Value(p.DefaultValue))).ToArray();
        }
    }
}
