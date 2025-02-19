
namespace Petit.Core.AST
{
    class SubscriptExpression : IExpression
    {
        public IExpression Collection;
        public IExpression Index;
    }
}
