using NUnit.Framework;

namespace Petit.Syntax.Parser
{
    class TestParser_List : TestParserBase
    {
        [Test]
        public void TestEmpty()
        {
            string code = @"[]";
            var expr = GetExpr(code);
            var list = As<AST.ListExpression>(expr);
            Assert.AreEqual(list.Elements.Count, 0);
        }
        [Test]
        public void TestUnit()
        {
            string code = @"[1]";
            var expr = GetExpr(code);
            var list = As<AST.ListExpression>(expr);
            Assert.AreEqual(list.Elements.Count, 1);
            var e1 = As<AST.IntLiteral>(list.Elements[0]);
            Assert.AreEqual(e1.Value, 1);
        }
        [Test]
        public void TestList()
        {
            string code = @"[1, 2]";
            var expr = GetExpr(code);
            var list = As<AST.ListExpression>(expr);
            Assert.AreEqual(list.Elements.Count, 2);
            var e1 = As<AST.IntLiteral>(list.Elements[0]);
            Assert.AreEqual(e1.Value, 1);
            var e2 = As<AST.IntLiteral>(list.Elements[1]);
            Assert.AreEqual(e2.Value, 2);
        }
        [Test]
        public void TestLastCamma()
        {
            string code = @"[1, 2,]";
            var expr = GetExpr(code);
            var list = As<AST.ListExpression>(expr);
            Assert.AreEqual(list.Elements.Count, 2);
            var e1 = As<AST.IntLiteral>(list.Elements[0]);
            Assert.AreEqual(e1.Value, 1);
            var e2 = As<AST.IntLiteral>(list.Elements[1]);
            Assert.AreEqual(e2.Value, 2);
        }
    }
}
