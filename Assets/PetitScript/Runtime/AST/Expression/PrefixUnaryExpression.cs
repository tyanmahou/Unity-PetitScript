namespace Petit.AST
{
    class PrefixUnaryExpression : IExpression
    {
        public string Op;
        public IExpression Right;
    }
}
