﻿namespace Petit.Syntax
{
    using TokenType = Petit.Syntax.Lexer.TokenType;

    enum Precedence
    {
        Dot,
        Not,
        Mul,
        Add,
        Shift,
        Spaceship,
        Comp,
        Equals,
        BitAnd,
        BitXor,
        BitOr,
        LogicalAnd,
        LogicalOr,
        Assign,
        Lowest,
    }

    static class PrecedenceExtensions
    {
        public static Precedence FromTokenType(TokenType tokenType, bool prefix = false)
        {
            if (prefix)
            {
                switch (tokenType)
                {
                    case TokenType.Not:
                    case TokenType.Plus:
                    case TokenType.Minus:
                    case TokenType.Inc:
                    case TokenType.Dec:
                    case TokenType.BitwiseNot:
                    case TokenType.BitwiseAnd: // address
                        return Precedence.Not;
                }
            }
            switch (tokenType)
            {
                case TokenType.Inc:
                case TokenType.Dec:
                case TokenType.LParen:   // Function call
                case TokenType.LBracket: // Subscript
                case TokenType.Dot:      // Member
                    return Precedence.Dot;

                case TokenType.Mul:
                case TokenType.Div:
                case TokenType.Mod:
                    return Precedence.Mul;

                case TokenType.Add:
                case TokenType.Sub:
                    return Precedence.Add;

                case TokenType.ShiftLeft:
                case TokenType.ShiftRight:
                    return Precedence.Shift;

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

                case TokenType.BitwiseAnd:
                    return Precedence.BitAnd;

                case TokenType.BitwiseXor:
                    return Precedence.BitXor;

                case TokenType.BitwiseOr:
                    return Precedence.BitOr;

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
                case TokenType.BitwiseAndAssign:
                case TokenType.BitwiseOrAssign:
                case TokenType.BitwiseXorAssign:
                case TokenType.ShiftLeftAssign:
                case TokenType.ShiftRightAssign:
                    return Precedence.Assign;
                default:
                    return Precedence.Lowest;
            }
        }
        public static bool RightToLeft(Precedence precedence)
        {
            switch (precedence)
            {
                case Precedence.Not:
                case Precedence.Assign:
                    return true;
            }
            return false;
        }
        public static bool RightToLeft(TokenType tokenType, bool prefix = false)
        {
            return RightToLeft(FromTokenType(tokenType, prefix));
        }
    }
}
