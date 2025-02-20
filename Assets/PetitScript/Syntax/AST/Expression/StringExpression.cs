using System.Collections.Generic;

namespace Petit.Core.AST
{
    public class StringExpression : IExpression
    {
        public List<IExpression> Expressions = new();
    }
}
