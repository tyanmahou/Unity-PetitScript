using System.Collections.Generic;

namespace Petit.Core.AST
{
    struct IfParam
    {
        public IExpression Condition;
        public IStatement Statement;
    }

    class IfStatement : IStatement
    {
        public List<IfParam> IfStatements = new List<IfParam>();
        public IStatement ElseStatement;
    }
}
