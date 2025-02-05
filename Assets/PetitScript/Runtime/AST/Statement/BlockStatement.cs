using System.Collections.Generic;

namespace Petit.AST
{
    class BlockStatement : IStatement
    {
        public List<IStatement> Statements = new List<IStatement>();
    }
}
