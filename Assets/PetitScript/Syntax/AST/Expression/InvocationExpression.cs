using System.Collections.Generic;

namespace Petit.Core.AST
{
    public struct Argument
    {
        public string Name;
        public IExpression Expression;
    }
    public class InvocationExpression : IExpression
    {
        public IExpression Function;
        public List<Argument> Args = new();
    }
}
