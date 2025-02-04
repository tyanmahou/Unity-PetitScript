using System;
using System.Collections.Generic;
using System.Text;

namespace Petit
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
        /// シンタックスエラー時処理
        /// </summary>
        public Action<string> OnSyntaxError {  get; set; } = UnityEngine.Debug.LogError;

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
            return executer.Exec(ast);
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
