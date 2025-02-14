using Petit.Core.AST;
using System;
using System.Linq;
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
        public Executor()
        {
        }
        public Value Exec(GlobalStatement global, Enviroment env)
        {
            return ExecGlobalStatement(global, env);
        }
        (Value, StatementCommand) ExecStatement(IStatement statement, Enviroment env)
        {
            if (statement is BlockStatement block)
            {
                return ExecBlockStatement(block, env);
            }
            else if (statement is IfStatement ifStatement)
            {
                return ExecIfStatement(ifStatement, env);
            }
            else if (statement is SwitchStatement switchStatement)
            {
                return ExecSwitchStatement(switchStatement, env);
            }
            else if (statement is WhileStatement whileStatement)
            {
                return ExecWhileStatement(whileStatement, env);
            }
            else if (statement is ForStatement forStatement)
            {
                return ExecForStatement(forStatement, env);
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
                return (ExecExpr(returnStatement.Expression, env).Item1, StatementCommand.Reutrn);
            }
            else if (statement is ExpressionStatement expression)
            {
                return (ExecExpr(expression.Expression, env).Item1, StatementCommand.None);
            }
            else if (statement is FunctionStatement function)
            {
                return ExecFunctionStatement(function, env);
            }
            return (Value.Invalid, StatementCommand.None);
        }
        Value ExecGlobalStatement(GlobalStatement global, Enviroment env)
        {
            Value result = Value.Invalid;
            foreach (var s in global.Statements)
            {
                StatementCommand ret;
                (result, ret) = ExecStatement(s, env);
                if (ret == StatementCommand.Reutrn)
                {
                    break;
                }
            }
            return result;
        }
        (Value, StatementCommand) ExecBlockStatement(BlockStatement statement, Enviroment env)
        {
            Value result = Value.Invalid;
            StatementCommand ret = StatementCommand.None;
            foreach (var s in statement.Statements)
            {
                (result, ret) = ExecStatement(s, env);
                if (ret == StatementCommand.Reutrn || ret == StatementCommand.Break || ret == StatementCommand.Continue)
                {
                    break;
                }
            }
            return (result, ret);
        }
        (Value, StatementCommand) ExecIfStatement(IfStatement ifStatement, Enviroment env)
        {
            foreach (IfParam param in ifStatement.IfStatements)
            {
                if (ExecExpr(param.Condition, env).Item1)
                {
                    return ExecStatement(param.Statement, env);
                }
            }
            if (ifStatement.ElseStatement != null)
            {
                return ExecStatement(ifStatement.ElseStatement, env);
            }
            return (Value.Invalid, StatementCommand.None);
        }
        (Value, StatementCommand) ExecSwitchStatement(SwitchStatement switchStatement, Enviroment env)
        {
            var condition = ExecExpr(switchStatement.Condition, env).Item1;

            // どのセクションを使用するか確定する
            bool IsMatch(ISwitchLabel label, in Value v)
            {
                if (label is SwitchCase caseLabel)
                {
                    return ExecExpr(caseLabel.Expression, env).Item1 == v;
                }
                return false;
            }
            int selectSection = -1;
            int defaultSection = -1;
            for (int s = 0; s < switchStatement.Sections.Count; ++s)
            {
                var section = switchStatement.Sections[s];
                if (section.Labels.Any(l => IsMatch(l, condition)))
                {
                    selectSection = s;
                    break;
                }
                if (defaultSection < 0 && section.Labels.Any(l => l is SwitchDefault))
                {
                    defaultSection = s;
                }
            }
            if (selectSection < 0)
            {
                // デフォルト
                selectSection = defaultSection;
            }
            var result = (Value.Invalid, StatementCommand.None);
            if (selectSection >= 0)
            {
                // それ以降のセクションを実行
                for (int s = selectSection; s < switchStatement.Sections.Count; ++s)
                {
                    var section = switchStatement.Sections[s];
                    foreach (var statement in section.Statements)
                    {
                        result = ExecStatement(statement, env);
                        if (result.Item2 == StatementCommand.Reutrn)
                        {
                            return result;
                        }
                        else if (result.Item2 == StatementCommand.Break)
                        {
                            return (result.Item1, StatementCommand.None);
                        }
                    }
                }
            }
            return result;
        }
        (Value, StatementCommand) ExecWhileStatement(WhileStatement whileStatement, Enviroment env)
        {
            (Value, StatementCommand) result = default;
            while (ExecExpr(whileStatement.Cond, env).Item1)
            {
                result = ExecStatement(whileStatement.Statement, env);
                if (result.Item2 == StatementCommand.Reutrn)
                {
                    return result;
                }
                else if (result.Item2 == StatementCommand.Break)
                {
                    return (result.Item1, StatementCommand.None);
                }
                else if (result.Item2 == StatementCommand.Continue)
                {
                    continue;
                }
            }
            return result;
        }
        (Value, StatementCommand) ExecForStatement(ForStatement forStatement, Enviroment env)
        {
            (Value, StatementCommand) result = default;
            for(ExecExpr(forStatement.Init, env);  ExecExpr(forStatement.Cond, env).Item1; ExecExpr(forStatement.Loop, env))
            {
                result = ExecStatement(forStatement.Statement, env);
                if (result.Item2 == StatementCommand.Reutrn)
                {
                    return result;
                }
                else if (result.Item2 == StatementCommand.Break)
                {
                    return (result.Item1, StatementCommand.None);
                }
                else if (result.Item2 == StatementCommand.Continue)
                {
                    continue;
                }
            }
            return result;
        }
        (Value, StatementCommand) ExecFunctionStatement(FunctionStatement function, Enviroment env)
        {
            Function func = new(args =>
            {
                var parameters = function.Paramerters;
                var newEnv = env.Stack();
                for (int i = 0; i < parameters.Count; ++i)
                {
                    Value arg = args[i];
                    if (arg.IsInvalid)
                    {
                        arg = ExecExpr(parameters[i].DefaultValue, newEnv).Item1;
                    }
                    newEnv.Set(parameters[i].Name, arg);
                }
                return ExecStatement(function.Statement, newEnv).Item1;
            }, function.Paramerters.Select(p => new Argument(p.Name, (Func<Value>)null)).ToArray());
            env.SetFunc(function.Ident, func);
            return (Value.Invalid, StatementCommand.None);
        }
        (Value, string) ExecExpr(IExpression expr, Enviroment env)
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
                return ExecExpr(str, env);
            }
            else if (expr is VariableExpression variable)
            {
                return ExecExpr(variable, env);
            }
            else if (expr is PrefixUnaryExpression prefixUnary)
            {
                return ExecExpr(prefixUnary, env);
            }
            else if (expr is PostfixUnaryExpression postfixUnary)
            {
                return ExecExpr(postfixUnary, env);
            }
            else if (expr is BinaryExpression binary)
            {
                return ExecExpr(binary, env);
            }
            else if (expr is TernaryExpression ternary)
            {
                return ExecExpr(ternary, env);
            }
            else if (expr is InvocationExpression func)
            {
                return ExecExpr(func, env);
            }
            return (Value.Invalid, null);
        }

        (Value, string) ExecExpr(LiteralExpression expr)
        {
            return (Value.Parse(expr.Value), null);
        }
        (Value, string) ExecExpr(StringExpression expr, Enviroment env)
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
                    value = ExecExpr(e, env).Item1.ToString();
                }
                sb.Append(value);
            }
            return (new Value(sb.ToString()), null);
        }
        (Value, string) ExecExpr(VariableExpression expr, Enviroment env)
        {
            return (env.Get(expr.Ident), expr.Ident);
        }
        (Value, string) ExecExpr(PrefixUnaryExpression expr, Enviroment env)
        {
            if (expr.Op == "!")
            {
                return (!ExecExpr(expr.Right, env).Item1, null);
            }
            else if (expr.Op == "+")
            {
                return (+ExecExpr(expr.Right, env).Item1, null);
            }
            else if (expr.Op == "-")
            {
                return (-ExecExpr(expr.Right, env).Item1, null);
            }
            else if (expr.Op == "++")
            {
                var eval = ExecExpr(expr.Right, env);
                var result = eval.Item1 + new Value(1);
                if (eval.Item2 != null)
                {
                    env.Set(eval.Item2, result);
                }
                return (result, eval.Item2);
            }
            else if (expr.Op == "--")
            {
                var eval = ExecExpr(expr.Right, env);
                var result = eval.Item1 - new Value(1);
                if (eval.Item2 != null)
                {
                    env.Set(eval.Item2, result);
                }
                return (result, eval.Item2);
            }
            else if (expr.Op == "~")
            {
                return (Value.BitwiseNot(ExecExpr(expr.Right, env).Item1), null);
            }
            return ExecExpr(expr.Right, env);
        }
        (Value, string) ExecExpr(PostfixUnaryExpression expr, Enviroment env)
        {
            if (expr.Op == "++")
            {
                var eval = ExecExpr(expr.Left, env);
                var result = eval.Item1 + new Value(1);
                if (eval.Item2 != null)
                {
                    env.Set(eval.Item2, result);
                }
                return (eval.Item1, null);
            }
            else if (expr.Op == "--")
            {
                var eval = ExecExpr(expr.Left, env);
                var result = eval.Item1 - new Value(1);
                if (eval.Item2 != null)
                {
                    env.Set(eval.Item2, result);
                }
                return (eval.Item1, null);
            }
            return ExecExpr(expr.Left, env);
        }
        (Value, string) ExecExpr(BinaryExpression expr, Enviroment env)
        {
            // 短絡評価
            if (expr.Op == "&&")
            {
                return (ExecExpr(expr.Left, env).Item1 && ExecExpr(expr.Right, env).Item1, null);
            }
            else if (expr.Op == "||")
            {
                return (ExecExpr(expr.Left, env).Item1 || ExecExpr(expr.Right, env).Item1, null);
            }
            else if (expr.Op == "==")
            {
                return (new Value(ExecExpr(expr.Left, env).Item1 == ExecExpr(expr.Right, env).Item1), null);
            }
            else if (expr.Op == "===")
            {
                return (new Value(Value.Identical(ExecExpr(expr.Left, env).Item1, ExecExpr(expr.Right, env).Item1)), null);
            }
            else if (expr.Op == "!=")
            {
                return (new Value(ExecExpr(expr.Left, env).Item1 != ExecExpr(expr.Right, env).Item1), null);
            }
            else if (expr.Op == "!==")
            {
                return (new Value(Value.NotIdentical(ExecExpr(expr.Left, env).Item1, ExecExpr(expr.Right, env).Item1)), null);
            }
            else if (expr.Op == ">")
            {
                return (new Value(ExecExpr(expr.Left, env).Item1 > ExecExpr(expr.Right, env).Item1), null);
            }
            else if (expr.Op == "<")
            {
                return (new Value(ExecExpr(expr.Left, env).Item1 < ExecExpr(expr.Right, env).Item1), null);
            }
            else if (expr.Op == ">=")
            {
                return (new Value(ExecExpr(expr.Left, env).Item1 >= ExecExpr(expr.Right, env).Item1), null);
            }
            else if (expr.Op == "<=")
            {
                return (new Value(ExecExpr(expr.Left, env).Item1 <= ExecExpr(expr.Right, env).Item1), null);
            }
            else if (expr.Op == "<=>")
            {
                return (new Value(Value.Compare(ExecExpr(expr.Left, env).Item1, ExecExpr(expr.Right, env).Item1)), null);
            }
            else if (expr.Op == "&")
            {
                return (Value.BitwiseAnd(ExecExpr(expr.Left, env).Item1, ExecExpr(expr.Right, env).Item1), null);
            }
            else if (expr.Op == "|")
            {
                return (Value.BitwiseOr(ExecExpr(expr.Left, env).Item1, ExecExpr(expr.Right, env).Item1), null);
            }
            else if (expr.Op == "^")
            {
                return (Value.BitwiseXor(ExecExpr(expr.Left, env).Item1, ExecExpr(expr.Right, env).Item1), null);
            }
            else if (expr.Op == "<<")
            {
                return (ExecExpr(expr.Left, env).Item1 << ExecExpr(expr.Right, env).Item1.ToInt(), null);
            }
            else if (expr.Op == ">>")
            {
                return (ExecExpr(expr.Left, env).Item1 >> ExecExpr(expr.Right, env).Item1.ToInt(), null);
            }
            else if (expr.Op == "+")
            {
                return (ExecExpr(expr.Left, env).Item1 + ExecExpr(expr.Right, env).Item1, null);
            }
            else if (expr.Op == "-")
            {
                return (ExecExpr(expr.Left, env).Item1 - ExecExpr(expr.Right, env).Item1, null);
            }
            else if (expr.Op == "*")
            {
                return (ExecExpr(expr.Left, env).Item1 * ExecExpr(expr.Right, env).Item1, null);
            }
            else if (expr.Op == "/")
            {
                return (ExecExpr(expr.Left, env).Item1 / ExecExpr(expr.Right, env).Item1, null);
            }
            else if (expr.Op == "%")
            {
                return (ExecExpr(expr.Left, env).Item1 % ExecExpr(expr.Right, env).Item1, null);
            }
            else if (expr.Op == "=")
            {
                var left = ExecExpr(expr.Left, env);
                var right = ExecExpr(expr.Right, env);
                if (left.Item2 != null)
                {
                    env.Set(left.Item2, right.Item1);
                }
                return (right.Item1, left.Item2);
            }
            else if (expr.Op == "+=")
            {
                var left = ExecExpr(expr.Left, env);
                var right = ExecExpr(expr.Right, env);

                var eval = left.Item1 + right.Item1;
                if (left.Item2 != null)
                {
                    env.Set(left.Item2, eval);
                }
                return (eval, left.Item2);
            }
            else if (expr.Op == "-=")
            {
                var left = ExecExpr(expr.Left, env);
                var right = ExecExpr(expr.Right, env);

                var eval = left.Item1 - right.Item1;
                if (left.Item2 != null)
                {
                    env.Set(left.Item2, eval);
                }
                return (eval, left.Item2);
            }
            else if (expr.Op == "*=")
            {
                var left = ExecExpr(expr.Left, env);
                var right = ExecExpr(expr.Right, env);

                var eval = left.Item1 * right.Item1;
                if (left.Item2 != null)
                {
                    env.Set(left.Item2, eval);
                }
                return (eval, left.Item2);
            }
            else if (expr.Op == "/=")
            {
                var left = ExecExpr(expr.Left, env);
                var right = ExecExpr(expr.Right, env);

                var eval = left.Item1 / right.Item1;
                if (left.Item2 != null)
                {
                    env.Set(left.Item2, eval);
                }
                return (eval, left.Item2);
            }
            else if (expr.Op == "%=")
            {
                var left = ExecExpr(expr.Left, env);
                var right = ExecExpr(expr.Right, env);

                var eval = left.Item1 % right.Item1;
                if (left.Item2 != null)
                {
                    env.Set(left.Item2, eval);
                }
                return (eval, left.Item2);
            }
            else if (expr.Op == "&=")
            {
                var left = ExecExpr(expr.Left, env);
                var right = ExecExpr(expr.Right, env);

                var eval = Value.BitwiseAnd(left.Item1, right.Item1);
                if (left.Item2 != null)
                {
                    env.Set(left.Item2, eval);
                }
                return (eval, left.Item2);
            }
            else if (expr.Op == "|=")
            {
                var left = ExecExpr(expr.Left, env);
                var right = ExecExpr(expr.Right, env);

                var eval = Value.BitwiseOr(left.Item1, right.Item1);
                if (left.Item2 != null)
                {
                    env.Set(left.Item2, eval);
                }
                return (eval, left.Item2);
            }
            else if (expr.Op == "^=")
            {
                var left = ExecExpr(expr.Left, env);
                var right = ExecExpr(expr.Right, env);

                var eval = Value.BitwiseXor(left.Item1, right.Item1);
                if (left.Item2 != null)
                {
                    env.Set(left.Item2, eval);
                }
                return (eval, left.Item2);
            }
            else if (expr.Op == "<<=")
            {
                var left = ExecExpr(expr.Left, env);
                var right = ExecExpr(expr.Right, env);

                var eval = left.Item1 << right.Item1.ToInt();
                if (left.Item2 != null)
                {
                    env.Set(left.Item2, eval);
                }
                return (eval, left.Item2);
            }
            else if (expr.Op == ">>=")
            {
                var left = ExecExpr(expr.Left, env);
                var right = ExecExpr(expr.Right, env);

                var eval = left.Item1 >> right.Item1.ToInt();
                if (left.Item2 != null)
                {
                    env.Set(left.Item2, eval);
                }
                return (eval, left.Item2);
            }
            return ExecExpr(expr.Left, env);
        }
        (Value, string) ExecExpr(TernaryExpression expr, Enviroment env)
        {
            if (expr.Op == "?" && expr.Op2 == ":")
            {
                var cond = ExecExpr(expr.Left, env).Item1.ToBool();

                (Value, string) mid = default;
                (Value, string) right = default;
                return (
                    (cond ? (mid = ExecExpr(expr.Mid, env)).Item1 : (right = ExecExpr(expr.Right, env)).Item1),
                    (cond ? mid.Item2 : right.Item2)
                    );
            }
            return default;
        }
        (Value, string) ExecExpr(InvocationExpression expr, Enviroment env)
        {
            string ident = string.Empty;
            if (expr.Function is VariableExpression v)
            {
                ident = v.Ident;
            }
            else
            {
                throw new Exception.RuntimeErrorException("Not impliment function object");
            }
            Function func = env.GetFunc(ident);
            var args = expr.Args
                .Select(arg => new Argument(arg.Name, ExecExpr(arg.Expression, env).Item1))
                .ToList();
            return (func.Invoke(args), null);
        }
    }
}
