using System.Collections.Generic;

namespace Petit.Syntax.Lexer
{
    static class Keyword
    {
        public static bool TryGetTokenType(string ident, out TokenType tokenType)
        {
            if (_keywords.TryGetValue(ident, out tokenType))
            {
                return true;
            }
            tokenType = TokenType.Identifier;
            return false;
        }

        static Dictionary<string, TokenType> _keywords = new()
        {
            {"true", TokenType.Value},
            {"false", TokenType.Value},
            {"if", TokenType.If },
            {"else", TokenType.Else },
            {"switch", TokenType.Switch },
            {"case", TokenType.Case },
            {"default", TokenType.Default },
            {"while", TokenType.While },
            {"break", TokenType.Break },
            {"continue", TokenType.Continue },
            {"for", TokenType.For },
            {"return", TokenType.Return },
            {"fn", TokenType.Fn },
        };
    }
}
