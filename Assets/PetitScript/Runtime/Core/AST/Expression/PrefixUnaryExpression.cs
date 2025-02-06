namespace Petit.Core.AST
{
    class PrefixUnaryExpression : IExpression
    {
        public string Op;
        public IExpression Right;
    }
}
