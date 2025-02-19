using System.Collections.Generic;

namespace Petit.Core.AST
{
    class ListExpression : IExpression
    {
        public List<IExpression> Elements = new();
    }
}
