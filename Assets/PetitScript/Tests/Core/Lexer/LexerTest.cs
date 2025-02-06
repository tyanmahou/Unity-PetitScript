using NUnit.Framework;

namespace Petit.Core.Lexer
{
    class LexerTest
    {
        [Test]
        public void TestBool()
        {
            var lexer = new Lexer();
            {
                var tokens = lexer.Tokenize("true");

                Assert.AreEqual(tokens.Count, 1);
                Assert.AreEqual(tokens[0].Value, "true");
                Assert.AreEqual(tokens[0].Type, TokenType.Value);
            }
            {
                var tokens = lexer.Tokenize("false");

                Assert.AreEqual(tokens.Count, 1);
                Assert.AreEqual(tokens[0].Value, "false");
                Assert.AreEqual(tokens[0].Type, TokenType.Value);
            }
        }
        [Test]
        public void TestNumber()
        {
            var lexer = new Lexer();
            {
                var tokens = lexer.Tokenize("123");

                Assert.AreEqual(tokens.Count, 1);
                Assert.AreEqual(tokens[0].Value, "123");
                Assert.AreEqual(tokens[0].Type, TokenType.Value);
            }
            {
                var tokens = lexer.Tokenize("45.06789");

                Assert.AreEqual(tokens.Count, 1);
                Assert.AreEqual(tokens[0].Value, "45.06789");
                Assert.AreEqual(tokens[0].Type, TokenType.Value);
            }
            {
                var tokens = lexer.Tokenize("+1");

                Assert.AreEqual(tokens.Count, 2);
                Assert.AreEqual(tokens[0].Value, "+");
                Assert.AreEqual(tokens[0].Type, TokenType.Plus);
                Assert.AreEqual(tokens[1].Value, "1");
                Assert.AreEqual(tokens[1].Type, TokenType.Value);
            }
            {
                var tokens = lexer.Tokenize("-1");

                Assert.AreEqual(tokens.Count, 2);
                Assert.AreEqual(tokens[0].Value, "-");
                Assert.AreEqual(tokens[0].Type, TokenType.Minus);
                Assert.AreEqual(tokens[1].Value, "1");
                Assert.AreEqual(tokens[1].Type, TokenType.Value);
            }
        }
        [Test]
        public void TestString()
        {
            var lexer = new Lexer();
            {
                var tokens = lexer.Tokenize("\"aaaa\"");

                Assert.AreEqual(tokens.Count, 1);
                Assert.AreEqual(tokens[0].Value, "aaaa");
                Assert.AreEqual(tokens[0].Type, TokenType.Value);
            }
        }
        [Test]
        public void TestIdent()
        {
            var lexer = new Lexer();
            {
                var tokens = lexer.Tokenize("abcd");

                Assert.AreEqual(tokens.Count, 1);
                Assert.AreEqual(tokens[0].Value, "abcd");
                Assert.AreEqual(tokens[0].Type, TokenType.Ident);
            }
            {
                var tokens = lexer.Tokenize("$a0_1-2");

                Assert.AreEqual(tokens.Count,3);
                Assert.AreEqual(tokens[0].Value, "$a0_1");
                Assert.AreEqual(tokens[0].Type, TokenType.Ident);
                Assert.AreEqual(tokens[1].Value, "-");
                Assert.AreEqual(tokens[1].Type, TokenType.Sub);
                Assert.AreEqual(tokens[2].Value, "2");
                Assert.AreEqual(tokens[2].Type, TokenType.Value);
            }
        }
        [Test]
        public void TestComp()
        {
            var lexer = new Lexer();
            {
                var tokens = lexer.Tokenize("a == b");

                Assert.AreEqual(tokens.Count, 3);
                Assert.AreEqual(tokens[0].Value, "a");
                Assert.AreEqual(tokens[0].Type, TokenType.Ident);
                Assert.AreEqual(tokens[1].Value, "==");
                Assert.AreEqual(tokens[1].Type, TokenType.Equals);
                Assert.AreEqual(tokens[2].Value, "b");
                Assert.AreEqual(tokens[2].Type, TokenType.Ident);
            }

            {
                var tokens = lexer.Tokenize("a === b");

                Assert.AreEqual(tokens.Count, 3);
                Assert.AreEqual(tokens[0].Value, "a");
                Assert.AreEqual(tokens[0].Type, TokenType.Ident);
                Assert.AreEqual(tokens[1].Value, "===");
                Assert.AreEqual(tokens[1].Type, TokenType.Identical);
                Assert.AreEqual(tokens[2].Value, "b");
                Assert.AreEqual(tokens[2].Type, TokenType.Ident);
            }
            {
                var tokens = lexer.Tokenize("a != b");

                Assert.AreEqual(tokens.Count, 3);
                Assert.AreEqual(tokens[0].Value, "a");
                Assert.AreEqual(tokens[0].Type, TokenType.Ident);
                Assert.AreEqual(tokens[1].Value, "!=");
                Assert.AreEqual(tokens[1].Type, TokenType.NotEquals);
                Assert.AreEqual(tokens[2].Value, "b");
                Assert.AreEqual(tokens[2].Type, TokenType.Ident);
            }
            {
                var tokens = lexer.Tokenize("a !== b");

                Assert.AreEqual(tokens.Count, 3);
                Assert.AreEqual(tokens[0].Value, "a");
                Assert.AreEqual(tokens[0].Type, TokenType.Ident);
                Assert.AreEqual(tokens[1].Value, "!==");
                Assert.AreEqual(tokens[1].Type, TokenType.NotIdentical);
                Assert.AreEqual(tokens[2].Value, "b");
                Assert.AreEqual(tokens[2].Type, TokenType.Ident);
            }
            {
                var tokens = lexer.Tokenize("a > b");

                Assert.AreEqual(tokens.Count, 3);
                Assert.AreEqual(tokens[0].Value, "a");
                Assert.AreEqual(tokens[0].Type, TokenType.Ident);
                Assert.AreEqual(tokens[1].Value, ">");
                Assert.AreEqual(tokens[1].Type, TokenType.GreaterThan);
                Assert.AreEqual(tokens[2].Value, "b");
                Assert.AreEqual(tokens[2].Type, TokenType.Ident);
            }
            {
                var tokens = lexer.Tokenize("a >= b");

                Assert.AreEqual(tokens.Count, 3);
                Assert.AreEqual(tokens[0].Value, "a");
                Assert.AreEqual(tokens[0].Type, TokenType.Ident);
                Assert.AreEqual(tokens[1].Value, ">=");
                Assert.AreEqual(tokens[1].Type, TokenType.GreaterThanOrEquals);
                Assert.AreEqual(tokens[2].Value, "b");
                Assert.AreEqual(tokens[2].Type, TokenType.Ident);
            }
            {
                var tokens = lexer.Tokenize("a < b");

                Assert.AreEqual(tokens.Count, 3);
                Assert.AreEqual(tokens[0].Value, "a");
                Assert.AreEqual(tokens[0].Type, TokenType.Ident);
                Assert.AreEqual(tokens[1].Value, "<");
                Assert.AreEqual(tokens[1].Type, TokenType.LessThan);
                Assert.AreEqual(tokens[2].Value, "b");
                Assert.AreEqual(tokens[2].Type, TokenType.Ident);
            }
            {
                var tokens = lexer.Tokenize("a <= b");

                Assert.AreEqual(tokens.Count, 3);
                Assert.AreEqual(tokens[0].Value, "a");
                Assert.AreEqual(tokens[0].Type, TokenType.Ident);
                Assert.AreEqual(tokens[1].Value, "<=");
                Assert.AreEqual(tokens[1].Type, TokenType.LessThanOrEquals);
                Assert.AreEqual(tokens[2].Value, "b");
                Assert.AreEqual(tokens[2].Type, TokenType.Ident);
            }
            {
                var tokens = lexer.Tokenize("a <=> b");

                Assert.AreEqual(tokens.Count, 3);
                Assert.AreEqual(tokens[0].Value, "a");
                Assert.AreEqual(tokens[0].Type, TokenType.Ident);
                Assert.AreEqual(tokens[1].Value, "<=>");
                Assert.AreEqual(tokens[1].Type, TokenType.Spaceship);
                Assert.AreEqual(tokens[2].Value, "b");
                Assert.AreEqual(tokens[2].Type, TokenType.Ident);
            }
        }
        [Test]
        public void TestLogical()
        {
            var lexer = new Lexer();
            {
                var tokens = lexer.Tokenize("!a");

                Assert.AreEqual(tokens.Count, 2);
                Assert.AreEqual(tokens[0].Value, "!");
                Assert.AreEqual(tokens[0].Type, TokenType.Not);
                Assert.AreEqual(tokens[1].Value, "a");
                Assert.AreEqual(tokens[1].Type, TokenType.Ident);
            }
            {
                var tokens = lexer.Tokenize("a && b");

                Assert.AreEqual(tokens.Count, 3);
                Assert.AreEqual(tokens[0].Value, "a");
                Assert.AreEqual(tokens[0].Type, TokenType.Ident);
                Assert.AreEqual(tokens[1].Value, "&&");
                Assert.AreEqual(tokens[1].Type, TokenType.LogicalAnd);
                Assert.AreEqual(tokens[2].Value, "b");
                Assert.AreEqual(tokens[2].Type, TokenType.Ident);
            }
            {
                var tokens = lexer.Tokenize("a || b");

                Assert.AreEqual(tokens.Count, 3);
                Assert.AreEqual(tokens[0].Value, "a");
                Assert.AreEqual(tokens[0].Type, TokenType.Ident);
                Assert.AreEqual(tokens[1].Value, "||");
                Assert.AreEqual(tokens[1].Type, TokenType.LogicalOr);
                Assert.AreEqual(tokens[2].Value, "b");
                Assert.AreEqual(tokens[2].Type, TokenType.Ident);
            }
        }
        [Test]
        public void TestParen()
        {
            var lexer = new Lexer();
            {
                var tokens = lexer.Tokenize("(a)");

                Assert.AreEqual(tokens.Count, 3);
                Assert.AreEqual(tokens[0].Value, "(");
                Assert.AreEqual(tokens[0].Type, TokenType.LParen);
                Assert.AreEqual(tokens[1].Value, "a");
                Assert.AreEqual(tokens[1].Type, TokenType.Ident);
                Assert.AreEqual(tokens[2].Value, ")");
                Assert.AreEqual(tokens[2].Type, TokenType.RParen);
            }
        }
        [Test]
        public void TestCond()
        {
            var lexer = new Lexer();
            {
                var tokens = lexer.Tokenize("a ? b : c");

                Assert.AreEqual(tokens.Count, 5);
                Assert.AreEqual(tokens[0].Value, "a");
                Assert.AreEqual(tokens[0].Type, TokenType.Ident);
                Assert.AreEqual(tokens[1].Value, "?");
                Assert.AreEqual(tokens[1].Type, TokenType.Question);
                Assert.AreEqual(tokens[2].Value, "b");
                Assert.AreEqual(tokens[2].Type, TokenType.Ident);
                Assert.AreEqual(tokens[3].Value, ":");
                Assert.AreEqual(tokens[3].Type, TokenType.Colon);
                Assert.AreEqual(tokens[4].Value, "c");
                Assert.AreEqual(tokens[4].Type, TokenType.Ident);
            }
        }
        [Test]
        public void TestMultiStatement()
        {
            var lexer = new Lexer();
            {
                var tokens = lexer.Tokenize("a=10;a+=2;");

                Assert.AreEqual(tokens.Count, 8);
                Assert.AreEqual(tokens[0].Value, "a");
                Assert.AreEqual(tokens[0].Type, TokenType.Ident);
                Assert.AreEqual(tokens[1].Value, "=");
                Assert.AreEqual(tokens[1].Type, TokenType.Assign);
                Assert.AreEqual(tokens[2].Value, "10");
                Assert.AreEqual(tokens[2].Type, TokenType.Value);
                Assert.AreEqual(tokens[3].Value, ";");
                Assert.AreEqual(tokens[3].Type, TokenType.Semicolon);
                Assert.AreEqual(tokens[4].Value, "a");
                Assert.AreEqual(tokens[4].Type, TokenType.Ident);
                Assert.AreEqual(tokens[5].Value, "+=");
                Assert.AreEqual(tokens[5].Type, TokenType.AddAssign);
                Assert.AreEqual(tokens[6].Value, "2");
                Assert.AreEqual(tokens[6].Type, TokenType.Value);
                Assert.AreEqual(tokens[7].Value, ";");
                Assert.AreEqual(tokens[7].Type, TokenType.Semicolon);
            }
        }
    }
}
