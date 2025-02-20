using System.Collections.Generic;

namespace Petit.Syntax.AST
{
    public class StringInterpolation : IExpression
    {
        public List<IExpression> Expressions = new();
    }
}
