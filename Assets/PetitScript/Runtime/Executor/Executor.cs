using Petit.AST;

namespace Petit.Executor
{
    class Executor
    {
        public Executor(Enviroment env)
        {
            _env = env;
        }
        public Value Exec(GlobalStatement global)
        {
            return ExecGlobalStatement(global);
        }
        (Value, bool) ExecStatement(IStatement statement)
        {
            if (statement is BlockStatement block)
            {
                return ExecBlockStatement(block);
            }
            else if (statement is IfStatement ifStatement)
            {
                return ExecIfStatement(ifStatement);
            }
            else if (statement is ReturnStatement returnStatement)
            {
                return (ExecExpr(returnStatement.Expression).Item1, true);
            }
            else if (statement is ExpressionStatement expression)
            {
                return (ExecExpr(expression.Expression).Item1, false);
            }
            return (Value.Invalid, false);
        }
        Value ExecGlobalStatement(GlobalStatement global)
        {
            Value result = Value.Invalid;
            foreach (var s in global.Statements)
            {
                bool ret;
                (result, ret) = ExecStatement(s);
                if (ret)
                {
                    break;
                }
            }
            return result;
        }
        (Value, bool) ExecBlockStatement(BlockStatement statement)
        {
            Value result = Value.Invalid;
            bool ret = false;
            foreach (var s in statement.Statements)
            {
                (result, ret) = ExecStatement(s);
                if (ret)
                {
                    break;
                }
            }
            return (result, ret);
        }
        (Value, bool) ExecIfStatement(IfStatement ifStatement)
        {
            foreach (IfParam param in ifStatement.IfStatements)
            {
                if (ExecExpr(param.Cond).Item1)
                {
                    return ExecStatement(param.Statement);
                }
            }
            if (ifStatement.ElseStatement != null)
            {
                return ExecStatement(ifStatement.ElseStatement);
            }
            return (Value.Invalid, false);
        }
        (Value, string) ExecExpr(IExpression expr)
        {
            if (expr is null)
            {
                return (Value.Invalid, null);
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
            return (Value.Invalid, null);
        }

        (Value, string) ExecExpr(LiteralExpression expr)
        {
            return (Value.Parse(expr.Value), null);
        }
        (Value, string) ExecExpr(VariableExpression expr)
        {
            return (_env.Variables.Get(expr.Ident), expr.Ident);
        }
        (Value, string) ExecExpr(PrefixUnaryExpression expr)
        {
            var eval = ExecExpr(expr.Right);
            if (expr.Op == "!")
            {
                return (!eval.Item1, null);
            }
            else if (expr.Op == "+")
            {
                return (+eval.Item1, null);
            }
            else if (expr.Op == "-")
            {
                return (-eval.Item1, null);
            }
            return eval;
        }
        (Value, string) ExecExpr(BinaryExpression expr)
        {
            var left = ExecExpr(expr.Left);
            var right = ExecExpr(expr.Right);
            if (expr.Op == "&&")
            {
                return (left.Item1 && right.Item1, null);
            }
            else if (expr.Op == "||")
            {
                return (left.Item1 || right.Item1, null);
            }
            else if (expr.Op == "==")
            {
                return (new Value(left.Item1 ==  right.Item1), null);
            }
            else if (expr.Op == "===")
            {
                return (new Value(Value.Identical(left.Item1, right.Item1)), null);
            }
            else if (expr.Op == "!=")
            {
                return (new Value(left.Item1 != right.Item1), null);
            }
            else if (expr.Op == "!==")
            {
                return (new Value(Value.NotIdentical(left.Item1, right.Item1)), null);
            }
            else if (expr.Op == ">")
            {
                return (new Value(left.Item1 > right.Item1), null);
            }
            else if (expr.Op == "<")
            {
                return (new Value(left.Item1 < right.Item1), null);
            }
            else if (expr.Op == ">=")
            {
                return (new Value(left.Item1 >= right.Item1), null);
            }
            else if (expr.Op == "<=")
            {
                return (new Value(left.Item1 <= right.Item1), null);
            }
            else if (expr.Op == "<=>")
            {
                return (new Value(Value.Compare(left.Item1, right.Item1)), null);
            }
            else if (expr.Op == "+")
            {
                return (left.Item1 + right.Item1, null);
            }
            else if (expr.Op == "-")
            {
                return (left.Item1 - right.Item1, null);
            }
            else if (expr.Op == "*")
            {
                return (left.Item1 * right.Item1, null);
            }
            else if (expr.Op == "/")
            {
                return (left.Item1 / right.Item1, null);
            }
            else if (expr.Op == "%")
            {
                return (left.Item1 % right.Item1, null);
            }
            else if (expr.Op == "=")
            {
                if (left.Item2 != null)
                {
                    _env.Variables.Set(left.Item2, right.Item1);
                }
                return (right.Item1, left.Item2);
            }
            else if (expr.Op == "+=")
            {
                var eval = left.Item1 + right.Item1;
                if (left.Item2 != null)
                {
                    _env.Variables.Set(left.Item2, eval);
                }
                return (eval, left.Item2);
            }
            else if (expr.Op == "-=")
            {
                var eval = left.Item1 - right.Item1;
                if (left.Item2 != null)
                {
                    _env.Variables.Set(left.Item2, eval);
                }
                return (eval, left.Item2);
            }
            else if (expr.Op == "*=")
            {
                var eval = left.Item1 * right.Item1;
                if (left.Item2 != null)
                {
                    _env.Variables.Set(left.Item2, eval);
                }
                return (eval, left.Item2);
            }
            else if (expr.Op == "/=")
            {
                var eval = left.Item1 / right.Item1;
                if (left.Item2 != null)
                {
                    _env.Variables.Set(left.Item2, eval);
                }
                return (eval, left.Item2);
            }
            else if (expr.Op == "%=")
            {
                var eval = left.Item1 % right.Item1;
                if (left.Item2 != null)
                {
                    _env.Variables.Set(left.Item2, eval);
                }
                return (eval, left.Item2);
            }
            return default;
        }
        (Value, string) ExecExpr(TernaryExpression expr)
        {
            var left = ExecExpr(expr.Left);
            var mid = ExecExpr(expr.Mid);
            var right = ExecExpr(expr.Right);
            if (expr.Op == "?" && expr.Op2 == ":")
            {
                return ((left.Item1 ? mid.Item1 : right.Item1), (left.Item1 ? mid.Item2 : right.Item2));
            }
            return default;
        }

        Enviroment _env;
    }
}
