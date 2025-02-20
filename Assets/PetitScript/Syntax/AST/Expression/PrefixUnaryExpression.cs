namespace Petit.Core.AST
{
    public class PrefixUnaryExpression : IExpression
    {
        public string Op;
        public IExpression Right;
    }
}
