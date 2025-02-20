
namespace Petit.Core.AST
{
    public class SubscriptExpression : IExpression
    {
        public IExpression Collection;
        public IExpression Index;
    }
}
