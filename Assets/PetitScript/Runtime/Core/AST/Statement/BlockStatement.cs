using System.Collections.Generic;

namespace Petit.Core.AST
{
    class BlockStatement : IStatement
    {
        public List<IStatement> Statements = new List<IStatement>();
    }
}
