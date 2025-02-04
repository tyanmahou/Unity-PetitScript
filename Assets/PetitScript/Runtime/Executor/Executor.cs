using Petit.AST;

namespace Petit.Executor
{
    class Executor
    {
        public Executor(Enviroment env)
        {
            _env = env;
        }
        public Value Exec(Root root)
        {
            return ExecGlobalStatement(root?.Statement);
        }

        Value ExecGlobalStatement(GlobalStatement statement)
        {
            return ExecExpr(statement?.Expression);
        }
        Value ExecExpr(IExpression expr)
        {
            if (expr is null)
            {
                return Value.Invalid;
            }
            else if (expr is LiteralExpression literal)
            {
                return ExecExpr(literal);
            }
            else if (expr is VariableExpression variable)
            {
                return ExecExpr(variable);
            }
            else if (expr is PrefixUnaryExpression unary)
            {
                return ExecExpr(unary);
            }
            else if (expr is BinaryExpression binary)
            {
                return ExecExpr(binary);
            }
            else if (expr is TernaryExpression ternary)
            {
                return ExecExpr(ternary);
            }
            return Value.Invalid;
        }

        Value ExecExpr(LiteralExpression expr)
        {
            return Value.Parse(expr.Value);
        }
        Value ExecExpr(VariableExpression expr)
        {
            return _env.Variables.Get(expr.Ident);
        }
        Value ExecExpr(PrefixUnaryExpression expr)
        {
            if (expr.Op == "!")
            {
                return !ExecExpr(expr.Right);
            }
            else if (expr.Op == "+")
            {
                return +ExecExpr(expr.Right);
            }
            else if (expr.Op == "-")
            {
                return -ExecExpr(expr.Right);
            }
            return ExecExpr(expr.Right);
        }
        Value ExecExpr(BinaryExpression expr)
        {
            if (expr.Op == "&&")
            {
                return ExecExpr(expr.Left) && ExecExpr(expr.Right);
            }
            else if (expr.Op == "||")
            {
                return ExecExpr(expr.Left) || ExecExpr(expr.Right);
            }
            else if (expr.Op == "==")
            {
                return new Value(ExecExpr(expr.Left) == ExecExpr(expr.Right));
            }
            else if (expr.Op == "===")
            {
                return new Value(Value.Identical(ExecExpr(expr.Left), ExecExpr(expr.Right)));
            }
            else if (expr.Op == "!=")
            {
                return new Value(ExecExpr(expr.Left) != ExecExpr(expr.Right));
            }
            else if (expr.Op == "!==")
            {
                return new Value(!Value.NotIdentical(ExecExpr(expr.Left), ExecExpr(expr.Right)));
            }
            else if (expr.Op == ">")
            {
                return new Value(ExecExpr(expr.Left) > ExecExpr(expr.Right));
            }
            else if (expr.Op == "<")
            {
                return new Value(ExecExpr(expr.Left) < ExecExpr(expr.Right));
            }
            else if (expr.Op == ">=")
            {
                return new Value(ExecExpr(expr.Left) >= ExecExpr(expr.Right));
            }
            else if (expr.Op == "<=")
            {
                return new Value(ExecExpr(expr.Left) <= ExecExpr(expr.Right));
            }
            else if (expr.Op == "<=>")
            {
                return new Value(Value.Compare(ExecExpr(expr.Left), ExecExpr(expr.Right)));
            }
            else if (expr.Op == "+")
            {
                return ExecExpr(expr.Left) + ExecExpr(expr.Right);
            }
            else if (expr.Op == "-")
            {
                return ExecExpr(expr.Left) - ExecExpr(expr.Right);
            }
            else if (expr.Op == "*")
            {
                return ExecExpr(expr.Left) * ExecExpr(expr.Right);
            }
            else if (expr.Op == "/")
            {
                return ExecExpr(expr.Left) / ExecExpr(expr.Right);
            }
            else if (expr.Op == "%")
            {
                return ExecExpr(expr.Left) % ExecExpr(expr.Right);
            }
            return default;
        }
        Value ExecExpr(TernaryExpression expr)
        {
            if (expr.Op == "?" && expr.Op2 == ":")
            {
                return ExecExpr(expr.Left) ? ExecExpr(expr.Mid) : ExecExpr(expr.Right);
            }
            return default;
        }

        Enviroment _env;
    }
}
