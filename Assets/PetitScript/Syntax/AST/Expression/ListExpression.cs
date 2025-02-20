using System.Collections.Generic;

namespace Petit.Core.AST
{
    public class ListExpression : IExpression
    {
        public List<IExpression> Elements = new();
    }
}
