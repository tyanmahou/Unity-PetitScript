namespace Petit.Core.AST
{
    public class BinaryExpression : IExpression
    {
        public IExpression Left;
        public string Op;
        public IExpression Right;
    }
}
