namespace Petit.Core.AST
{
    public class TernaryExpression : IExpression
    {
        public IExpression Left;
        public string Op;
        public IExpression Mid;
        public string Op2;
        public IExpression Right;
    }
}
