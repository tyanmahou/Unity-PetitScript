using NUnit.Framework;
using Petit.Syntax.AST;
using Petit.Syntax.Exception;

namespace Petit.Syntax.Parser
{
    class ParserTest
    {
        [Test]
        public void TestGloabalStatement()
        {
            string code = @"a";
            var root = Parse(code);
            Assert.AreEqual(root.Statements.Count, 1);

            var e = As<ExpressionStatement>(root?.Statements?[0]);
            Assert.True(e?.Expression != null);
        }
        [Test]
        public void TestPrefixUnaryExpression()
        {
            string code = @"-1";
            var expr = GetExpr(code);
            var unary = As<AST.PrefixUnaryExpression>(expr);
            Assert.AreEqual(unary.Op, "-");
            var right = As<AST.LiteralExpression>(unary.Right);
            Assert.AreEqual(right.Value, "1");
        }
        [Test]
        public void TestBinaryExpression()
        {
            string code = @"a >= 1";
            var expr = GetExpr(code);
            var binary = As<AST.BinaryExpression>(expr);
            var left = As<AST.VariableExpression>(binary.Left);
            Assert.AreEqual(left.Ident, "a");
            Assert.AreEqual(binary.Op, ">=");

            var right = As<AST.LiteralExpression>(binary.Right);
            Assert.AreEqual(right.Value, "1");
        }
        [Test]
        public void TestTernaryExpression()
        {
            string code = @"a ? true : false";
            var expr = GetExpr(code);
            var terrnary = As<AST.TernaryExpression>(expr);
            var left = As<AST.VariableExpression>(terrnary.Left);
            Assert.AreEqual(left.Ident, "a");
            Assert.AreEqual(terrnary.Op, "?");
            var mid = As<AST.LiteralExpression>(terrnary.Mid);
            Assert.AreEqual(mid.Value, "true");
            Assert.AreEqual(terrnary.Op2, ":");
            var right = As<AST.LiteralExpression>(terrnary.Right);
            Assert.AreEqual(right.Value, "false");
        }
        [Test]
        public void TestLogical()
        {
            string code = @"a || b && c";
            var expr = GetExpr(code);

            var b = As<AST.BinaryExpression>(expr);
            Assert.AreEqual(b.Op, "||");
            var l = As<AST.VariableExpression>(b.Left);
            Assert.AreEqual(l.Ident, "a");

            var r = As<AST.BinaryExpression>(b.Right);
            Assert.AreEqual(r.Op, "&&");

            var l2 = As<AST.VariableExpression>(r.Left);
            Assert.AreEqual(l2.Ident, "b");
            var r2 = As<AST.VariableExpression>(r.Right);
            Assert.AreEqual(r2.Ident, "c");
        }
        [Test]
        public void TestCalc()
        {
            string code = @"-1 / 2 + 3 * 4";
            var expr = GetExpr(code);

            var b = As<AST.BinaryExpression>(expr);
            Assert.AreEqual(b.Op, "+");
            {
                var lb = As<AST.BinaryExpression>(b.Left);
                Assert.AreEqual(lb.Op, "/");
                {
                    var lbl = As<AST.PrefixUnaryExpression>(lb.Left);
                    Assert.AreEqual(lbl.Op, "-");
                    var lblr = As<AST.LiteralExpression>(lbl.Right);
                    Assert.AreEqual(lblr.Value, "1");
                }
                {
                    var lbr = As<AST.LiteralExpression>(lb.Right);
                    Assert.AreEqual(lbr.Value, "2");
                }
            }
            {
                var rb = As<AST.BinaryExpression>(b.Right);
                Assert.AreEqual(rb.Op, "*");
                {
                    var rbl = As<AST.LiteralExpression>(rb.Left);
                    Assert.AreEqual(rbl.Value, "3");
                }
                {
                    var rbr = As<AST.LiteralExpression>(rb.Right);
                    Assert.AreEqual(rbr.Value, "4");
                }
            }
        }
        [Test]
        public void TestParen()
        {
            string code = @"(a || b) && c";
            var expr = GetExpr(code);

            var b = As<AST.BinaryExpression>(expr);
            Assert.AreEqual(b.Op, "&&");
            var l = As<AST.BinaryExpression>(b.Left);
            Assert.AreEqual(l.Op, "||");
            var l2 = As<AST.VariableExpression>(l.Left);
            Assert.AreEqual(l2.Ident, "a");
            var r2 = As<AST.VariableExpression>(l.Right);
            Assert.AreEqual(r2.Ident, "b");

            var r = As<AST.VariableExpression>(b.Right);
            Assert.AreEqual(r.Ident, "c");
        }
        [Test]
        public void TestList()
        {
            {
                string code = @"[]";
                var expr = GetExpr(code);
                var list = As<AST.ListExpression>(expr);
                Assert.AreEqual(list.Elements.Count, 0);
            }
            {
                string code = @"[1]";
                var expr = GetExpr(code);
                var list = As<AST.ListExpression>(expr);
                Assert.AreEqual(list.Elements.Count, 1);
                var e1 = As<AST.LiteralExpression>(list.Elements[0]);
                Assert.AreEqual(e1.Value, "1");
            }
            {
                string code = @"[1, 2]";
                var expr = GetExpr(code);
                var list = As<AST.ListExpression>(expr);
                Assert.AreEqual(list.Elements.Count, 2);
                var e1 = As<AST.LiteralExpression>(list.Elements[0]);
                Assert.AreEqual(e1.Value, "1");
                var e2 = As<AST.LiteralExpression>(list.Elements[1]);
                Assert.AreEqual(e2.Value, "2");
            }
            {
                string code = @"[1, 2,]";
                var expr = GetExpr(code);
                var list = As<AST.ListExpression>(expr);
                Assert.AreEqual(list.Elements.Count, 2);
                var e1 = As<AST.LiteralExpression>(list.Elements[0]);
                Assert.AreEqual(e1.Value, "1");
                var e2 = As<AST.LiteralExpression>(list.Elements[1]);
                Assert.AreEqual(e2.Value, "2");
            }
        }
        [Test]
        public void TestSubscript()
        {
            string code = @"a[0]";
            var expr = GetExpr(code);

            var s = As<AST.SubscriptExpression>(expr);
            var c = As<AST.VariableExpression>(s.Collection);
            Assert.AreEqual(c.Ident, "a");
            var i = As<AST.LiteralExpression>(s.Index);
            Assert.AreEqual(i.Value, "0");
        }
        [Test]
        public void TestParenError()
        {
            string code = @"(a || b && c";
            var exception = Assert.Catch<SyntaxErrorException>(() =>
            {
                Parse(code);
            });
            Assert.AreEqual(exception.Line, 1);
            Assert.AreEqual(exception.Column, 12);
        }
        [Test]
        public void TestMultiStatement()
        {
            string code = @"a=10;a+=2;";
            var root = Parse(code);
            Assert.AreEqual(root.Statements.Count, 2);
        }
        [Test]
        public void TestIf()
        {
            string code = @"
if (a > 0)
{
   ""plus"";
}
else if (a == 0)
{
   ""zero"";
}
else
{
   ""minus"";
}
";
            var root = Parse(code);
            var ifState = As<IfStatement>(root?.Statements?[0]);
            Assert.True(ifState != null);
            Assert.True(ifState.IfStatements.Count == 2);
            Assert.True(ifState.IfStatements[0].Statement is BlockStatement);
            Assert.True(ifState.IfStatements[1].Statement is BlockStatement);
            Assert.True(ifState.ElseStatement is BlockStatement);
        }
        static U As<U>(object v)
            where U : class
        {
            Assert.True(v is U);
            return v as U;
        }
        private GlobalStatement Parse(string code)
        {
            Lexer.Lexer lexer = new();
            var tokens = lexer.Tokenize(code);
            Parser parser = new();
            return parser.Parse(tokens).GlobalStatement;
        }
        private IExpression GetExpr(string code)
        {
            var root = Parse(code);
            var e = As<ExpressionStatement>(root?.Statements?[0]);
            return e?.Expression;
        }
    }
}
