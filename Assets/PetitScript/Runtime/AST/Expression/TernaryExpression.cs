namespace Petit.AST
{
    class TernaryExpression : IExpression
    {
        public IExpression Left;
        public string Op;
        public IExpression Mid;
        public string Op2;
        public IExpression Right;
    }
}
