using System.Collections.Generic;

namespace Petit.Core
{
    public static class Syntax
    {
        public static AST.GlobalStatement Parse(string code)
        {
            var lexer = new Lexer.Lexer();
            var tokens = lexer.Tokenize(code);

            var parser = new Parser.Parser();
            return parser.Parse(tokens);
        }
    }
}
