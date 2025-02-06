using System.Collections.Generic;

namespace Petit.Core.AST
{
    class StringExpression : IExpression
    {
        public List<IExpression> Expressions = new();
    }
}
