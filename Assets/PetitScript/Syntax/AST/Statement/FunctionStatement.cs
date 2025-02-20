using System.Collections.Generic;

namespace Petit.Core.AST
{
    public struct FunctionParamerter
    {
        public string Name;
        public IExpression DefaultValue;
    }
    public class FunctionStatement : IStatement
    {
        public string Ident;
        public List<FunctionParamerter> Paramerters = new List<FunctionParamerter>();
        public IStatement Statement;
    }
}
