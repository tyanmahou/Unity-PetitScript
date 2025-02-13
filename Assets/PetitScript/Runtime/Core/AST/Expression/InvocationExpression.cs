using System.Collections.Generic;

namespace Petit.Core.AST
{
    struct Argument
    {
        public string Name;
        public IExpression Expression;
    }
    class InvocationExpression : IExpression
    {
        public IExpression Function;
        public List<Argument> Args = new();
    }
}
