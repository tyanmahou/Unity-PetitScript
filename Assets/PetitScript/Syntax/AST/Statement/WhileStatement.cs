
namespace Petit.Core.AST
{
    public class WhileStatement : IStatement
    {
        public IExpression Cond;
        public IStatement Statement;
    }
}
