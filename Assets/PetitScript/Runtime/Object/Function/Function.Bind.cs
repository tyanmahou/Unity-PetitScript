using System;
using System.Collections.Generic;
using System.Linq;

namespace Petit.Runtime
{
    /// <summary>
    /// 関数バインド
    /// </summary>
    public partial class Function
    {
        public static Function Raw(Func<IReadOnlyList<Value>, Value> func)
        {
            return new Function(
                func?.Method?.Name,
                func,
                null
                );
        }
        public static Function Bind(Action action)
        {
            return new Function(
                action?.Method?.Name,
                args =>
                {
                    action?.Invoke();
                    return Value.Invalid;
                }
            );
        }
        public static Function Bind(Action<Value> action)
        {
            return new Function(
                action?.Method?.Name,
                args =>
                {
                    action?.Invoke(args[0]);
                    return Value.Invalid;
                },
                GetArgs(action)
            );
        }
        public static Function Bind<T0>(Action<T0> action)
        {
            return new Function(
                action?.Method?.Name,
                args =>
                {
                    action?.Invoke(args[0].Cast<T0>());
                    return Value.Invalid;
                },
                GetArgs(action)
            );
        }
        public static Function Bind(Action<Value, Value> action)
        {
            return new Function(
                action?.Method?.Name,
                args =>
                {
                    action?.Invoke(args[0], args[1]);
                    return Value.Invalid;
                },
                GetArgs(action)
            );
        }
        public static Function Bind<T0, T1>(Action<T0, T1> action)
        {
            return new Function(
                action?.Method?.Name,
                args =>
                {
                    action?.Invoke(args[0].Cast<T0>(), args[1].Cast<T1>());
                    return Value.Invalid;
                },
                GetArgs(action)
            );
        }
        public static Function Bind(Action<Value, Value, Value> action)
        {
            return new Function(
                action?.Method?.Name,
                args =>
                {
                    action?.Invoke(args[0], args[1], args[2]);
                    return Value.Invalid;
                },
                GetArgs(action)
            );
        }
        public static Function Bind<T0, T1, T2>(Action<T0, T1, T2> action)
        {
            return new Function(
                action?.Method?.Name,
                args =>
                {
                    action?.Invoke(args[0].Cast<T0>(), args[1].Cast<T1>(), args[2].Cast<T2>());
                    return Value.Invalid;
                },
                GetArgs(action)
            );
        }
        public static Function Bind(Action<Value, Value, Value, Value> action)
        {
            return new Function(
                action?.Method?.Name,
                args =>
                {
                    action?.Invoke(args[0], args[1], args[2], args[3]);
                    return Value.Invalid;
                },
                GetArgs(action)
            );
        }
        public static Function Bind<T0, T1, T2, T3>(Action<T0, T1, T2, T3> action)
        {
            return new Function(
                action?.Method?.Name,
                args =>
                {
                    action?.Invoke(args[0].Cast<T0>(), args[1].Cast<T1>(), args[2].Cast<T2>(), args[3].Cast<T3>());
                    return Value.Invalid;
                },
                GetArgs(action)
            );
        }
        public static Function Bind(Action<Value, Value, Value, Value, Value> action)
        {
            return new Function(
                action?.Method?.Name,
                args =>
                {
                    action?.Invoke(args[0], args[1], args[2], args[3], args[4]);
                    return Value.Invalid;
                },
                GetArgs(action)
            );
        }
        public static Function Bind<T0, T1, T2, T3, T4>(Action<T0, T1, T2, T3, T4> action)
        {
            return new Function(
                action?.Method?.Name,
                args =>
                {
                    action?.Invoke(args[0].Cast<T0>(), args[1].Cast<T1>(), args[2].Cast<T2>(), args[3].Cast<T3>(), args[4].Cast<T4>());
                    return Value.Invalid;
                },
                GetArgs(action)
            );
        }
        public static Function Bind(Func<Value> action)
        {
            return new Function(
                action?.Method?.Name,
                args =>
                {
                    return action?.Invoke() ?? Value.Invalid;
                }
            );
        }
        public static Function Bind<TResult>(Func<TResult> action)
        {
            return new Function(
                action?.Method?.Name,
                args =>
                {
                    if (action is null)
                    {
                        return Value.Invalid;
                    }
                    return Value.Of(action.Invoke());
                }
            );
        }
        public static Function Bind(Func<Value, Value> action)
        {
            return new Function(
                action?.Method?.Name,
                args =>
                {
                    return action?.Invoke(args[0]) ?? Value.Invalid;
                },
                GetArgs(action)
            );
        }
        public static Function Bind<T0, TResult>(Func<T0, TResult> action)
        {
            return new Function(
                action?.Method?.Name,
                args =>
                {
                    if (action is null)
                    {
                        return Value.Invalid;
                    }
                    return Value.Of(action.Invoke(args[0].Cast<T0>()));
                },
                GetArgs(action)
            );
        }
        public static Function Bind(Func<Value, Value, Value> action)
        {
            return new Function(
                action?.Method?.Name,
                args =>
                {
                    return action?.Invoke(args[0], args[1]) ?? Value.Invalid;
                },
                GetArgs(action)
            );
        }
        public static Function Bind<T0, T1, TResult>(Func<T0, T1, TResult> action)
        {
            return new Function(
                action?.Method?.Name,
                args =>
                {
                    if (action is null)
                    {
                        return Value.Invalid;
                    }
                    return Value.Of(action.Invoke(args[0].Cast<T0>(), args[1].Cast<T1>()));
                },
                GetArgs(action)
            );
        }
        public static Function Bind(Func<Value, Value, Value, Value> action)
        {
            return new Function(
                action?.Method?.Name,
                args =>
                {
                    return action?.Invoke(args[0], args[1], args[2]) ?? Value.Invalid;
                },
                GetArgs(action)
            );
        }
        public static Function Bind<T0, T1, T2, TResult>(Func<T0, T1, T2, TResult> action)
        {
            return new Function(
                action?.Method?.Name,
                args =>
                {
                    if (action is null)
                    {
                        return Value.Invalid;
                    }
                    return Value.Of(action.Invoke(args[0].Cast<T0>(), args[1].Cast<T1>(), args[2].Cast<T2>()));
                },
                GetArgs(action)
            );
        }
        public static Function Bind(Func<Value, Value, Value, Value, Value> action)
        {
            return new Function(
                action?.Method?.Name, 
                args =>
                {
                    return action?.Invoke(args[0], args[1], args[2], args[3]) ?? Value.Invalid;
                },
                GetArgs(action)
            );
        }
        public static Function Bind<T0, T1, T2, T3, TResult>(Func<T0, T1, T2, T3, TResult> action)
        {
            return new Function(
                action?.Method?.Name,
                args =>
                {
                    if (action is null)
                    {
                        return Value.Invalid;
                    }
                    return Value.Of(action.Invoke(args[0].Cast<T0>(), args[1].Cast<T1>(), args[2].Cast<T2>(), args[3].Cast<T3>()));
                },
                GetArgs(action)
            );
        }
        public static Function Bind(Func<Value, Value, Value, Value, Value, Value> action)
        {
            return new Function(
                action?.Method?.Name,
                args =>
                {
                    return action?.Invoke(args[0], args[1], args[2], args[3], args[4]) ?? Value.Invalid;
                },
                GetArgs(action)
            );
        }
        public static Function Bind<T0, T1, T2, T3, T4, TResult>(Func<T0, T1, T2, T3, T4, TResult> action)
        {
            return new Function(
                action?.Method?.Name,
                args =>
                {
                    if (action is null)
                    {
                        return Value.Invalid;
                    }
                    return Value.Of(action.Invoke(args[0].Cast<T0>(), args[1].Cast<T1>(), args[2].Cast<T2>(), args[3].Cast<T3>(), args[4].Cast<T4>()));
                },
                GetArgs(action)
            );
        }
        internal static Argument[] GetArgs(Delegate del)
        {
            // メソッド情報を取得
            System.Reflection.MethodInfo methodInfo = del.Method;

            // 引数名を取得
            return methodInfo.GetParameters().Select(p => new Argument(p.Name, Value.Of(p.DefaultValue))).ToArray();
        }
    }
}
