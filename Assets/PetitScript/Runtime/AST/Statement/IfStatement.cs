using System.Collections.Generic;

namespace Petit.AST
{
    struct IfParam
    {
        public IExpression Cond;
        public IStatement Statement;
    }

    class IfStatement : IStatement
    {
        public List<IfParam> IfStatements = new List<IfParam>();
        public IStatement ElseStatement;
    }
}
