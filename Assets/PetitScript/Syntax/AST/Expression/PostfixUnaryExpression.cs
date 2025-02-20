namespace Petit.Syntax.AST
{
    public class PostfixUnaryExpression : IExpression
    {
        public IExpression Left;
        public string Op;
    }
}
