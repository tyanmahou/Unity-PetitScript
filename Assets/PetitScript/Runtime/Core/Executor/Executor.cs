using Petit.Core.AST;
using System.Text;

namespace Petit.Core.Executor
{
    class Executor
    {
        enum StatementCommand
        {
            None,
            Reutrn,
            Break,
            Continue
        }
        public Executor(Enviroment env)
        {
            _env = env;
        }
        public Value Exec(GlobalStatement global)
        {
            return ExecGlobalStatement(global);
        }
        (Value, StatementCommand) ExecStatement(IStatement statement)
        {
            if (statement is BlockStatement block)
            {
                return ExecBlockStatement(block);
            }
            else if (statement is IfStatement ifStatement)
            {
                return ExecIfStatement(ifStatement);
            }
            else if (statement is WhileStatement whileStatement)
            {
                return ExecWhileStatement(whileStatement);
            }
            else if (statement is ForStatement forStatement)
            {
                return ExecForStatement(forStatement);
            }
            else if (statement is BreakStatement breakStatement)
            {
                return (Value.Invalid, StatementCommand.Break);
            }
            else if (statement is ContinueStatement continueStatement)
            {
                return (Value.Invalid, StatementCommand.Continue);
            }
            else if (statement is ReturnStatement returnStatement)
            {
                return (ExecExpr(returnStatement.Expression).Item1, StatementCommand.Reutrn);
            }
            else if (statement is ExpressionStatement expression)
            {
                return (ExecExpr(expression.Expression).Item1, StatementCommand.None);
            }
            return (Value.Invalid, StatementCommand.None);
        }
        Value ExecGlobalStatement(GlobalStatement global)
        {
            Value result = Value.Invalid;
            foreach (var s in global.Statements)
            {
                StatementCommand ret;
                (result, ret) = ExecStatement(s);
                if (ret == StatementCommand.Reutrn)
                {
                    break;
                }
            }
            return result;
        }
        (Value, StatementCommand) ExecBlockStatement(BlockStatement statement)
        {
            Value result = Value.Invalid;
            StatementCommand ret = StatementCommand.None;
            foreach (var s in statement.Statements)
            {
                (result, ret) = ExecStatement(s);
                if (ret == StatementCommand.Reutrn || ret == StatementCommand.Break || ret == StatementCommand.Continue)
                {
                    break;
                }
            }
            return (result, ret);
        }
        (Value, StatementCommand) ExecIfStatement(IfStatement ifStatement)
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
            return (Value.Invalid, StatementCommand.None);
        }
        (Value, StatementCommand) ExecWhileStatement(WhileStatement whileStatement)
        {
            (Value, StatementCommand) result = default;
            while (ExecExpr(whileStatement.Cond).Item1)
            {
                result = ExecStatement(whileStatement.Statement);
                if (result.Item2 == StatementCommand.Reutrn || result.Item2 == StatementCommand.Break)
                {
                    return result;
                }
                else if (result.Item2 == StatementCommand.Continue)
                {
                    continue;
                }
            }
            return result;
        }
        (Value, StatementCommand) ExecForStatement(ForStatement forStatement)
        {
            (Value, StatementCommand) result = default;
            for(ExecExpr(forStatement.Init);  ExecExpr(forStatement.Cond).Item1; ExecExpr(forStatement.Loop))
            {
                result = ExecStatement(forStatement.Statement);
                if (result.Item2 == StatementCommand.Reutrn || result.Item2 == StatementCommand.Break)
                {
                    return result;
                }
                else if (result.Item2 == StatementCommand.Continue)
                {
                    continue;
                }
            }
            return result;
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
            else if (expr is StringExpression str)
            {
                return ExecExpr(str);
            }
            else if (expr is VariableExpression variable)
            {
                return ExecExpr(variable);
            }
            else if (expr is PrefixUnaryExpression prefixUnary)
            {
                return ExecExpr(prefixUnary);
            }
            else if (expr is PostfixUnaryExpression postfixUnary)
            {
                return ExecExpr(postfixUnary);
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
        (Value, string) ExecExpr(StringExpression expr)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var e in expr.Expressions)
            {
                string value;
                if (e is LiteralExpression l)
                {
                    // そのまま流し込む
                    value = l.Value;
                }
                else
                {
                    value = ExecExpr(e).Item1.ToString();
                }
                sb.Append(value);
            }
            return (new Value(sb.ToString()), null);
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
            else if (expr.Op == "++")
            {
                var result = eval.Item1 + new Value(1);
                if (eval.Item2 != null)
                {
                    _env.Variables.Set(eval.Item2, result);
                }
                return (result, eval.Item2);
            }
            else if (expr.Op == "--")
            {
                var result = eval.Item1 - new Value(1);
                if (eval.Item2 != null)
                {
                    _env.Variables.Set(eval.Item2, result);
                }
                return (result, eval.Item2);
            }
            return eval;
        }
        (Value, string) ExecExpr(PostfixUnaryExpression expr)
        {
            var eval = ExecExpr(expr.Left);
            if (expr.Op == "++")
            {
                var result = eval.Item1 + new Value(1);
                if (eval.Item2 != null)
                {
                    _env.Variables.Set(eval.Item2, result);
                }
                return (eval.Item1, null);
            }
            else if (expr.Op == "--")
            {
                var result = eval.Item1 - new Value(1);
                if (eval.Item2 != null)
                {
                    _env.Variables.Set(eval.Item2, result);
                }
                return (eval.Item1, null);
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
