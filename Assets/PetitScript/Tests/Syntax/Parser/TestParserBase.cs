using NUnit.Framework;
using Petit.Syntax.AST;

namespace Petit.Syntax.Parser
{
    class TestParserBase
    {
        protected static U As<U>(object v)
            where U : class
        {
            Assert.True(v is U);
            return v as U;
        }
        protected static GlobalStatement Parse(string code)
        {
            Lexer.Lexer lexer = new();
            var tokens = lexer.Tokenize(code);
            Parser parser = new();
            return parser.Parse(tokens).GlobalStatement;
        }
        protected static IExpression GetExpr(string code)
        {
            var root = Parse(code);
            var e = As<ExpressionStatement>(root?.Statements?[0]);
            return e?.Expression;
        }
    }
}
