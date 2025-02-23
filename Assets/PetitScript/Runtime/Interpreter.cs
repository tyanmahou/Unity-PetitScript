using System;
using System.Linq;

namespace Petit.Runtime
{
    /// <summary>
    /// インタプリタ
    /// </summary>
    public class Interpreter
    {
        /// <summary>
        /// スクリプト実行
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static Value RunScript(string code)
        {
            return new Interpreter().Run(code);
        }

        public Environment Environment => _env;

        /// <summary>
        /// リザルト処理
        /// </summary>
        public Interpreter Then(Action<Value> action)
        {
            _onResult += action;
            return this;
        }
        public Interpreter Then(Action<int> action)
        {
            _onResult += v => action?.Invoke(v.ToInt());
            return this;
        }
        public Interpreter Then(Action<float> action)
        {
            _onResult += v => action?.Invoke(v.ToFloat());
            return this;
        }
        public Interpreter Then(Action<string> action)
        {
            _onResult += v => action?.Invoke(v.ToString());
            return this;
        }
        public Interpreter Then(Action<bool> action)
        {
            _onResult += v => action?.Invoke(v.ToBool());
            return this;
        }
        /// <summary>
        /// エラー時処理
        /// </summary>
        public Interpreter Catch(Action<System.Exception> action)
        {
            _onCatchError += ex =>
            {
                action?.Invoke(ex);
                return false;
            };
            return this;
        }
        public Interpreter Catch<T>(Action<T> action)
            where T : System.Exception
        {
            _onCatchError += ex =>
            {
                if (ex is T exT)
                {
                    action?.Invoke(exT);
                    return true;
                }
                return false;
            };
            return this;
        }
        public Interpreter Catch(Action<string> action)
        {
            _onCatchError += ex =>
            {
                action?.Invoke(ex.Message);
                return false;
            };
            return this;
        }
        public Value Run(string code)
        {
            return Run(code, _env);
        }
        public Value Run(string code, Environment env)
        {
            Value result;
            try
            {
                var ast = Syntax.SyntaxTree.Parse(code).GlobalStatement;
                var executer = new Executor.Executor();
                result = executer.Exec(ast, env ?? _env);
            }
            catch (System.Exception ex)
            {
                if (_onCatchError != null)
                {
                    bool catched = false;
                    foreach (var catchAction in _onCatchError.GetInvocationList().Cast<Func<System.Exception, bool>>())
                    {
                        catched |= catchAction.Invoke(ex);
                    }
                    if (catched)
                    {
                        return Value.Invalid;
                    }
                }
                throw;
            }
            _onResult?.Invoke(result);
            return result;
        }
        Environment _env = new(Environment.Global);
        Func<System.Exception, bool> _onCatchError;
        Action<Value> _onResult;
    }
}
