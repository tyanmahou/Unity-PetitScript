
namespace Petit.Syntax.AST
{

    public class IfStatement : IStatement
    {
        public IExpression Condition;
        public IStatement Statement;
        public IStatement ElseStatement;
    }
}
