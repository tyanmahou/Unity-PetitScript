using System.Collections.Generic;

namespace Petit.Core.AST
{
    class GlobalStatement : IStatement
    {
        public List<IStatement> Statements = new List<IStatement>();
    }
}
