
namespace Petit.Core.AST
{
    class WhileStatement : IStatement
    {
        public IExpression Cond;
        public IStatement Statement;
    }
}
