﻿using Petit.Core.AST;
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

        public GlobalStatement Parse(IReadOnlyList<Token> tokens)
        {
            if (_tokens != tokens)
            {
                this._tokens = tokens;
                this._pos = 0;
                _cache = ParseGlobalStatement();
            }
            return (_cache);
        }
        GlobalStatement ParseGlobalStatement()
        {
            var globalStatement = new GlobalStatement();
            while (_pos < _tokens.Count)
            {
                int prevPos = _pos;
                IStatement statement = ParseStatement();
                if (prevPos == _pos)
                {
                    // 無限ループ防止
                    ++_pos;
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
            if ( _pos < _tokens.Count)
            {
                if (_tokens[_pos].Type == TokenType.Semicolon)
                {
                    ++_pos;
                    return null;
                }
                else if (_tokens[_pos].Type == TokenType.LBrace)
                {
                    return ParseBlockStatement();
                }
                else if (_tokens[_pos].Type == TokenType.If)
                {
                    return ParseIfStatement();
                }
                else if (_tokens[_pos].Type == TokenType.Switch)
                {
                    return ParseSwitchStatement();
                }
                else if (_tokens[_pos].Type == TokenType.While)
                {
                    return ParseWhileStatement();
                }
                else if (_tokens[_pos].Type == TokenType.For)
                {
                    return ParseForStatement();
                }
                else if (_tokens[_pos].Type == TokenType.Break)
                {
                    return ParseBreakStatement();
                }
                else if (_tokens[_pos].Type == TokenType.Continue)
                {
                    return ParseContinueStatement();
                }
                else if (_tokens[_pos].Type == TokenType.Return)
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
            ++_pos; // {
            var block = new BlockStatement();
            while (_pos < _tokens.Count)
            {
                if (_tokens[_pos].Type == TokenType.RBrace)
                {
                    break;
                }
                int prevPos = _pos;
                IStatement statement = ParseStatement();
                if (prevPos == _pos)
                {
                    // 無限ループ防止
                    ++_pos;
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
            TryErrorCheckType("Not found '}'", TokenType.RBrace);
            ++_pos; // }
            return block;
        }
        IfStatement ParseIfStatement()
        {
            ++_pos; // if
            IfStatement statement = new IfStatement();

            IfParam ParseParam()
            {
                IfParam param = new IfParam();
                TryErrorCheckType("Not found if '('", TokenType.LParen);
                // (
                ++_pos;

                param.Cond = ParseExpression();

                TryErrorCheckType("Not found if ')'", TokenType.RParen);
                // )
                ++_pos;
                param.Statement = ParseStatement();
                return param;
            }
            statement.IfStatements.Add(ParseParam());
            while ((_pos + 1 < _tokens.Count) && _tokens[_pos].Type == TokenType.Else && _tokens[_pos + 1].Type == TokenType.If)
            {
                _pos += 2; // else if

                statement.IfStatements.Add(ParseParam());
            }
            if ((_pos < _tokens.Count) && _tokens[_pos].Type == TokenType.Else)
            {
                ++_pos; // else
                statement.ElseStatement = ParseStatement();
            }
            return statement;
        }
        SwitchStatement ParseSwitchStatement()
        {
            ++_pos; // switch
            SwitchStatement switchStatement = new SwitchStatement();
            TryErrorCheckType("Not Found while '('", TokenType.LParen);
            ++_pos; // (
            switchStatement.Condition = ParseExpression();

            TryErrorCheckType("Not Found while ')'", TokenType.RParen);
            ++_pos; // )

            if (TryCheckType(TokenType.LBrace))
            {
                _pos++; // {

                while (true)
                {
                    var section = ParseSwitchSection();
                    if (section is null)
                    {
                        break;
                    }
                    else
                    {
                        switchStatement.Sections.Add(section);
                    }
                }

                TryErrorCheckType("Not Found switch '}'", TokenType.RBrace);
                ++_pos; // }
            }
            else
            {
                var section = ParseSwitchSection(block: false);
                if (section != null)
                {
                    switchStatement.Sections.Add(section);
                }
            }
            return switchStatement;
        }
        SwitchSection ParseSwitchSection(bool block = true)
        {
            var section = new SwitchSection();
            while (_pos < _tokens.Count)
            {
                var label = ParseSwitchLabel();
                if (label is null)
                {
                    break;
                }
                section.Labels.Add(label);
            }
            if (section.Labels.Count <= 0)
            {
                // ラベル無し
                return null;
            }
            if (TryCheckType(TokenType.RBrace))
            {
                return section;
            }
            do
            {
                int prevPos = _pos;
                IStatement statement = ParseStatement();
                if (statement != null)
                {
                    section.Statements.Add(statement);
                }
                if (!block)
                {
                    // ブロックになってない場合は、一個まで
                    break;
                }
                if (TryCheckType(TokenType.Case) || TryCheckType(TokenType.Default) || TryCheckType(TokenType.RBrace))
                {
                    // 
                    break;
                }
                if (prevPos == _pos)
                {
                    // 無限ループ防止
                    ++_pos;
                }
            } while (_pos < _tokens.Count);
            return section;
        }
        ISwitchLabel ParseSwitchLabel()
        {
            ISwitchLabel label = null;
            if (TryCheckType(TokenType.Case))
            {
                _pos++; // case
                var expr = ParseExpression();
                if (expr != null)
                {
                    label = new SwitchCase()
                    {
                        Expression = expr
                    };
                }
            } 
            else if (TryCheckType(TokenType.Default))
            {
                _pos++; // default;
                label = new SwitchDefault();
            }
            else
            {
                return null;
            }

            TryErrorCheckType("Not found switch label ':'", TokenType.Colon);
            ++_pos; // :
            return label;
        }
        WhileStatement ParseWhileStatement()
        {
            ++_pos; // while
            WhileStatement statement = new WhileStatement();
            TryErrorCheckType("Not found while '('", TokenType.LParen);
            ++_pos; // (

            statement.Cond = ParseExpression();

            TryErrorCheckType("Not found while ')'", TokenType.RParen);
            // )
            ++_pos;
            statement.Statement = ParseStatement();
            return statement;
        }
        ForStatement ParseForStatement()
        {
            ++_pos; // for
            ForStatement statement = new ForStatement();
            TryErrorCheckType("Not found for (", TokenType.LParen);
            // (
            ++_pos;

            if (_pos < _tokens.Count && _tokens[_pos].Type != TokenType.Semicolon)
            {
                statement.Init = ParseExpression();
            }

            TryErrorCheckType("Not found for ';'", TokenType.Semicolon);
            // ;
            ++_pos;
            if (_pos < _tokens.Count && _tokens[_pos].Type != TokenType.Semicolon)
            {
                statement.Cond = ParseExpression();
            }
            TryErrorCheckType("Not found for ';'", TokenType.Semicolon);
            // ;
            ++_pos;

            if (_pos < _tokens.Count && _tokens[_pos].Type != TokenType.RParen)
            {
                statement.Loop = ParseExpression();
            }
            TryErrorCheckType("Not found for ')'", TokenType.RParen);
            // )
            ++_pos;
            statement.Statement = ParseStatement();
            return statement;
        }
        BreakStatement ParseBreakStatement()
        {
            var statement = new BreakStatement();
            ++_pos; // break;

            if (_pos < _tokens.Count && _tokens[_pos].Type == TokenType.Semicolon)
            {
                ++_pos;
            }
            return statement;
        }
        ContinueStatement ParseContinueStatement()
        {
            var statement = new ContinueStatement();
            ++_pos; // break;

            if (_pos < _tokens.Count && _tokens[_pos].Type == TokenType.Semicolon)
            {
                ++_pos;
            }

            return statement;
        }
        ReturnStatement ParseReturnStatement()
        {
            var statement = new ReturnStatement();
            ++_pos; // return
            if (_pos < _tokens.Count && _tokens[_pos].Type != TokenType.Semicolon)
            {
                statement.Expression = ParseExpression();
            }
            if (_pos < _tokens.Count && _tokens[_pos].Type == TokenType.Semicolon)
            {
                ++_pos;
            }
            return statement;
        }
        ExpressionStatement ParseExpressionStatement()
        {
            var statement = new ExpressionStatement();
            if (_pos < _tokens.Count)
            {
                statement.Expression = ParseExpression();
            }
            if (_pos < _tokens.Count && _tokens[_pos].Type == TokenType.Semicolon)
            {
                ++_pos;
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
                    case TokenType.Inc:
                    case TokenType.Dec:
                    case TokenType.BitwiseNot:
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
                    case TokenType.Inc:
                    case TokenType.Dec:
                        return ParsePostfixUnaryExpression;
                    case TokenType.LParen:
                        return ParseFunctionCallExpression;

                    case TokenType.Add:
                    case TokenType.Sub:

                    case TokenType.Mul:
                    case TokenType.Div:
                    case TokenType.Mod:

                    case TokenType.ShiftLeft:
                    case TokenType.ShiftRight:

                    case TokenType.Spaceship:

                    case TokenType.LessThan:
                    case TokenType.LessThanOrEquals:
                    case TokenType.GreaterThan:
                    case TokenType.GreaterThanOrEquals:

                    case TokenType.Equals:
                    case TokenType.Identical:
                    case TokenType.NotEquals:
                    case TokenType.NotIdentical:

                    case TokenType.BitwiseAnd:

                    case TokenType.BitwiseXor:

                    case TokenType.BitwiseOr:

                    case TokenType.LogicalAnd:

                    case TokenType.LogicalOr:

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
                        return ParseBinaryExpression;
                    case TokenType.Question:
                        return ParseTernaryExpression;
                }
                return null;
            }
            if (_pos >= _tokens.Count)
            {
                return null;
            }
            Func<IExpression> prefixOp = FindPrefixOp(_tokens[_pos].Type);
            if (prefixOp is null)
            {
                return null;
            }
            IExpression left = prefixOp();

            bool NeedParseLeft()
            {
                if (_pos >= _tokens.Count)
                {
                    return false;
                }
                return rightToLeft
                    ? (precedence >= PrecedenceExtensions.FromTokenType(_tokens[_pos].Type))
                    : (precedence > PrecedenceExtensions.FromTokenType(_tokens[_pos].Type))
                    ;
            }
            while (NeedParseLeft())
            {
                Func<IExpression, IExpression> binaryOp = FindBinaryOp(_tokens[_pos].Type);
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
            string literal = _tokens[_pos].Value;
            ++_pos;
            return new LiteralExpression()
            {
                Value = literal
            };
        }
        StringExpression ParseStringExpression()
        {
            var expr = new StringExpression();
            ++_pos; // "
            while (_pos < _tokens.Count)
            {
                if (_tokens[_pos].Type == TokenType.Value)
                {
                    expr.Expressions.Add(ParseLiteralExpression());
                }
                else if (_tokens[_pos].Type == TokenType.LBrace)
                {
                    ++_pos; // {
                    // 補完文字列
                    var inner = ParseExpression();
                    if (inner != null)
                    {
                        expr.Expressions.Add(inner);
                    }
                    TryErrorCheckType("Not found '}' by string interpolation", TokenType.RBrace);
                    ++_pos; // }
                }
                else if (_tokens[_pos].Type == TokenType.DoubleQuote)
                {
                    break;
                }
                else
                {
                    ++_pos;
                }
            }
            TryErrorCheckType("Not found string '\"'", TokenType.DoubleQuote);
            ++_pos; // }
            return expr;
        }
        VariableExpression ParseVariableExpression()
        {
            string ident = _tokens[_pos].Value;
            ++_pos;
            return new VariableExpression()
            {
                Ident = ident
            };
        }
        PrefixUnaryExpression ParsePrefixUnaryExpression()
        {
            Precedence precedence = PrecedenceExtensions.FromTokenType(_tokens[_pos].Type, prefix: true);
            string op = _tokens[_pos].Value;
            ++_pos;
            bool rightToLeft = PrecedenceExtensions.RightToLeft(precedence);
            IExpression right = ParseExpression(precedence);
            return new PrefixUnaryExpression()
            {
                Op = op,
                Right = right,
            };
        }
        PostfixUnaryExpression ParsePostfixUnaryExpression(IExpression left)
        {
            string op = _tokens[_pos].Value;
            ++_pos;
            return new PostfixUnaryExpression()
            {
                Left = left,
                Op = op,
            };
        }
        BinaryExpression ParseBinaryExpression(IExpression left)
        {
            Precedence precedence = PrecedenceExtensions.FromTokenType(_tokens[_pos].Type);
            string op = _tokens[_pos].Value;
            ++_pos;

            bool rightToLeft = PrecedenceExtensions.RightToLeft(precedence);
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
            Precedence precedence = PrecedenceExtensions.FromTokenType(_tokens[_pos].Type);
            string op = _tokens[_pos].Value;
            ++_pos;
            IExpression mid = ParseExpression(precedence, rightToLeft: true);
            if (_pos >= _tokens.Count)
            {
                Error($"Not found infix operator with '{op}'");
                return null;
            }
            string op2 = _tokens[_pos].Value;
            ++_pos;
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
        FunctionCallExpression ParseFunctionCallExpression(IExpression left)
        {
            var expr = new FunctionCallExpression();
            expr.Function = left;
            string op = _tokens[_pos].Value;
            ++_pos; // (

            // Argument
            do
            {
                AST.Argument arg = default;
                bool useName = false;
                if (TryCheckNextType(TokenType.Ident, TokenType.Colon))
                {
                    // 名前付き引数
                    arg.Name = _tokens[_pos].Value;
                    _pos += 2; // name:
                    useName = true;
                }
                var value = ParseExpression();
                if (value is null)
                {
                    if (useName)
                    {
                        // 名前付きなのに式がない
                        Error($"Not found function argument with {arg.Name}");
                    }
                    break;
                }
                arg.Expression = value;
                expr.Args.Add(arg);

                if (TryCheckType(TokenType.Comma))
                {
                    ++_pos; // ,
                }
                else
                {
                    break; // 引数終わり
                }
            } while (true);
            TryErrorCheckType("Not found function call')'", TokenType.RParen);
            ++_pos; // )
            return expr;
        }
        IExpression ParseParen()
        {
            // (
            ++_pos;
            IExpression expr = ParseExpression();

            TryErrorCheckType("Not found ')'", TokenType.RParen);
            // )
            ++_pos;
            return expr;
        }
        bool TryCheckType(TokenType type)
        {
            return _pos < _tokens.Count && _tokens[_pos].Type == type;
        }
        bool TryCheckNextType(TokenType type, TokenType typeNext)
        {
            return _pos + 1 < _tokens.Count && _tokens[_pos].Type == type && _tokens[_pos + 1].Type == typeNext;
        }
        bool TryErrorCheckType(string message, TokenType type)
        {
            if (_pos < _tokens.Count && _tokens[_pos].Type == type)
            {
                return false;
            }
            else
            {
                Error(message);
                return true;
            }
        }
        void Error(string message) => Error(message, _pos - 1);
        void Error(string message, int pos)
        {
            Token errorToken = pos >= _tokens.Count ? _tokens[_tokens.Count - 1] : _tokens[pos];
            throw new Core.Exception.SyntaxErrorException(message, errorToken.Line, errorToken.Column);
        }
        IReadOnlyList<Token> _tokens;
        int _pos;
        GlobalStatement _cache;
    }
}
