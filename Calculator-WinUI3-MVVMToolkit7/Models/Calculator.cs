using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator_WinUI3_MVVMToolkit7.Models
{
    enum Token
    {
        D0, D1, D2, D3, D4, D5, D6, D7, D8, D9,
        Dot, Invert,
        Add, Sub, Mul, Div, Eval, Clear, ClearAll,
    }

    enum Operator
    {
        Add, Sub, Mul, Div
    }

    internal static class TokenExt
    {
        public static string PerttyString(this Token token)
        {
            return token switch
            {
                Token.D0 => "0",
                Token.D1 => "1",
                Token.D2 => "2",
                Token.D3 => "3",
                Token.D4 => "4",
                Token.D5 => "5",
                Token.D6 => "6",
                Token.D7 => "7",
                Token.D8 => "8",
                Token.D9 => "9",
                Token.Dot => ".",
                Token.Invert => "+/-",
                Token.Add => "+",
                Token.Sub => "-",
                Token.Mul => "*",
                Token.Div => "/",
                Token.Eval => "=",
                Token.Clear => "C",
                Token.ClearAll => "CA",
                _ => throw new NotImplementedException(),
            };
        }

        public static bool TryParse(out Token token, string str)
        {
            switch (str)
            {
                case "0": token = Token.D0; break;
                case "1": token = Token.D1; break;
                case "2": token = Token.D2; break;
                case "3": token = Token.D3; break;
                case "4": token = Token.D4; break;
                case "5": token = Token.D5; break;
                case "6": token = Token.D6; break;
                case "7": token = Token.D7; break;
                case "8": token = Token.D8; break;
                case "9": token = Token.D9; break;
                case ".": token = Token.Dot; break;
                case "+/-": token = Token.Invert; break;
                case "+": token = Token.Add; break;
                case "-": token = Token.Sub; break;
                case "*": token = Token.Mul; break;
                case "/": token = Token.Div; break;
                case "=": token = Token.Eval; break;
                case "C": token = Token.Clear; break;
                case "CA": token = Token.ClearAll; break;
                default: token = Token.ClearAll; return false;
            }
            return true;
        }

        public static bool TryToOperator(this Token token, out Operator op)
        {
            switch (token)
            {
                case Token.Add: op = Operator.Add; break;
                case Token.Sub: op = Operator.Sub; break;
                case Token.Mul: op = Operator.Mul; break;
                case Token.Div: op = Operator.Div; break;
                default: op = new(); return false;
            }
            return true;
        }

        public static bool TryToDigit(this Token token, out int digit)
        {
            switch (token)
            {
                case Token.D0: digit = 0; break;
                case Token.D1: digit = 1; break;
                case Token.D2: digit = 2; break;
                case Token.D3: digit = 3; break;
                case Token.D4: digit = 4; break;
                case Token.D5: digit = 5; break;
                case Token.D6: digit = 6; break;
                case Token.D7: digit = 7; break;
                case Token.D8: digit = 8; break;
                case Token.D9: digit = 9; break;
                default: digit = -1; return false;
            }
            return true;
        }
    }

    internal static class StackExt
    {
        public static T Pop<T>(this Stack<T> stack, T dflt)
        {
            if (stack.Count > 0)
                return stack.Pop();
            return dflt;
        }

        public static T Peek<T>(this Stack<T> stack, T dflt)
        {
            if (stack.Count > 0)
                return stack.Peek();
            return dflt;
        }
    }

    internal static class OperatorExt
    {
        public static void Eval(this Operator op, Stack<decimal> stack)
        {
            if (op == Operator.Add)
            {
                var rhs = stack.Pop(0);
                var lhs = stack.Pop(rhs);
                stack.Push(lhs + rhs);
            }
            else if (op == Operator.Sub)
            {
                var rhs = stack.Pop(0);
                var lhs = stack.Pop(rhs);
                stack.Push(lhs - rhs);
            }
            else if (op == Operator.Mul)
            {
                var rhs = stack.Pop(0);
                var lhs = stack.Pop(rhs);
                stack.Push(lhs * rhs);
            }
            else if (op == Operator.Div)
            {
                var rhs = stack.Pop(1);
                var lhs = stack.Pop(rhs);
                if (rhs == 0)
                    stack.Push(decimal.MaxValue);
                else
                    stack.Push(lhs / rhs);
            }
            else
                throw new NotImplementedException();
        }

        public static string PrettyString(this Operator op)
        {
            return op switch
            {
                Operator.Add => "+",
                Operator.Sub => "-",
                Operator.Mul => "*",
                Operator.Div => "/",
                _ => throw new NotImplementedException()
            };
        }
    }

    internal class Calculator
    {
        public enum State
        {
            InputNum,
            InputOp,
            Result,
        }

        public State state { get; private set; } = State.Result;
        public Operator? Op { get; private set; } = null;
        private readonly NumberBuilder Nb = new();
        private readonly Stack<decimal> NumberStack = new();

        public decimal? Result() => PeekStack();

        public decimal PeekStack(decimal dflt) => NumberStack.Peek(dflt);

        public decimal? PeekStack()
        {
            if(NumberStack.TryPeek(out var result))
                return result;
            return null;
        }

        public string CurrentNumberBuilder() => Nb.ToString();

        public IEnumerable<decimal> EnumerateStack()
        {
            foreach(var item in NumberStack)
                yield return item;
        }


        private void ClearAll()
        {
            state = State.Result;
            Op = null;
            Nb.Clear();
            NumberStack.Clear();
        }

        private void PushNb()
        {
            var num = Nb.Build();
            if (num is decimal num_)
                NumberStack.Push(num_);
            Nb.Clear();
        }

        public void AddToken(Token token)
        {
            if (state == State.Result)
            {
                if (token.TryToDigit(out var digit))
                {
                    state = State.InputNum;
                    Nb.AddDigit(digit);
                }
                else if (token == Token.Dot)
                {
                    state = State.InputNum;
                    Nb.AddDot();
                }
                else if (token == Token.Invert)
                {
                    state = State.InputNum;
                    if (NumberStack.TryPeek(out var prev))
                        Nb.Clear(prev);
                    Nb.Invert();
                }
                else if (token.TryToOperator(out var op))
                {
                    state = State.InputOp;
                    Op = op;
                }
                else if (token == Token.Eval) { /*Nothing*/}
                else if (token == Token.Clear || token == Token.ClearAll)
                    ClearAll();
                else
                    throw new NotImplementedException();
            }
            else if (state == State.InputNum)
            {
                if (token.TryToDigit(out var digit))
                    Nb.AddDigit(digit);
                else if (token == Token.Dot)
                    Nb.AddDot();
                else if (token == Token.Invert)
                    Nb.Invert();
                else if (token.TryToOperator(out var op))
                {
                    state = State.InputOp;
                    PushNb();
                    Op = op;
                }
                else if (token == Token.Eval)
                {
                    state = State.Result;
                    PushNb();
                    Op?.Eval(NumberStack);
                }
                else if (token == Token.Clear)
                    Nb.Clear();
                else if (token == Token.ClearAll)
                    ClearAll();
                else
                    throw new NotImplementedException();
            }
            else if (state == State.InputOp)
            {
                if (token.TryToDigit(out var digit))
                {
                    state = State.InputNum;
                    Nb.AddDigit(digit);
                }
                else if (token == Token.Dot)
                {
                    state = State.InputNum;
                    Nb.AddDot();
                }
                else if (token == Token.Invert)
                {
                    state = State.InputNum;
                    if (NumberStack.TryPeek(out var prev))
                        Nb.Clear(prev);
                    Nb.Invert();
                }
                else if (token.TryToOperator(out var op))
                    Op = op;
                else if (token == Token.Eval)
                {
                    state = State.Result;
                    Op?.Eval(NumberStack);
                }
                else
                    throw new NotImplementedException();
            }
            else
                throw new NotImplementedException();
        }
    }
}
