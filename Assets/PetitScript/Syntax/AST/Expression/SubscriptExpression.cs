
namespace Petit.Syntax.AST
{
    public class SubscriptExpression : IExpression
    {
        public IExpression Collection;
        public IExpression Index;
    }
}
