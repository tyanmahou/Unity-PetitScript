using System.Collections.Generic;

namespace Petit.Syntax.AST
{
    public class StringExpression : IExpression
    {
        public List<IExpression> Expressions = new();
    }
}
