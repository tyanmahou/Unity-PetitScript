namespace Petit.Core.AST
{
    class PostfixUnaryExpression : IExpression
    {
        public IExpression Left;
        public string Op;
    }
}
