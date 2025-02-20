
namespace Petit.Syntax.AST
{
    public class ForStatement : IStatement
    {
        public IExpression Init;
        public IExpression Cond;
        public IExpression Loop;
        public IStatement Statement;
    }
}
