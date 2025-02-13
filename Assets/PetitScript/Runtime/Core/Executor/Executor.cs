using Petit.Core.AST;
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
            else if (statement is SwitchStatement switchStatement)
            {
                return ExecSwitchStatement(switchStatement);
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
        (Value, StatementCommand) ExecSwitchStatement(SwitchStatement switchStatement)
        {
            var condition = ExecExpr(switchStatement.Condition).Item1;

            // どのセクションを使用するか確定する
            bool IsMatch(ISwitchLabel label, in Value v)
            {
                if (label is SwitchCase caseLabel)
                {
                    return ExecExpr(caseLabel.Expression).Item1 == v;
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
                        result = ExecStatement(statement);
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
        (Value, StatementCommand) ExecWhileStatement(WhileStatement whileStatement)
        {
            (Value, StatementCommand) result = default;
            while (ExecExpr(whileStatement.Cond).Item1)
            {
                result = ExecStatement(whileStatement.Statement);
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
        (Value, StatementCommand) ExecForStatement(ForStatement forStatement)
        {
            (Value, StatementCommand) result = default;
            for(ExecExpr(forStatement.Init);  ExecExpr(forStatement.Cond).Item1; ExecExpr(forStatement.Loop))
            {
                result = ExecStatement(forStatement.Statement);
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
            else if (expr is FunctionCallExpression func)
            {
                return ExecExpr(func);
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
            if (expr.Op == "!")
            {
                return (!ExecExpr(expr.Right).Item1, null);
            }
            else if (expr.Op == "+")
            {
                return (+ExecExpr(expr.Right).Item1, null);
            }
            else if (expr.Op == "-")
            {
                return (-ExecExpr(expr.Right).Item1, null);
            }
            else if (expr.Op == "++")
            {
                var eval = ExecExpr(expr.Right);
                var result = eval.Item1 + new Value(1);
                if (eval.Item2 != null)
                {
                    _env.Variables.Set(eval.Item2, result);
                }
                return (result, eval.Item2);
            }
            else if (expr.Op == "--")
            {
                var eval = ExecExpr(expr.Right);
                var result = eval.Item1 - new Value(1);
                if (eval.Item2 != null)
                {
                    _env.Variables.Set(eval.Item2, result);
                }
                return (result, eval.Item2);
            }
            else if (expr.Op == "~")
            {
                return (Value.BitwiseNot(ExecExpr(expr.Right).Item1), null);
            }
            return ExecExpr(expr.Right);
        }
        (Value, string) ExecExpr(PostfixUnaryExpression expr)
        {
            if (expr.Op == "++")
            {
                var eval = ExecExpr(expr.Left);
                var result = eval.Item1 + new Value(1);
                if (eval.Item2 != null)
                {
                    _env.Variables.Set(eval.Item2, result);
                }
                return (eval.Item1, null);
            }
            else if (expr.Op == "--")
            {
                var eval = ExecExpr(expr.Left);
                var result = eval.Item1 - new Value(1);
                if (eval.Item2 != null)
                {
                    _env.Variables.Set(eval.Item2, result);
                }
                return (eval.Item1, null);
            }
            return ExecExpr(expr.Left);
        }
        (Value, string) ExecExpr(BinaryExpression expr)
        {
            // 短絡評価
            if (expr.Op == "&&")
            {
                return (ExecExpr(expr.Left).Item1 && ExecExpr(expr.Right).Item1, null);
            }
            else if (expr.Op == "||")
            {
                return (ExecExpr(expr.Left).Item1 || ExecExpr(expr.Right).Item1, null);
            }
            else if (expr.Op == "==")
            {
                return (new Value(ExecExpr(expr.Left).Item1 == ExecExpr(expr.Right).Item1), null);
            }
            else if (expr.Op == "===")
            {
                return (new Value(Value.Identical(ExecExpr(expr.Left).Item1, ExecExpr(expr.Right).Item1)), null);
            }
            else if (expr.Op == "!=")
            {
                return (new Value(ExecExpr(expr.Left).Item1 != ExecExpr(expr.Right).Item1), null);
            }
            else if (expr.Op == "!==")
            {
                return (new Value(Value.NotIdentical(ExecExpr(expr.Left).Item1, ExecExpr(expr.Right).Item1)), null);
            }
            else if (expr.Op == ">")
            {
                return (new Value(ExecExpr(expr.Left).Item1 > ExecExpr(expr.Right).Item1), null);
            }
            else if (expr.Op == "<")
            {
                return (new Value(ExecExpr(expr.Left).Item1 < ExecExpr(expr.Right).Item1), null);
            }
            else if (expr.Op == ">=")
            {
                return (new Value(ExecExpr(expr.Left).Item1 >= ExecExpr(expr.Right).Item1), null);
            }
            else if (expr.Op == "<=")
            {
                return (new Value(ExecExpr(expr.Left).Item1 <= ExecExpr(expr.Right).Item1), null);
            }
            else if (expr.Op == "<=>")
            {
                return (new Value(Value.Compare(ExecExpr(expr.Left).Item1, ExecExpr(expr.Right).Item1)), null);
            }
            else if (expr.Op == "&")
            {
                return (Value.BitwiseAnd(ExecExpr(expr.Left).Item1, ExecExpr(expr.Right).Item1), null);
            }
            else if (expr.Op == "|")
            {
                return (Value.BitwiseOr(ExecExpr(expr.Left).Item1, ExecExpr(expr.Right).Item1), null);
            }
            else if (expr.Op == "^")
            {
                return (Value.BitwiseXor(ExecExpr(expr.Left).Item1, ExecExpr(expr.Right).Item1), null);
            }
            else if (expr.Op == "<<")
            {
                return (ExecExpr(expr.Left).Item1 << ExecExpr(expr.Right).Item1.ToInt(), null);
            }
            else if (expr.Op == ">>")
            {
                return (ExecExpr(expr.Left).Item1 >> ExecExpr(expr.Right).Item1.ToInt(), null);
            }
            else if (expr.Op == "+")
            {
                return (ExecExpr(expr.Left).Item1 + ExecExpr(expr.Right).Item1, null);
            }
            else if (expr.Op == "-")
            {
                return (ExecExpr(expr.Left).Item1 - ExecExpr(expr.Right).Item1, null);
            }
            else if (expr.Op == "*")
            {
                return (ExecExpr(expr.Left).Item1 * ExecExpr(expr.Right).Item1, null);
            }
            else if (expr.Op == "/")
            {
                return (ExecExpr(expr.Left).Item1 / ExecExpr(expr.Right).Item1, null);
            }
            else if (expr.Op == "%")
            {
                return (ExecExpr(expr.Left).Item1 % ExecExpr(expr.Right).Item1, null);
            }
            else if (expr.Op == "=")
            {
                var left = ExecExpr(expr.Left);
                var right = ExecExpr(expr.Right);
                if (left.Item2 != null)
                {
                    _env.Variables.Set(left.Item2, right.Item1);
                }
                return (right.Item1, left.Item2);
            }
            else if (expr.Op == "+=")
            {
                var left = ExecExpr(expr.Left);
                var right = ExecExpr(expr.Right);

                var eval = left.Item1 + right.Item1;
                if (left.Item2 != null)
                {
                    _env.Variables.Set(left.Item2, eval);
                }
                return (eval, left.Item2);
            }
            else if (expr.Op == "-=")
            {
                var left = ExecExpr(expr.Left);
                var right = ExecExpr(expr.Right);

                var eval = left.Item1 - right.Item1;
                if (left.Item2 != null)
                {
                    _env.Variables.Set(left.Item2, eval);
                }
                return (eval, left.Item2);
            }
            else if (expr.Op == "*=")
            {
                var left = ExecExpr(expr.Left);
                var right = ExecExpr(expr.Right);

                var eval = left.Item1 * right.Item1;
                if (left.Item2 != null)
                {
                    _env.Variables.Set(left.Item2, eval);
                }
                return (eval, left.Item2);
            }
            else if (expr.Op == "/=")
            {
                var left = ExecExpr(expr.Left);
                var right = ExecExpr(expr.Right);

                var eval = left.Item1 / right.Item1;
                if (left.Item2 != null)
                {
                    _env.Variables.Set(left.Item2, eval);
                }
                return (eval, left.Item2);
            }
            else if (expr.Op == "%=")
            {
                var left = ExecExpr(expr.Left);
                var right = ExecExpr(expr.Right);

                var eval = left.Item1 % right.Item1;
                if (left.Item2 != null)
                {
                    _env.Variables.Set(left.Item2, eval);
                }
                return (eval, left.Item2);
            }
            else if (expr.Op == "&=")
            {
                var left = ExecExpr(expr.Left);
                var right = ExecExpr(expr.Right);

                var eval = Value.BitwiseAnd(left.Item1, right.Item1);
                if (left.Item2 != null)
                {
                    _env.Variables.Set(left.Item2, eval);
                }
                return (eval, left.Item2);
            }
            else if (expr.Op == "|=")
            {
                var left = ExecExpr(expr.Left);
                var right = ExecExpr(expr.Right);

                var eval = Value.BitwiseOr(left.Item1, right.Item1);
                if (left.Item2 != null)
                {
                    _env.Variables.Set(left.Item2, eval);
                }
                return (eval, left.Item2);
            }
            else if (expr.Op == "^=")
            {
                var left = ExecExpr(expr.Left);
                var right = ExecExpr(expr.Right);

                var eval = Value.BitwiseXor(left.Item1, right.Item1);
                if (left.Item2 != null)
                {
                    _env.Variables.Set(left.Item2, eval);
                }
                return (eval, left.Item2);
            }
            else if (expr.Op == "<<=")
            {
                var left = ExecExpr(expr.Left);
                var right = ExecExpr(expr.Right);

                var eval = left.Item1 << right.Item1.ToInt();
                if (left.Item2 != null)
                {
                    _env.Variables.Set(left.Item2, eval);
                }
                return (eval, left.Item2);
            }
            else if (expr.Op == ">>=")
            {
                var left = ExecExpr(expr.Left);
                var right = ExecExpr(expr.Right);

                var eval = left.Item1 >> right.Item1.ToInt();
                if (left.Item2 != null)
                {
                    _env.Variables.Set(left.Item2, eval);
                }
                return (eval, left.Item2);
            }
            return ExecExpr(expr.Left);
        }
        (Value, string) ExecExpr(TernaryExpression expr)
        {
            if (expr.Op == "?" && expr.Op2 == ":")
            {
                var cond = ExecExpr(expr.Left).Item1.ToBool();

                (Value, string) mid = default;
                (Value, string) right = default;
                return (
                    (cond ? (mid = ExecExpr(expr.Mid)).Item1 : (right = ExecExpr(expr.Right)).Item1),
                    (cond ? mid.Item2 : right.Item2)
                    );
            }
            return default;
        }
        (Value, string) ExecExpr(FunctionCallExpression expr)
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
            Function func = _env.Variables.GetFunc(ident);
            var args = expr.Args
                .Select(arg => new Argument(arg.Name, ExecExpr(arg.Expression).Item1))
                .ToList();
            return (func.Invoke(args), null);
        }
        Enviroment _env;
    }
}
