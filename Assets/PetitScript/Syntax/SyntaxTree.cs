
namespace Petit.Core
{
    /// <summary>
    /// 構文木
    /// </summary>
    public static class SyntaxTree
    {
        public static AST.Program Parse(string code)
        {
            var lexer = new Lexer.Lexer();
            var tokens = lexer.Tokenize(code);

            var parser = new Parser.Parser();
            return parser.Parse(tokens);
        }
    }
}
