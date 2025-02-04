using Petit.AST;
using Petit.Lexer;
using System;
using System.Collections.Generic;

namespace Petit.Parser
{
    class Parser
    {
        public Parser()
        {
        }

        public (AST.Root, IReadOnlyList<SyntaxError>) Parse(IReadOnlyList<Token> tokens)
        {
            if (_tokens != tokens)
            {
                this._tokens = tokens;
                this._errors.Clear();
                this._iteratorPos = 0;
                _cache = ParseRoot();
            }
            return (_cache, _errors);
        }
        Root ParseRoot()
        {
            var root = new Root();
            if (_iteratorPos < _tokens.Count)
            {
                root.Statement = ParseGlobalStatement();
            }
            return root;
        }
        GlobalStatement ParseGlobalStatement()
        {
            var globalStatement = new GlobalStatement();

            // 式を評価
            globalStatement.Expression = ParseExpression();

            return globalStatement;
        }
        IExpression ParseExpression()
        {
            return ParseExpression(Precedence.Lowest);
        }
        IExpression ParseExpression(Precedence precedence, bool rightToLeft = false)
        {

            Func<IExpression> FindPrefixOp(TokenType tokenType)
            {
                switch (tokenType)
                {
                    case TokenType.Not:
                    case TokenType.Plus:
                    case TokenType.Minus:
                        return ParsePrefixUnaryExpression;
                    case TokenType.LParen:
                        return ParseParen;
                    case TokenType.Value:
                        return ParseLiteralExpression;
                    case TokenType.Ident:
                        return ParseVariableExpression;
                }
                return null;
            }
            Func<IExpression, IExpression> FindBinaryOp(TokenType tokenType)
            {
                switch (tokenType)
                {
                    case TokenType.Add:
                    case TokenType.Sub:
                    case TokenType.Mul:
                    case TokenType.Div:
                    case TokenType.Mod:
                    case TokenType.LessThan:
                    case TokenType.LessThanOrEquals:
                    case TokenType.GreaterThan:
                    case TokenType.GreaterThanOrEquals:
                    case TokenType.Spaceship:
                    case TokenType.Equals:
                    case TokenType.EqualsStrict:
                    case TokenType.NotEquals:
                    case TokenType.NotEqualsStrict:
                    case TokenType.LogicalOr:
                    case TokenType.LogicalAnd:
                        return ParseBinaryExpression;
                    case TokenType.Question:
                        return ParseTernaryExpression;
                }
                return null;
            }
            Func<IExpression> prefixOp = FindPrefixOp(_tokens[_iteratorPos].Type);
            if (prefixOp is null)
            {
                return null;
            }
            IExpression left = prefixOp();

            bool NeedParseLeft()
            {
                if (_iteratorPos >= _tokens.Count)
                {
                    return false;
                }
                return rightToLeft
                    ? (precedence >= PrecedenceExtensions.FromTokenType(_tokens[_iteratorPos].Type))
                    : (precedence > PrecedenceExtensions.FromTokenType(_tokens[_iteratorPos].Type))
                    ;
            }
            while (NeedParseLeft())
            {
                Func<IExpression, IExpression> binaryOp = FindBinaryOp(_tokens[_iteratorPos].Type);
                if (binaryOp is null)
                {
                    return left;
                }
                left = binaryOp(left);
            }

            return left;
        }
        LiteralExpression ParseLiteralExpression()
        {
            string literal = _tokens[_iteratorPos].Value;
            ++_iteratorPos;
            return new LiteralExpression()
            {
                Value = literal
            };
        }
        VariableExpression ParseVariableExpression()
        {
            string ident = _tokens[_iteratorPos].Value;
            ++_iteratorPos;
            return new VariableExpression()
            {
                Ident = ident
            };
        }
        PrefixUnaryExpression ParsePrefixUnaryExpression()
        {
            Precedence precedence = PrecedenceExtensions.FromTokenType(_tokens[_iteratorPos].Type, unary: true);
            string op = _tokens[_iteratorPos].Value;
            ++_iteratorPos;
            IExpression right = ParseExpression(precedence);
            return new PrefixUnaryExpression()
            {
                Op = op,
                Right = right,
            };
        }
        BinaryExpression ParseBinaryExpression(IExpression left)
        {
            Precedence precedence = PrecedenceExtensions.FromTokenType(_tokens[_iteratorPos].Type);
            string op = _tokens[_iteratorPos].Value;
            ++_iteratorPos;
            IExpression right = ParseExpression(precedence);
            return new BinaryExpression()
            {
                Left = left,
                Op = op,
                Right = right,
            };
        }
        TernaryExpression ParseTernaryExpression(IExpression left)
        {
            Precedence precedence = PrecedenceExtensions.FromTokenType(_tokens[_iteratorPos].Type);
            string op = _tokens[_iteratorPos].Value;
            ++_iteratorPos;
            IExpression mid = ParseExpression(precedence, rightToLeft: true);
            if (_iteratorPos >= _tokens.Count)
            {
                Error("Not Found ternary operator");
                return null;
            }
            string op2 = _tokens[_iteratorPos].Value;
            ++_iteratorPos;
            IExpression right = ParseExpression(precedence, rightToLeft: true);

            return new TernaryExpression()
            {
                Left = left,
                Op = op,
                Mid = mid,
                Op2 = op2,
                Right = right,
            };
        }
        IExpression ParseParen()
        {
            // (
            ++_iteratorPos;
            IExpression expr = ParseExpression();
            if (_iteratorPos >= _tokens.Count || _tokens[_iteratorPos].Type != TokenType.RParen)
            {
                Error("Not Found ')'");
            }
            // )
            ++_iteratorPos;
            return expr;
        }
        void Error(string message) => Error(message, _iteratorPos - 1, head: false);
        void Error(string message, int pos, bool head = true)
        {
            Token errorToken = pos >= _tokens.Count ? _tokens[_tokens.Count - 1] : _tokens[pos];
            _errors.Add(new SyntaxError(message, errorToken.Line, errorToken.Column + (head ? 0 : errorToken.Value.Length)));
        }
        IReadOnlyList<Token> _tokens;
        int _iteratorPos;
        Root _cache;
        List<SyntaxError> _errors = new List<SyntaxError>();
    }
}
