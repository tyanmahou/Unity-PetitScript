using System.Collections.Generic;

namespace Petit.Syntax.AST
{
    public struct FunctionParamerter
    {
        public string Name;
        public IExpression DefaultValue;
    }
    public class FunctionDeclaration : IDeclaration
    {
        public string Identifier;
        public List<FunctionParamerter> Paramerters = new List<FunctionParamerter>();
        public IStatement Statement;
    }
}
