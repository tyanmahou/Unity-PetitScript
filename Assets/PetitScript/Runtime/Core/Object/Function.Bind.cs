using System;

namespace Petit.Core
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
        public static Function Bind(
            Action<Value> action
            , Argument arg0 = default
            )
        {
            return new Function(args =>
            {
                action?.Invoke(args[0]);
                return Value.Invalid;
            }, arg0);
        }
        public static Function Bind(
            Action<Value, Value> action
            , Argument arg0 = default
            , Argument arg1 = default
            )
        {
            return new Function(args =>
            {
                action?.Invoke(args[0], args[1]);
                return Value.Invalid;
            }, arg0, arg1);
        }
        public static Function Bind(
            Action<Value, Value, Value> action
            , Argument arg0 = default
            , Argument arg1 = default
            , Argument arg2 = default
            )
        {
            return new Function(args =>
            {
                action?.Invoke(args[0], args[1], args[2]);
                return Value.Invalid;
            }, arg0, arg1, arg2);
        }
        public static Function Bind(
            Action<Value, Value, Value, Value> action
            , Argument arg0 = default
            , Argument arg1 = default
            , Argument arg2 = default
            , Argument arg3 = default
            )
        {
            return new Function(args =>
            {
                action?.Invoke(args[0], args[1], args[2], args[3]);
                return Value.Invalid;
            }, arg0, arg1, arg2, arg3);
        }
        public static Function Bind(
            Action<Value, Value, Value, Value, Value> action
            , Argument arg0 = default
            , Argument arg1 = default
            , Argument arg2 = default
            , Argument arg3 = default
            , Argument arg4 = default
            )
        {
            return new Function(args =>
            {
                action?.Invoke(args[0], args[1], args[2], args[3], args[4]);
                return Value.Invalid;
            }, arg0, arg1, arg2, arg3, arg4);
        }

        public static Function Bind(Func<Value> action)
        {
            return new Function(args =>
            {
                return action?.Invoke() ?? Value.Invalid;
            });
        }
        public static Function Bind(
            Func<Value, Value> action
            , Argument arg0 = default
            )
        {
            return new Function(args =>
            {
                action?.Invoke(args[0]);
                return Value.Invalid;
            }, arg0);
        }
        public static Function Bind(
            Func<Value, Value, Value> action
            , Argument arg0 = default
            , Argument arg1 = default
            )
        {
            return new Function(args =>
            {
                return action?.Invoke(args[0], args[1]) ?? Value.Invalid;
            }, arg0, arg1);
        }
        public static Function Bind(
            Func<Value, Value, Value, Value> action
            , Argument arg0 = default
            , Argument arg1 = default
            , Argument arg2 = default
            )
        {
            return new Function(args =>
            {
                return action?.Invoke(args[0], args[1], args[2]) ?? Value.Invalid;
            }, arg0, arg1, arg2);
        }
        public static Function Bind(
            Func<Value, Value, Value, Value, Value> action
            , Argument arg0 = default
            , Argument arg1 = default
            , Argument arg2 = default
            , Argument arg3 = default
            )
        {
            return new Function(args =>
            {
                return action?.Invoke(args[0], args[1], args[2], args[3]) ?? Value.Invalid;
            }, arg0, arg1, arg2, arg3);
        }
        public static Function Bind(
            Func<Value, Value, Value, Value, Value, Value> action
            , Argument arg0 = default
            , Argument arg1 = default
            , Argument arg2 = default
            , Argument arg3 = default
            , Argument arg4 = default
            )
        {
            return new Function(args =>
            {                
                return action?.Invoke(args[0], args[1], args[2], args[3], args[4]) ?? Value.Invalid;
            }, arg0, arg1, arg2, arg3, arg4);
        }
    }
}
