using System.Collections.Generic;

namespace Petit.Core.AST
{
    interface ISwitchLabel
    {
    }

    class SwitchCase : ISwitchLabel
    {
        public IExpression Expression;
    }
    class SwitchDefault : ISwitchLabel
    {
    }
    class SwitchSection
    {
        public List<ISwitchLabel> Labels = new();
        public List<IStatement> Statements = new();
    }
    class SwitchStatement : IStatement
    {
        public IExpression Condition;
        public List<SwitchSection> Sections = new List<SwitchSection>();
    }
}
