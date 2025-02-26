using Petit.Syntax.AST;
using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Petit.Runtime.Executor
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
        public Value Exec(GlobalStatement global, Environment env)
        {
            return ExecGlobalStatement(global, env).Copy();
        }
        (Value, StatementCommand) ExecStatement(IStatement statement, Environment env)
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
                return (ExecExpr(returnStatement.Expression, env), StatementCommand.Reutrn);
            }
            else if (statement is ExpressionStatement expression)
            {
                return (ExecExpr(expression.Expression, env), StatementCommand.None);
            }
            else if (statement is FunctionDeclaration function)
            {
                return ExecFunctionStatement(function, env);
            }
            return (Value.Invalid, StatementCommand.None);
        }
        Value ExecGlobalStatement(GlobalStatement global, Environment env)
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
        (Value, StatementCommand) ExecBlockStatement(BlockStatement statement, Environment env)
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
        (Value, StatementCommand) ExecIfStatement(IfStatement ifStatement, Environment env)
        {
            if (ExecExpr(ifStatement.Condition, env))
            {
                return ExecStatement(ifStatement.Statement, env);
            }
            if (ifStatement.ElseStatement != null)
            {
                return ExecStatement(ifStatement.ElseStatement, env);
            }
            return (Value.Invalid, StatementCommand.None);
        }
        (Value, StatementCommand) ExecSwitchStatement(SwitchStatement switchStatement, Environment env)
        {
            var condition = ExecExpr(switchStatement.Condition, env);

            // どのセクションを使用するか確定する
            bool IsMatch(ISwitchLabel label, in Value v)
            {
                if (label is SwitchCase caseLabel)
                {
                    return ExecExpr(caseLabel.Expression, env) == v;
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
        (Value, StatementCommand) ExecWhileStatement(WhileStatement whileStatement, Environment env)
        {
            (Value, StatementCommand) result = default;
            while (ExecExpr(whileStatement.Cond, env))
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
        (Value, StatementCommand) ExecForStatement(ForStatement forStatement, Environment env)
        {
            (Value, StatementCommand) result = default;
            for(ExecExpr(forStatement.Init, env);  ExecExpr(forStatement.Cond, env); ExecExpr(forStatement.Loop, env))
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
        (Value, StatementCommand) ExecFunctionStatement(FunctionDeclaration function, Environment env)
        {
            Function func = new(
                function.Identifier,
                args =>
                {
                    var parameters = function.Paramerters;
                    var newEnv = env.Stack();
                    for (int i = 0; i < parameters.Count; ++i)
                    {
                        Value arg = args[i];
                        if (arg.IsInvalid)
                        {
                            arg = ExecExpr(parameters[i].DefaultValue, newEnv);
                        }
                        newEnv.SetScope(parameters[i].Name, arg);
                    }
                    return ExecStatement(function.Statement, newEnv).Item1;
                },
                function.Paramerters.Select(p => new Argument(p.Name, (Func<Value>)null)).ToArray()
            );
            env.SetScope(function.Identifier, func);
            return (Value.Of(func), StatementCommand.None);
        }
        Value ExecExpr(IExpression expr, Environment env)
        {
            if (expr is null)
            {
                return default;
            }
            else if (expr is BoolLiteral boolLiteral)
            {
                return ExecExpr(boolLiteral);
            }
            else if (expr is IntLiteral intLiteral)
            {
                return ExecExpr(intLiteral);
            }
            else if (expr is FloatLiteral floatLiteral)
            {
                return ExecExpr(floatLiteral);
            }
            else if (expr is StringLiteral strLiteral)
            {
                return ExecExpr(strLiteral);
            }
            else if (expr is StringInterpolation str)
            {
                return ExecExpr(str, env);
            }
            else if (expr is ListExpression list)
            {
                return ExecExpr(list, env);
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
            else if (expr is SubscriptExpression subscript)
            {
                return ExecExpr(subscript, env);
            }
            return default;
        }
        Value ExecExpr(BoolLiteral expr)
        {
            return Value.Of(expr.Value);
        }
        Value ExecExpr(IntLiteral expr)
        {
            return Value.Of(expr.Value);
        }
        Value ExecExpr(FloatLiteral expr)
        {
            return Value.Of(expr.Value);
        }
        Value ExecExpr(StringLiteral expr)
        {
            return Value.Of(expr.Value);
        }
        Value ExecExpr(StringInterpolation expr, Environment env)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var e in expr.Expressions)
            {
                string value;
                if (e is StringLiteral l)
                {
                    // そのまま流し込む
                    value = l.Value;
                }
                else
                {
                    value = ExecExpr(e, env).ToString();
                }
                sb.Append(value);
            }
            return Value.Of(sb.ToString());
        }
        Value ExecExpr(ListExpression expr, Environment env)
        {
            List<Value> values = new List<Value>(expr.Elements.Count);
            foreach (var e in expr.Elements)
            {
                values.Add(ExecExpr(e, env));
            }
            return Value.Of(values);
        }
        Value ExecExpr(VariableExpression expr, Environment env)
        {
            return new Reference(
                get: () => env.Get(expr.Identifier),
                set: v => env.Set(expr.Identifier, v)
                );
        }
        Value ExecExpr(PrefixUnaryExpression expr, Environment env)
        {
            if (expr.Op == "!")
            {
                return !ExecExpr(expr.Right, env);
            }
            else if (expr.Op == "+")
            {
                return +ExecExpr(expr.Right, env);
            }
            else if (expr.Op == "-")
            {
                return -ExecExpr(expr.Right, env);
            }
            else if (expr.Op == "++")
            {
                var eval = ExecExpr(expr.Right, env);
                return eval.SetIndirect(eval + Value.Of(1));
            }
            else if (expr.Op == "--")
            {
                var eval = ExecExpr(expr.Right, env);
                return eval.SetIndirect(eval - Value.Of(1));
            }
            else if (expr.Op == "~")
            {
                return Value.BitwiseNot(ExecExpr(expr.Right, env));
            }
            return ExecExpr(expr.Right, env);
        }
        Value ExecExpr(PostfixUnaryExpression expr, Environment env)
        {
            if (expr.Op == "++")
            {
                var eval = ExecExpr(expr.Left, env);
                var copy = eval.Copy();
                eval.TrySetIndirect(eval + Value.Of(1));
                return copy;
            }
            else if (expr.Op == "--")
            {
                var eval = ExecExpr(expr.Left, env);
                var copy = eval.Copy();
                eval.TrySetIndirect(eval - Value.Of(1));
                return copy;
            }
            return ExecExpr(expr.Left, env);
        }
        Value ExecExpr(BinaryExpression expr, Environment env)
        {
            if (expr.Op == ".")
            {
                return ExecExpr(expr.Right, env).ToFunction().Partial(ExecExpr(expr.Left, env));
            }
            else if (expr.Op == "&&")
            {
                return ExecExpr(expr.Left, env) && ExecExpr(expr.Right, env);
            }
            else if (expr.Op == "||")
            {
                return ExecExpr(expr.Left, env) || ExecExpr(expr.Right, env);
            }
            else if (expr.Op == "==")
            {
                return ExecExpr(expr.Left, env) == ExecExpr(expr.Right, env);
            }
            else if (expr.Op == "===")
            {
                return Value.Identical(ExecExpr(expr.Left, env), ExecExpr(expr.Right, env));
            }
            else if (expr.Op == "!=")
            {
                return ExecExpr(expr.Left, env) != ExecExpr(expr.Right, env);
            }
            else if (expr.Op == "!==")
            {
                return Value.NotIdentical(ExecExpr(expr.Left, env), ExecExpr(expr.Right, env));
            }
            else if (expr.Op == ">")
            {
                return ExecExpr(expr.Left, env) > ExecExpr(expr.Right, env);
            }
            else if (expr.Op == "<")
            {
                return ExecExpr(expr.Left, env) < ExecExpr(expr.Right, env);
            }
            else if (expr.Op == ">=")
            {
                return ExecExpr(expr.Left, env) >= ExecExpr(expr.Right, env);
            }
            else if (expr.Op == "<=")
            {
                return ExecExpr(expr.Left, env) <= ExecExpr(expr.Right, env);
            }
            else if (expr.Op == "<=>")
            {
                return Value.Compare(ExecExpr(expr.Left, env), ExecExpr(expr.Right, env));
            }
            else if (expr.Op == "&")
            {
                return Value.BitwiseAnd(ExecExpr(expr.Left, env), ExecExpr(expr.Right, env));
            }
            else if (expr.Op == "|")
            {
                return Value.BitwiseOr(ExecExpr(expr.Left, env), ExecExpr(expr.Right, env));
            }
            else if (expr.Op == "^")
            {
                return Value.BitwiseXor(ExecExpr(expr.Left, env), ExecExpr(expr.Right, env));
            }
            else if (expr.Op == "<<")
            {
                return ExecExpr(expr.Left, env) << ExecExpr(expr.Right, env).ToInt();
            }
            else if (expr.Op == ">>")
            {
                return ExecExpr(expr.Left, env) >> ExecExpr(expr.Right, env).ToInt();
            }
            else if (expr.Op == "+")
            {
                return ExecExpr(expr.Left, env) + ExecExpr(expr.Right, env);
            }
            else if (expr.Op == "-")
            {
                return ExecExpr(expr.Left, env) - ExecExpr(expr.Right, env);
            }
            else if (expr.Op == "*")
            {
                return ExecExpr(expr.Left, env) * ExecExpr(expr.Right, env);
            }
            else if (expr.Op == "/")
            {
                return ExecExpr(expr.Left, env) / ExecExpr(expr.Right, env);
            }
            else if (expr.Op == "%")
            {
                return ExecExpr(expr.Left, env) % ExecExpr(expr.Right, env);
            }
            else if (expr.Op == "=")
            {
                var left = ExecExpr(expr.Left, env);
                var right = ExecExpr(expr.Right, env);
                return left.SetIndirect(right);
            }
            else if (expr.Op == "+=")
            {
                var left = ExecExpr(expr.Left, env);
                var right = ExecExpr(expr.Right, env);

                var eval = left + right;
                return left.SetIndirect(eval);
            }
            else if (expr.Op == "-=")
            {
                var left = ExecExpr(expr.Left, env);
                var right = ExecExpr(expr.Right, env);

                var eval = left -right;
                return left.SetIndirect(eval);
            }
            else if (expr.Op == "*=")
            {
                var left = ExecExpr(expr.Left, env);
                var right = ExecExpr(expr.Right, env);

                var eval = left * right;
                return left.SetIndirect(eval);
            }
            else if (expr.Op == "/=")
            {
                var left = ExecExpr(expr.Left, env);
                var right = ExecExpr(expr.Right, env);

                var eval = left / right;
                return left.SetIndirect(eval);
            }
            else if (expr.Op == "%=")
            {
                var left = ExecExpr(expr.Left, env);
                var right = ExecExpr(expr.Right, env);

                var eval = left % right;
                return left.SetIndirect(eval);
            }
            else if (expr.Op == "&=")
            {
                var left = ExecExpr(expr.Left, env);
                var right = ExecExpr(expr.Right, env);

                var eval = Value.BitwiseAnd(left, right);
                return left.SetIndirect(eval);
            }
            else if (expr.Op == "|=")
            {
                var left = ExecExpr(expr.Left, env);
                var right = ExecExpr(expr.Right, env);

                var eval = Value.BitwiseOr(left, right);
                return left.SetIndirect(eval);
            }
            else if (expr.Op == "^=")
            {
                var left = ExecExpr(expr.Left, env);
                var right = ExecExpr(expr.Right, env);

                var eval = Value.BitwiseXor(left, right);
                return left.SetIndirect(eval);
            }
            else if (expr.Op == "<<=")
            {
                var left = ExecExpr(expr.Left, env);
                var right = ExecExpr(expr.Right, env);

                var eval = left << right.ToInt();
                return left.SetIndirect(eval);
            }
            else if (expr.Op == ">>=")
            {
                var left = ExecExpr(expr.Left, env);
                var right = ExecExpr(expr.Right, env);

                var eval = left >> right.ToInt();
                return left.SetIndirect(eval);
            }
            return ExecExpr(expr.Left, env);
        }
        Value ExecExpr(TernaryExpression expr, Environment env)
        {
            if (expr.Op == "?" && expr.Op2 == ":")
            {
                var cond = ExecExpr(expr.Left, env).ToBool();
                return cond ? ExecExpr(expr.Mid, env) : ExecExpr(expr.Right, env);
            }
            return default;
        }
        Value ExecExpr(InvocationExpression expr, Environment env)
        {
            Function func = ExecExpr(expr.Function, env).ToFunction();
            var args = expr.Args
                .Select(arg => new Argument(arg.Name, ExecExpr(arg.Expression, env)))
                .ToList();
            return func.Invoke(args);
        }
        Value ExecExpr(SubscriptExpression expr, Environment env)
        {
            var collection = ExecExpr(expr.Collection, env);
            var index = ExecExpr(expr.Index, env);
            return new Reference(
                get: () => collection[index],
                set: v => collection[index] = v
                );
        }
    }
}
