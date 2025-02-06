using Petit.Core.AST;
using Petit.Core.Lexer;
using System;
using System.Collections.Generic;

namespace Petit.Core.Parser
{
    class Parser
    {
        public Parser()
        {
        }

        public (GlobalStatement, IReadOnlyList<SyntaxError>) Parse(IReadOnlyList<Token> tokens)
        {
            if (_tokens != tokens)
            {
                this._tokens = tokens;
                this._errors.Clear();
                this._iteratorPos = 0;
                _cache = ParseGlobalStatement();
            }
            return (_cache, _errors);
        }
        GlobalStatement ParseGlobalStatement()
        {
            var globalStatement = new GlobalStatement();
            while (_iteratorPos < _tokens.Count)
            {
                int prevPos = _iteratorPos;
                IStatement statement = ParseStatement();
                if (prevPos == _iteratorPos)
                {
                    // 無限ループ防止
                    ++_iteratorPos;
                }
                if (statement != null)
                {
                    globalStatement.Statements.Add(statement);
                }
                else
                {
                    continue;
                }
            }
            return globalStatement;
        }
        IStatement ParseStatement()
        {
            if ( _iteratorPos < _tokens.Count)
            {
                if (_tokens[_iteratorPos].Type == TokenType.Semicolon)
                {
                    ++_iteratorPos;
                    return null;
                }
                else if (_tokens[_iteratorPos].Type == TokenType.LBrace)
                {
                    return ParseBlockStatement();
                }
                else if (_tokens[_iteratorPos].Type == TokenType.If)
                {
                    return ParseIfStatement();
                }
                else if (_tokens[_iteratorPos].Type == TokenType.Return)
                {
                    return ParseReturnStatement();
                }
                else
                {
                    return ParseExpressionStatement();
                }
            }
            return null;
        }
        BlockStatement ParseBlockStatement()
        {
            ++_iteratorPos; // {
            var block = new BlockStatement();
            while (_iteratorPos < _tokens.Count)
            {
                if (_tokens[_iteratorPos].Type == TokenType.RBrace)
                {
                    break;
                }
                int prevPos = _iteratorPos;
                IStatement statement = ParseStatement();
                if (prevPos == _iteratorPos)
                {
                    // 無限ループ防止
                    ++_iteratorPos;
                }
                if (statement != null)
                {
                    block.Statements.Add(statement);
                }
                else
                {
                    continue;
                }
            }
            if (_iteratorPos >= _tokens.Count || _tokens[_iteratorPos].Type != TokenType.RBrace)
            {
                Error("Not Found '}'");
            }
            ++_iteratorPos; // }
            return block;
        }
        IfStatement ParseIfStatement()
        {
            ++_iteratorPos; // if
            IfStatement statement = new IfStatement();

            IfParam ParseParam()
            {
                IfParam param = new IfParam();
                if (_iteratorPos < _tokens.Count && _tokens[_iteratorPos].Type == TokenType.LParen)
                {
                    // (
                    ++_iteratorPos;
                }
                else
                {
                    Error("Not Found if '('");
                }
                param.Cond = ParseExpression();
                if (_iteratorPos < _tokens.Count && _tokens[_iteratorPos].Type == TokenType.RParen)
                {
                    // )
                    ++_iteratorPos;
                }
                else
                {
                    Error("Not Found if ')'");
                }
                param.Statement = ParseStatement();
                return param;
            }
            statement.IfStatements.Add(ParseParam());
            while ((_iteratorPos + 1 < _tokens.Count) && _tokens[_iteratorPos].Type == TokenType.Else && _tokens[_iteratorPos + 1].Type == TokenType.If)
            {
                _iteratorPos += 2; // else if

                statement.IfStatements.Add(ParseParam());
            }
            if ((_iteratorPos < _tokens.Count) && _tokens[_iteratorPos].Type == TokenType.Else)
            {
                ++_iteratorPos; // else
                statement.ElseStatement = ParseStatement();
            }
            return statement;
        }
        ReturnStatement ParseReturnStatement()
        {
            var statement = new ReturnStatement();
            ++_iteratorPos; // return
            if (_iteratorPos < _tokens.Count)
            {
                statement.Expression = ParseExpression();
            }
            if (_iteratorPos < _tokens.Count && _tokens[_iteratorPos].Type == TokenType.Semicolon)
            {
                ++_iteratorPos;
            }
            return statement;
        }
        ExpressionStatement ParseExpressionStatement()
        {
            var statement = new ExpressionStatement();
            if (_iteratorPos < _tokens.Count)
            {
                statement.Expression = ParseExpression();
            }
            if (_iteratorPos < _tokens.Count && _tokens[_iteratorPos].Type == TokenType.Semicolon)
            {
                ++_iteratorPos;
            }
            return statement;
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
                    case TokenType.DoubleQuote:
                        return ParseStringExpression;
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
                    case TokenType.Identical:
                    case TokenType.NotEquals:
                    case TokenType.NotIdentical:
                    case TokenType.LogicalOr:
                    case TokenType.LogicalAnd:
                    case TokenType.Assign:
                    case TokenType.AddAssign:
                    case TokenType.SubAssign:
                    case TokenType.MulAssign:
                    case TokenType.DivAssign:
                    case TokenType.ModAssign:
                        return ParseBinaryExpression;
                    case TokenType.Question:
                        return ParseTernaryExpression;
                }
                return null;
            }
            if (_iteratorPos >= _tokens.Count)
            {
                return null;
            }
            Func<IExpression> prefixOp = FindPrefixOp(_tokens[_iteratorPos].Type);
            if (prefixOp is null)
            {
                ++_iteratorPos;
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
        StringExpression ParseStringExpression()
        {
            var expr = new StringExpression();
            ++_iteratorPos; // "
            while (_iteratorPos < _tokens.Count)
            {
                if (_tokens[_iteratorPos].Type == TokenType.Value)
                {
                    expr.Expressions.Add(ParseLiteralExpression());
                }
                else if (_tokens[_iteratorPos].Type == TokenType.LBrace)
                {
                    ++_iteratorPos; // {
                    // 補完文字列
                    var inner = ParseExpression();
                    if (inner != null)
                    {
                        expr.Expressions.Add(inner);
                    }
                    if (_iteratorPos < _tokens.Count && _tokens[_iteratorPos].Type == TokenType.RBrace)
                    {
                    }
                    else
                    {
                        Error("Not Found }");
                    }
                    ++_iteratorPos; // }
                }
                else if (_tokens[_iteratorPos].Type == TokenType.DoubleQuote)
                {
                    break;
                }
                else
                {
                    ++_iteratorPos;
                }
            }
            if (_iteratorPos < _tokens.Count && _tokens[_iteratorPos].Type == TokenType.DoubleQuote)
            {
            }
            else
            {
                Error("Not Found \"");
            }
            ++_iteratorPos; // }
            return expr;
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
            TokenType tokenType = _tokens[_iteratorPos].Type;
            Precedence precedence = PrecedenceExtensions.FromTokenType(tokenType);
            string op = _tokens[_iteratorPos].Value;
            ++_iteratorPos;

            bool rightToLeft = PrecedenceExtensions.RightToLeft(tokenType);
            IExpression right = ParseExpression(precedence, rightToLeft);
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
        GlobalStatement _cache;
        List<SyntaxError> _errors = new List<SyntaxError>();
    }
}
