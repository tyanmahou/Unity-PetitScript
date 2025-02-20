using NUnit.Framework;

namespace Petit.Syntax.Parser
{
    class TestParser_String : TestParserBase
    {
        [Test]
        public void TestStringLiteral()
        {
            string code = @"""abcde""";
            var expr = GetExpr(code);
            var str = As<AST.StringLiteral>(expr);
            Assert.AreEqual(str.Value, "abcde");
        }
        [Test]
        public void TestStringLiteralEmpty()
        {
            string code = @"""""";
            var expr = GetExpr(code);
            var str = As<AST.StringLiteral>(expr);
            Assert.AreEqual(str.Value, string.Empty);
        }
        [Test]
        public void TestStringInterpolation()
        {
            string code = @"""abc{1}def""";
            var expr = GetExpr(code);
            var list = As<AST.StringInterpolation>(expr);
            Assert.AreEqual(list.Expressions.Count, 3);

            var e0 = As<AST.StringLiteral>(list.Expressions[0]);
            Assert.AreEqual(e0.Value, "abc");
            var e1 = As<AST.IntLiteral>(list.Expressions[1]);
            Assert.AreEqual(e1.Value, 1);
            var e2 = As<AST.StringLiteral>(list.Expressions[2]);
            Assert.AreEqual(e2.Value, "def");
        }
        [Test]
        public void TestStringNoneInterpolation()
        {
            {
                string code = @"""abc{{1}}def""";
                var expr = GetExpr(code);
                var str = As<AST.StringLiteral>(expr);
                Assert.AreEqual(str.Value, "abc{1}def");
            }
            {
                string code = @"""abc{{1}def""";
                var expr = GetExpr(code);
                var str = As<AST.StringLiteral>(expr);
                Assert.AreEqual(str.Value, "abc{1}def");
            }
        }
        [Test]
        public void TestStringInterpolationNest()
        {
            string code = @"""abc{""x{""y""}z""}def""";
            var expr = GetExpr(code);
            var list = As<AST.StringInterpolation>(expr);
            Assert.AreEqual(list.Expressions.Count, 3);

            var e0 = As<AST.StringLiteral>(list.Expressions[0]);
            Assert.AreEqual(e0.Value, "abc");
            var e1 = As<AST.StringInterpolation>(list.Expressions[1]);
            {
                Assert.AreEqual(e1.Expressions.Count, 3);

                var e10 = As<AST.StringLiteral>(e1.Expressions[0]);
                Assert.AreEqual(e10.Value, "x");
                var e11 = As<AST.StringLiteral>(e1.Expressions[1]);
                Assert.AreEqual(e11.Value, "y");
                var e12 = As<AST.StringLiteral>(e1.Expressions[2]);
                Assert.AreEqual(e12.Value, "z");
            }
            var e2 = As<AST.StringLiteral>(list.Expressions[2]);
            Assert.AreEqual(e2.Value, "def");
        }
    }
}
