using System.Collections.Generic;

namespace Petit.Syntax.AST
{
    public interface ISwitchLabel
    {
    }

    public class SwitchCase : ISwitchLabel
    {
        public IExpression Expression;
    }
    public class SwitchDefault : ISwitchLabel
    {
    }
    public class SwitchSection
    {
        public List<ISwitchLabel> Labels = new();
        public List<IStatement> Statements = new();
    }
    public class SwitchStatement : IStatement
    {
        public IExpression Condition;
        public List<SwitchSection> Sections = new List<SwitchSection>();
    }
}
