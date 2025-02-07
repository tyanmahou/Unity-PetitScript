
namespace Petit.Core.AST
{
    class ForStatement : IStatement
    {
        public IExpression Init;
        public IExpression Cond;
        public IExpression Loop;
        public IStatement Statement;
    }
}
