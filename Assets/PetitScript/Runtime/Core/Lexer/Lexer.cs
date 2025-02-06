using System;
using System.Collections.Generic;

namespace Petit.Core.Lexer
{
    class Lexer
    {
        public Lexer()
        {
        }
        public IReadOnlyList<Token> Tokenize(string code)
        {
            _tokens.Clear();
            int lineNum = 1;
            foreach (string line in code.EnumerateSplitAndRemoveNewLine())
            {
                TokenizeLine(line, lineNum);
                ++lineNum;
            }
            return _tokens;
        }
        void TokenizeLine(string line, int lineNum)
        {
            Stack<bool> isInterpolation = new();
            int pos = 0;
            int length = line.Length;
            if (length <= 0)
            {
                return;
            }
            while(pos < length)
            {
                bool isInterpolationEnd = (isInterpolation.Count > 0 && !isInterpolation.Peek());
                // 空白スキップ
                while (!isInterpolationEnd && char.IsWhiteSpace(line[pos]))
                {
                    ++pos;
                    if (pos >= length)
                    {
                        return;
                    }
                }
                if (isInterpolationEnd || line[pos] == '"')
                {
                    // 文字列
                    if (!isInterpolationEnd)
                    {
                        _tokens.Add(new Token(TokenType.DoubleQuote, "\"", lineNum, pos + 1));
                        isInterpolation.Push(false);
                        ++pos;
                    }
                    else
                    {
                        // 文字列補間復帰
                    }
                    int start = pos;
                    while (pos < length)
                    {
                        if (line[pos] == '\"' && line[pos - 1] != '\\')
                        {
                            break;
                        }
                        if (line[pos] == '{')
                        {
                            if (pos + 1 < length)
                            {
                                if (line[pos + 1] == '{')
                                {
                                    ++pos;
                                }
                                else
                                {
                                    // 文字列補間開始
                                    if (isInterpolation.Count > 0)
                                    {
                                        isInterpolation.Pop();
                                        isInterpolation.Push(true);
                                    }
                                    break;
                                }
                            }
                        }
                        ++pos;
                    }
                    string text = line.Substring(start, pos - start)
                        .Replace("\\\"", "\"")
                        .Replace("{{", "{")
                        .Replace("}}", "}")
                        ;
                    text = System.Text.RegularExpressions.Regex.Unescape(text);
                    _tokens.Add(new Token(TokenType.Value, text, lineNum, start + 1));
                    if (!(isInterpolation.Count > 0 && isInterpolation.Peek()))
                    {
                        _tokens.Add(new Token(TokenType.DoubleQuote, "\"", lineNum, pos + 1));

                        // 文字列終了
                        if (isInterpolation.Count > 0)
                        {
                            isInterpolation.Pop();
                        }
                        ++pos;
                    }
                }
                else if(char.IsLetter(line[pos]) || line[pos] == '$')
                {
                    // 識別子
                    int start = pos;
                    ++pos;
                    while (pos < length && (char.IsLetterOrDigit(line[pos]) || line[pos] == '_'))
                    {
                        ++pos;
                    }
                    TokenType tokenType = TokenType.Ident;
                    string ident = line.Substring(start, pos - start);
                    if (ident == "true" || ident == "false")
                    {
                        tokenType = TokenType.Value;
                    }
                    else if (ident == "if")
                    {
                        tokenType = TokenType.If;
                    }
                    else if (ident == "else")
                    {
                        tokenType = TokenType.Else;
                    }
                    else if (ident == "while")
                    {
                        tokenType = TokenType.While;
                    }
                    else if (ident == "break")
                    {
                        tokenType = TokenType.Break;
                    }
                    else if (ident == "continue")
                    {
                        tokenType = TokenType.Continue;
                    }
                    else if (ident == "for")
                    {
                        tokenType = TokenType.For;
                    }
                    else if (ident == "return")
                    {
                        tokenType = TokenType.Return;
                    }
                    _tokens.Add(new Token(tokenType, line.Substring(start, pos - start), lineNum, start + 1));
                }
                else if (char.IsDigit(line[pos]))
                {
                    // 数
                    int start = pos;
                    bool isFoundDot = false;

                    ++pos;
                    while (pos < length)
                    {
                        if (char.IsDigit(line[pos]))
                        {
                            ++pos;
                        }
                        else if (!isFoundDot && line[pos] == '.' && pos + 1 < length && char.IsDigit(line[pos + 1]))
                        {
                            isFoundDot = true;
                            pos += 2;
                        }
                        else
                        {
                            break;
                        }
                    }
                    _tokens.Add(new Token(TokenType.Value, line.Substring(start, pos - start), lineNum, start + 1));
                }
                //else if (line[pos] == '@')
                //{
                //    _tokens.Add(new Token(TokenType.At, "@", lineNum, pos + 1));
                //    ++pos;
                //}
                else if (line[pos] == '(')
                {
                    _tokens.Add(new Token(TokenType.LParen, "(", lineNum, pos + 1));
                    ++pos;
                }
                else if (line[pos] == ')')
                {
                    _tokens.Add(new Token(TokenType.RParen, ")", lineNum, pos + 1));
                    ++pos;
                }
                //else if (line[pos] == '[')
                //{
                //    _tokens.Add(new Token(TokenType.LBracket, "[", lineNum, pos + 1));
                //    ++pos;
                //}
                //else if (line[pos] == ']')
                //{
                //    _tokens.Add(new Token(TokenType.RBracket, "]", lineNum, pos + 1));
                //    ++pos;
                //}
                else if (line[pos] == '{')
                {
                    _tokens.Add(new Token(TokenType.LBrace, "{", lineNum, pos + 1));
                    ++pos;
                }
                else if (line[pos] == '}')
                {
                    _tokens.Add(new Token(TokenType.RBrace, "}", lineNum, pos + 1));
                    ++pos;
                    if (isInterpolation.Count > 0 && isInterpolation.Peek())
                    {
                        isInterpolation.Pop();
                        isInterpolation.Push(false);
                    }
                }
                else if (line[pos] == ':')
                {
                    _tokens.Add(new Token(TokenType.Colon, ":", lineNum, pos + 1));
                    ++pos;
                }
                else if (line[pos] == ';')
                {
                    _tokens.Add(new Token(TokenType.Semicolon, ";", lineNum, pos + 1));
                    ++pos;
                }
                //else if (line[pos] == '#')
                //{
                //    _tokens.Add(new Token(TokenType.Sharp, "#", lineNum, pos + 1));
                //    ++pos;
                //}
                else if (line[pos] == '\\')
                {
                    _tokens.Add(new Token(TokenType.BackSlash, "\\", lineNum, pos + 1));
                    ++pos;
                }
                else if (line[pos] == '.')
                {
                    _tokens.Add(new Token(TokenType.Dot, ".", lineNum, pos + 1));
                    ++pos;
                }
                else if (line[pos] == '=')
                {
                    if (pos + 1 < length && line[pos + 1] == '=')
                    {
                        if (pos + 2 < length && line[pos + 2] == '=')
                        {
                            _tokens.Add(new Token(TokenType.Identical, "===", lineNum, pos + 1));
                            pos += 3;
                        }
                        else
                        {
                            _tokens.Add(new Token(TokenType.Equals, "==", lineNum, pos + 1));
                            pos += 2;
                        }
                    }
                    //else if (pos + 1 < length && line[pos + 1] == '>')
                    //{
                    //    _tokens.Add(new Token(TokenType.Arrow, "=>", lineNum, pos + 1));
                    //    pos += 2;
                    //}
                    else
                    {
                        _tokens.Add(new Token(TokenType.Assign, "=", lineNum, pos + 1));
                        ++pos;
                    }
                }
                else if (line[pos] == '<')
                {
                    if (pos + 1 < length && line[pos + 1] == '=')
                    {
                        if (pos + 2 < length && line[pos + 2] == '>')
                        {
                            _tokens.Add(new Token(TokenType.Spaceship, "<=>", lineNum, pos + 1));
                            pos += 3;
                        }
                        else
                        {
                            _tokens.Add(new Token(TokenType.LessThanOrEquals, "<=", lineNum, pos + 1));
                            pos += 2;
                        }
                    }
                    else
                    {
                        _tokens.Add(new Token(TokenType.LessThan, "<", lineNum, pos + 1));
                        ++pos;
                    }
                }
                else if (line[pos] == '>')
                {
                    if (pos + 1 < length && line[pos + 1] == '=')
                    {
                        _tokens.Add(new Token(TokenType.GreaterThanOrEquals, ">=", lineNum, pos + 1));
                        pos += 2;
                    }
                    else
                    {
                        _tokens.Add(new Token(TokenType.GreaterThan, ">", lineNum, pos + 1));
                        ++pos;
                    }
                }
                else if (line[pos] == '!')
                {
                    if (pos + 1 < length && line[pos + 1] == '=')
                    {
                        if (pos + 2 < length && line[pos + 2] == '=')
                        {
                            _tokens.Add(new Token(TokenType.NotIdentical, "!==", lineNum, pos + 1));
                            pos += 3;
                        }
                        else
                        {
                            _tokens.Add(new Token(TokenType.NotEquals, "!=", lineNum, pos + 1));
                            pos += 2;
                        }
                    }
                    else
                    {
                        _tokens.Add(new Token(TokenType.Not, "!", lineNum, pos + 1));
                        ++pos;
                    }
                }
                else if (line[pos] == '&' && pos + 1 < length && line[pos + 1] == '&')
                {
                    _tokens.Add(new Token(TokenType.LogicalAnd, "&&", lineNum, pos + 1));
                    pos += 2;
                }
                else if (line[pos] == '|' && pos + 1 < length && line[pos + 1] == '|')
                {
                    _tokens.Add(new Token(TokenType.LogicalOr, "||", lineNum, pos + 1));
                    pos += 2;
                }
                else if (line[pos] == '+')
                {
                    if (pos + 1 < length && line[pos + 1] == '=')
                    {
                        _tokens.Add(new Token(TokenType.AddAssign, "+=", lineNum, pos + 1));
                        pos += 2;
                    }
                    else
                    {
                        _tokens.Add(new Token(TokenType.Add, "+", lineNum, pos + 1));
                        ++pos;
                    }
                }
                else if (line[pos] == '-')
                {
                    if (pos + 1 < length && line[pos + 1] == '=')
                    {
                        _tokens.Add(new Token(TokenType.SubAssign, "-=", lineNum, pos + 1));
                        pos += 2;
                    }
                    else
                    {
                        _tokens.Add(new Token(TokenType.Sub, "-", lineNum, pos + 1));
                        ++pos;
                    }
                }
                else if (line[pos] == '*')
                {
                    if (pos + 1 < length && line[pos + 1] == '=')
                    {
                        _tokens.Add(new Token(TokenType.MulAssign, "*=", lineNum, pos + 1));
                        pos += 2;
                    }
                    else
                    {
                        _tokens.Add(new Token(TokenType.Mul, "*", lineNum, pos + 1));
                        ++pos;
                    }
                }
                else if (line[pos] == '/')
                {
                    if (pos + 1 < length && line[pos + 1] == '/')
                    {
                        // コメント
                        return;
                    }
                    else if (pos + 1 < length && line[pos + 1] == '=')
                    {
                        _tokens.Add(new Token(TokenType.DivAssign, "/=", lineNum, pos + 1));
                        pos += 2;
                    }
                    else
                    {
                        _tokens.Add(new Token(TokenType.Div, "/", lineNum, pos + 1));
                        ++pos;
                    }
                }
                else if (line[pos] == '%')
                {
                    if (pos + 1 < length && line[pos + 1] == '=')
                    {
                        _tokens.Add(new Token(TokenType.ModAssign, "%=", lineNum, pos + 1));
                        pos += 2;
                    }
                    else
                    {
                        _tokens.Add(new Token(TokenType.Mod, "%", lineNum, pos + 1));
                        ++pos;
                    }
                }
                else if (line[pos] == '?')
                {
                    _tokens.Add(new Token(TokenType.Question, "?", lineNum, pos + 1));
                    ++pos;
                }
            }

        }
        List<Token> _tokens = new List<Token>();
    }

    static class LexerHelper
    {
        public static IEnumerable<string> EnumerateSplitAndRemoveNewLine(this string self)
        {
            foreach (string line in self.SplitNewLine())
            {
                yield return line.RemoveNewline();
            }
        }
        public static string RemoveNewline(this string self)
        {
            return self.Replace("\n", string.Empty).Replace("\r", string.Empty);
        }
        public static string[] SplitNewLine(this string self)
        {
            return self.Split(_lineSeparators, StringSplitOptions.None);
        }
        static readonly string[] _lineSeparators = new[] { "\r\n", "\n", "\r" };
    }
}
