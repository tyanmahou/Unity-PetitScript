using System.Collections.Generic;

namespace Petit.Core.AST
{
    struct FunctionParamerter
    {
        public string Name;
        public IExpression DefaultValue;
    }
    class FunctionStatement : IStatement
    {
        public string Ident;
        public List<FunctionParamerter> Paramerters = new List<FunctionParamerter>();
        public IStatement Statement;
    }
}
