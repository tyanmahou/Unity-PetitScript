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
        (Value, StatementCommand) ExecSwitchStatement(SwitchStatement switchStatement, Enviroment env)
        {
            var condition = ExecExpr(switchStatement.Condition, env).Result;

            // どのセクションを使用するか確定する
            bool IsMatch(ISwitchLabel label, in Value v)
            {
                if (label is SwitchCase caseLabel)
                {
                    return ExecExpr(caseLabel.Expression, env).Result == v;
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
        (Value, StatementCommand) ExecForStatement(ForStatement forStatement, Enviroment env)
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
        (Value, StatementCommand) ExecFunctionStatement(FunctionDeclaration function, Enviroment env)
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
                        arg = ExecExpr(parameters[i].DefaultValue, newEnv);
                    }
                    newEnv.Set(parameters[i].Name, arg);
                }
                return ExecStatement(function.Statement, newEnv).Item1;
            }, function.Paramerters.Select(p => new Argument(p.Name, (Func<Value>)null)).ToArray());
            env.Set(function.Identifier, func);
            return (Value.Invalid, StatementCommand.None);
        }
        ExprResult ExecExpr(IExpression expr, Enviroment env)
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
        ExprResult ExecExpr(BoolLiteral expr)
        {
            return new(Value.Of(expr.Value));
        }
        ExprResult ExecExpr(IntLiteral expr)
        {
            return new(Value.Of(expr.Value));
        }
        ExprResult ExecExpr(FloatLiteral expr)
        {
            return new(Value.Of(expr.Value));
        }
        ExprResult ExecExpr(StringLiteral expr)
        {
            return new(Value.Of(expr.Value));
        }
        ExprResult ExecExpr(StringInterpolation expr, Enviroment env)
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
                    value = ExecExpr(e, env).Result.ToString();
                }
                sb.Append(value);
            }
            return new (Value.Of(sb.ToString()));
        }
        ExprResult ExecExpr(ListExpression expr, Enviroment env)
        {
            List<Value> values = new List<Value>(expr.Elements.Count);
            foreach (var e in expr.Elements)
            {
                values.Add(ExecExpr(e, env));
            }
            return new(Value.Of(values));
        }
        ExprResult ExecExpr(VariableExpression expr, Enviroment env)
        {
            return new ExprResult()
            {
                Result = env.Get(expr.Identifier),
                Address = new IdentAddress()
                {
                    Enviroment = env,
                    Identifier = expr.Identifier,
                }
            };
        }
        ExprResult ExecExpr(PrefixUnaryExpression expr, Enviroment env)
        {
            if (expr.Op == "!")
            {
                return new(!ExecExpr(expr.Right, env).Result);
            }
            else if (expr.Op == "+")
            {
                return new(+ExecExpr(expr.Right, env).Result);
            }
            else if (expr.Op == "-")
            {
                return new(-ExecExpr(expr.Right, env).Result);
            }
            else if (expr.Op == "++")
            {
                var eval = ExecExpr(expr.Right, env);
                var result = eval.Result + Value.Of(1);
                eval.Set(result);
                return new(result, eval.Address);
            }
            else if (expr.Op == "--")
            {
                var eval = ExecExpr(expr.Right, env);
                var result = eval.Result - Value.Of(1);
                eval.Set(result);
                return new(result, eval.Address);

            }
            else if (expr.Op == "~")
            {
                return new(Value.BitwiseNot(ExecExpr(expr.Right, env).Result));
            }
            return ExecExpr(expr.Right, env);
        }
        ExprResult ExecExpr(PostfixUnaryExpression expr, Enviroment env)
        {
            if (expr.Op == "++")
            {
                var eval = ExecExpr(expr.Left, env);
                var result = eval.Result + Value.Of(1);
                eval.Set(result);
                return new(eval.Result);
            }
            else if (expr.Op == "--")
            {
                var eval = ExecExpr(expr.Left, env);
                var result = eval.Result - Value.Of(1);
                eval.Set(result);
                return new(eval.Result);
            }
            return ExecExpr(expr.Left, env);
        }
        ExprResult ExecExpr(BinaryExpression expr, Enviroment env)
        {
            // 短絡評価
            if (expr.Op == "&&")
            {
                return new(ExecExpr(expr.Left, env).Result && ExecExpr(expr.Right, env).Result);
            }
            else if (expr.Op == "||")
            {
                return new(ExecExpr(expr.Left, env).Result || ExecExpr(expr.Right, env).Result);
            }
            else if (expr.Op == "==")
            {
                return new(Value.Of(ExecExpr(expr.Left, env).Result == ExecExpr(expr.Right, env).Result));
            }
            else if (expr.Op == "===")
            {
                return new(Value.Of(Value.Identical(ExecExpr(expr.Left, env).Result, ExecExpr(expr.Right, env).Result)));
            }
            else if (expr.Op == "!=")
            {
                return new(Value.Of(ExecExpr(expr.Left, env).Result != ExecExpr(expr.Right, env).Result));
            }
            else if (expr.Op == "!==")
            {
                return new(Value.Of(Value.NotIdentical(ExecExpr(expr.Left, env).Result, ExecExpr(expr.Right, env).Result)));
            }
            else if (expr.Op == ">")
            {
                return new(Value.Of(ExecExpr(expr.Left, env).Result > ExecExpr(expr.Right, env).Result));
            }
            else if (expr.Op == "<")
            {
                return new(Value.Of(ExecExpr(expr.Left, env).Result < ExecExpr(expr.Right, env).Result));
            }
            else if (expr.Op == ">=")
            {
                return new(Value.Of(ExecExpr(expr.Left, env).Result >= ExecExpr(expr.Right, env).Result));
            }
            else if (expr.Op == "<=")
            {
                return new(Value.Of(ExecExpr(expr.Left, env).Result <= ExecExpr(expr.Right, env).Result));
            }
            else if (expr.Op == "<=>")
            {
                return new(Value.Of(Value.Compare(ExecExpr(expr.Left, env).Result, ExecExpr(expr.Right, env).Result)));
            }
            else if (expr.Op == "&")
            {
                return new(Value.BitwiseAnd(ExecExpr(expr.Left, env).Result, ExecExpr(expr.Right, env).Result));
            }
            else if (expr.Op == "|")
            {
                return new(Value.BitwiseOr(ExecExpr(expr.Left, env).Result, ExecExpr(expr.Right, env).Result));
            }
            else if (expr.Op == "^")
            {
                return new(Value.BitwiseXor(ExecExpr(expr.Left, env).Result, ExecExpr(expr.Right, env).Result));
            }
            else if (expr.Op == "<<")
            {
                return new(ExecExpr(expr.Left, env).Result << ExecExpr(expr.Right, env).Result.ToInt());
            }
            else if (expr.Op == ">>")
            {
                return new(ExecExpr(expr.Left, env).Result >> ExecExpr(expr.Right, env).Result.ToInt());
            }
            else if (expr.Op == "+")
            {
                return new(ExecExpr(expr.Left, env).Result + ExecExpr(expr.Right, env).Result);
            }
            else if (expr.Op == "-")
            {
                return new(ExecExpr(expr.Left, env).Result - ExecExpr(expr.Right, env).Result);
            }
            else if (expr.Op == "*")
            {
                return new(ExecExpr(expr.Left, env).Result * ExecExpr(expr.Right, env).Result);
            }
            else if (expr.Op == "/")
            {
                return new(ExecExpr(expr.Left, env).Result / ExecExpr(expr.Right, env).Result);
            }
            else if (expr.Op == "%")
            {
                return new(ExecExpr(expr.Left, env).Result % ExecExpr(expr.Right, env).Result);
            }
            else if (expr.Op == "=")
            {
                var left = ExecExpr(expr.Left, env);
                var right = ExecExpr(expr.Right, env);
                left.Set(right);
                return new(right.Result, left.Address);
            }
            else if (expr.Op == "+=")
            {
                var left = ExecExpr(expr.Left, env);
                var right = ExecExpr(expr.Right, env);

                var eval = left.Result + right.Result;
                left.Set(eval);
                return new(eval, left.Address);
            }
            else if (expr.Op == "-=")
            {
                var left = ExecExpr(expr.Left, env);
                var right = ExecExpr(expr.Right, env);

                var eval = left.Result - right.Result;
                left.Set(eval);
                return new(eval, left.Address);
            }
            else if (expr.Op == "*=")
            {
                var left = ExecExpr(expr.Left, env);
                var right = ExecExpr(expr.Right, env);

                var eval = left.Result * right.Result;
                left.Set(eval);
                return new(eval, left.Address);
            }
            else if (expr.Op == "/=")
            {
                var left = ExecExpr(expr.Left, env);
                var right = ExecExpr(expr.Right, env);

                var eval = left.Result / right.Result;
                left.Set(eval);
                return new(eval, left.Address);
            }
            else if (expr.Op == "%=")
            {
                var left = ExecExpr(expr.Left, env);
                var right = ExecExpr(expr.Right, env);

                var eval = left.Result % right.Result;
                left.Set(eval);
                return new(eval, left.Address);
            }
            else if (expr.Op == "&=")
            {
                var left = ExecExpr(expr.Left, env);
                var right = ExecExpr(expr.Right, env);

                var eval = Value.BitwiseAnd(left.Result, right.Result);
                left.Set(eval);
                return new(eval, left.Address);
            }
            else if (expr.Op == "|=")
            {
                var left = ExecExpr(expr.Left, env);
                var right = ExecExpr(expr.Right, env);

                var eval = Value.BitwiseOr(left.Result, right.Result);
                left.Set(eval);
                return new(eval, left.Address);
            }
            else if (expr.Op == "^=")
            {
                var left = ExecExpr(expr.Left, env);
                var right = ExecExpr(expr.Right, env);

                var eval = Value.BitwiseXor(left.Result, right.Result);
                left.Set(eval);
                return new(eval, left.Address);
            }
            else if (expr.Op == "<<=")
            {
                var left = ExecExpr(expr.Left, env);
                var right = ExecExpr(expr.Right, env);

                var eval = left.Result << right.Result.ToInt();
                left.Set(eval);
                return new(eval, left.Address);
            }
            else if (expr.Op == ">>=")
            {
                var left = ExecExpr(expr.Left, env);
                var right = ExecExpr(expr.Right, env);

                var eval = left.Result >> right.Result.ToInt();
                left.Set(eval);
                return new(eval, left.Address);
            }
            return ExecExpr(expr.Left, env);
        }
        ExprResult ExecExpr(TernaryExpression expr, Enviroment env)
        {
            if (expr.Op == "?" && expr.Op2 == ":")
            {
                var cond = ExecExpr(expr.Left, env).Result.ToBool();

                ExprResult mid = default;
                ExprResult right = default;
                return new(
                    (cond ? (mid = ExecExpr(expr.Mid, env)).Result : (right = ExecExpr(expr.Right, env)).Result),
                    (cond ? mid.Address : right.Address)
                    );
            }
            return default;
        }
        ExprResult ExecExpr(InvocationExpression expr, Enviroment env)
        {
            Function func = ExecExpr(expr.Function, env).Result.ToFunction();
            var args = expr.Args
                .Select(arg => new Argument(arg.Name, ExecExpr(arg.Expression, env).Result))
                .ToList();
            return new() { Result = func.Invoke(args)};
        }
        ExprResult ExecExpr(SubscriptExpression expr, Enviroment env)
        {
            var collection = ExecExpr(expr.Collection, env);
            var index = ExecExpr(expr.Index, env);

            IAddress address = null;
            if (collection.Address != null)
            {
                var indexAddress = new IndexAddress();
                indexAddress.Parent = collection.Address;
                indexAddress.Index = index.Result;
                address = indexAddress;
            }
            return new()
            {
                Result = collection.Result[index.Result],
                Address = address,
            };
        }
        struct ExprResult
        {
            public Value Result;
            public IAddress Address;

            public ExprResult(in Value result, in IAddress address = null)
            {
                Result = result;
                Address = address;
            }
            public static implicit operator Value(in ExprResult v) => v.Result;
            public static implicit operator bool(in ExprResult v) => v.Result.ToBool();

            public void Set(in Value value)
            {
                Address?.Set(value);
            }
        }
        interface IAddress
        {
            Value Value{  get; }
            void Set(in Value value);
        }
        struct IdentAddress : IAddress
        {
            public Enviroment Enviroment;
            public string Identifier;

            public Value Value
            {
                get => Enviroment.Get(Identifier);
            }
            public void Set(in Value value)
            {
                Enviroment.Set(Identifier, value);
            }
        }
        struct IndexAddress : IAddress
        {
            public IAddress Parent;
            public Value Index;
            public Value Value
            {
                get => Parent.Value[Index];
            }
            public void Set(in Value value)
            {
                Value v = Parent.Value;
                v[Index] = value;
            }
        }
    }
}
