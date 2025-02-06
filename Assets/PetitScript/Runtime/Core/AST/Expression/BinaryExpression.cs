namespace Petit.Core.AST
{
    class BinaryExpression : IExpression
    {
        public IExpression Left;
        public string Op;
        public IExpression Right;
    }
}
