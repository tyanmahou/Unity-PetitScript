using System.Collections.Generic;

namespace Petit.Syntax.AST
{
    public class ListExpression : IExpression
    {
        public List<IExpression> Elements = new();
    }
}
