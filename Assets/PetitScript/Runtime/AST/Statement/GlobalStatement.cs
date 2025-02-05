using System.Collections.Generic;

namespace Petit.AST
{
    class GlobalStatement : IStatement
    {
        public List<IStatement> Statements = new List<IStatement>();
    }
}
