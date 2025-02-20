namespace Petit.Syntax.AST
{
    public class PrefixUnaryExpression : IExpression
    {
        public string Op;
        public IExpression Right;
    }
}
