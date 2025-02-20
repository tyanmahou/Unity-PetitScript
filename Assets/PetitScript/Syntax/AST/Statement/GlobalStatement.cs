using System.Collections.Generic;

namespace Petit.Core.AST
{
    public class GlobalStatement : IStatement
    {
        public List<IStatement> Statements = new List<IStatement>();
    }
}
