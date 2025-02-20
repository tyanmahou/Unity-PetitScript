namespace Petit.Core.Lexer
{
    class Token
    {
        public Token(TokenType type, string value, int line, int column)
        {
            Type = type;
            Value = value;
            Line = line;
            Column = column;
        }

        public readonly TokenType Type;
        public readonly string Value;

        public readonly int Line;
        public readonly int Column;
    }
}
