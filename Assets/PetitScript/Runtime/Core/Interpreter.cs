using System;
using System.Linq;

namespace Petit.Core
{
    /// <summary>
    /// インタプリタ
    /// </summary>
    public class Interpreter
    {
        /// <summary>
        /// 変数
        /// </summary>
        public Variables Variables
        {
            get => _env.Variables;
            set => _env.Variables = value;
        }

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
            Value result;
            try
            {
                var lexer = new Lexer.Lexer();
                var tokens = lexer.Tokenize(code);

                var parser = new Parser.Parser();
                var ast = parser.Parse(tokens);
                var executer = new Executor.Executor(_env);
                result = executer.Exec(ast);
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
        Enviroment _env = new();
        Func<System.Exception, bool> _onCatchError;
        Action<Value> _onResult;
    }
}
