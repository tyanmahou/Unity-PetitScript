﻿namespace Petit.Core.Parser
{
    using TokenType = Petit.Core.Lexer.TokenType;

    enum Precedence
    {
        Dot,
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
                        return Precedence.Not;
                }
            }
            switch (tokenType)
            {
                case TokenType.Inc:
                case TokenType.Dec:
                    return Precedence.Dot;
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
        public static bool RightToLeft(TokenType tokenType, bool prefix = false)
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
                        return true;
                }
            }
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
