using System;
using System.Collections.Generic;
using System.Text;

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
        /// リザルト
        /// </summary>
        public Action<Value> OnResult { get; set; }
        public Interpreter SetOnResult(Action<Value> action)
        {
            OnResult = action;
            return this;
        }
        public Interpreter SetOnResult(Action<int> action)
        {
            OnResult = v => action?.Invoke(v.ToInt());
            return this;
        }
        public Interpreter SetOnResult(Action<float> action)
        {
            OnResult = v => action?.Invoke(v.ToFloat());
            return this;
        }
        public Interpreter SetOnResult(Action<string> action)
        {
            OnResult = v => action?.Invoke(v.ToString());
            return this;
        }
        public Interpreter SetOnResult(Action<bool> action)
        {
            OnResult = v => action?.Invoke(v.ToBool());
            return this;
        }
        /// <summary>
        /// シンタックスエラー時処理
        /// </summary>
        public Action<string> OnSyntaxError {  get; set; }
        public Interpreter SetOnSyntaxError(Action<string> action)
        {
            OnSyntaxError = action;
            return this;
        }
        public Value Run(string code)
        {
            var lexer = new Lexer.Lexer();
            var tokens = lexer.Tokenize(code);

            var parser = new Parser.Parser();
            var (ast, errors) = parser.Parse(tokens);

            if (SyntaxError(errors))
            {
                // 構文エラー
                return Value.Invalid;
            }

            var executer = new Executor.Executor(_env);
            var result = executer.Exec(ast);
            OnResult?.Invoke(result);
            return result;
        }

        bool SyntaxError(IReadOnlyList<Parser.SyntaxError> errors)
        {
            if (errors is null || errors.Count == 0)
            {
                return false;
            }
            if (OnSyntaxError is null)
            {
                return true;
            }
            StringBuilder errorBuilder = new StringBuilder();
            errorBuilder.AppendLine("[Petit] SyntaxError");
            for (int i = 0; i < errors.Count; ++i) 
            {
                errorBuilder.AppendLine(errors[i].ToString());
            }
            OnSyntaxError?.Invoke(errorBuilder.ToString());
            return true;
        }
        Enviroment _env = new();
    }
}
