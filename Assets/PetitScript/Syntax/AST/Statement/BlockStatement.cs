using System.Collections.Generic;

namespace Petit.Syntax.AST
{
    public class BlockStatement : IStatement
    {
        public List<IStatement> Statements = new List<IStatement>();
    }
}
