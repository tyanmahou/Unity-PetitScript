using System.Collections.Generic;

namespace Petit.Syntax.AST
{
    public class GlobalStatement : IStatement
    {
        public List<IStatement> Statements = new List<IStatement>();
    }
}
