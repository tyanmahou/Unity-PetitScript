using System.Collections.Generic;

namespace Petit.Core.AST
{
    public struct IfParam
    {
        public IExpression Condition;
        public IStatement Statement;
    }

    public class IfStatement : IStatement
    {
        public List<IfParam> IfStatements = new List<IfParam>();
        public IStatement ElseStatement;
    }
}
