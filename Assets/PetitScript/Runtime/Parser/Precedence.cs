namespace Petit.Parser
{
    using TokenType = Petit.Lexer.TokenType;

    enum Precedence
    {
        Not,
        Mul,
        Add,
        Spaceship,
        Comp,
        Equals,
        LogicalAnd,
        LogicalOr,
        Assign,
        Lowest,
    }

    static class PrecedenceExtensions
    {
        public static Precedence FromTokenType(TokenType tokenType, bool unary = false)
        {
            if (unary)
            {
                switch (tokenType)
                {
                    case TokenType.Not:
                    case TokenType.Plus:
                    case TokenType.Minus:
                        return Precedence.Not;
                }
            }
            switch (tokenType)
            {
                case TokenType.Mul:
                case TokenType.Div:
                case TokenType.Mod:
                    return Precedence.Mul;

                case TokenType.Add:
                case TokenType.Sub:
                    return Precedence.Add;

                case TokenType.Spaceship:
                    return Precedence.Spaceship;

                case TokenType.LessThan:
                case TokenType.LessThanOrEquals:
                case TokenType.GreaterThan:
                case TokenType.GreaterThanOrEquals:
                    return Precedence.Comp;

                case TokenType.Equals:
                case TokenType.Identical:
                case TokenType.NotEquals:
                case TokenType.NotIdentical:
                    return Precedence.Equals;

                case TokenType.LogicalAnd:
                    return Precedence.LogicalAnd;

                case TokenType.LogicalOr:
                    return Precedence.LogicalOr;
                case TokenType.Question:
                case TokenType.Colon:
                case TokenType.Assign:
                case TokenType.AddAssign:
                case TokenType.SubAssign:
                case TokenType.MulAssign:
                case TokenType.DivAssign:
                case TokenType.ModAssign:
                    return Precedence.Assign;
                default:
                    return Precedence.Lowest;
            }
        }
        public static bool RightToLeft(TokenType tokenType)
        {
            switch (tokenType)
            {
                case TokenType.Assign:
                case TokenType.AddAssign:
                case TokenType.SubAssign:
                case TokenType.MulAssign:
                case TokenType.DivAssign:
                case TokenType.ModAssign:
                    return true;
            }
            return false;
        }
    }
}
