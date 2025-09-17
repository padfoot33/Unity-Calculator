using System;
using System.Collections.Generic;
using System.Globalization;

public static class ExpressionEvaluator
{
    public enum TokType { Number, Op }

    public struct Token
    {
        public TokType Type;
        public double Number; // valid if Type == Number
        public char Op;       // '+', '-', '*', '/' if Type == Op

        public static Token Num(double v) => new Token { Type = TokType.Number, Number = v };
        public static Token O(char c) => new Token { Type = TokType.Op, Op = c };
        public override string ToString() => Type == TokType.Number ? Number.ToString(CultureInfo.InvariantCulture) : Op.ToString();
    }

    public static bool TryEvaluate(string expr, out double result)
    {
        result = 0;

        if (string.IsNullOrWhiteSpace(expr))
            return false;

        try
        {
            var tokens = Tokenize(expr);
            if (tokens.Count == 0) 
                return false;

            var reduced = ReduceMulDiv(tokens);
            result = ReduceAddSub(reduced); 
            return true;
        }
        catch
        {
            result = 0;
            return false;
        }
    }

    private static List<Token> Tokenize(string expr)
    {
        var tokens = new List<Token>();
        int i = 0;
        int n = expr.Length;
        bool expectNumber = true; 

        while (i < n)
        {
            char ch = expr[i];

            if (char.IsWhiteSpace(ch))
            {
                i++;
                continue;
            }

            if (expectNumber)
            {
                int sign = 1;
                //Check for + & - in the start of expr
                if (ch == '+' || ch == '-')
                {
                    if (ch == '-') 
                        sign = -1;
                    
                    i++;
                   
                    if (i >= n)
                        throw new Exception("Dangling sign at end.");

                    ch = expr[i];
                    
                    if (!char.IsDigit(ch) && ch != '.')
                        throw new Exception("Sign not followed by number.");
                }

               
                bool hasDot = false;
                int start = i;
                while (i < n)
                {
                    char c = expr[i];
                    if (char.IsDigit(c))
                    {
                        i++;
                    }
                    else if (c == '.')
                    {
                        if (hasDot) 
                            throw new Exception("Multiple dots in number.");
                        hasDot = true;
                        i++;
                    }
                    else break;
                }

                if (i == start) 
                    throw new Exception("Invalid number.");

                //Create a str of expr to add to our list
                string numStr = expr.Substring(start, i - start);
                if (numStr.StartsWith(".", StringComparison.Ordinal))
                    numStr = "0" + numStr;

                double value = double.Parse(numStr, CultureInfo.InvariantCulture) * sign;
                tokens.Add(Token.Num(value));
                expectNumber = false;
                continue;
            }
            else
            {
                if (IsOp(ch))
                {
                    tokens.Add(Token.O(ch));
                    i++;
                    expectNumber = true;
                }
                else
                {
                    throw new Exception($"Unexpected character '{ch}' while expecting operator.");
                }
            }
        }

        if (expectNumber)
            throw new Exception("Expression ends with operator.");

        // basic structural validation: tokens alternate Number, Op, Number, etc
        if (!IsAlternating(tokens))
            throw new Exception("Bad token ordering.");

        return tokens;
    }

    private static bool IsOp(char c) => c == '+' || c == '-' || c == '*' || c == '/';

    private static bool IsAlternating(List<Token> t)
    {
        if (t.Count == 0 || t[0].Type != TokType.Number) return false;

        for (int i = 1; i < t.Count; i++)
        {
            var expect = (i % 2 == 1) ? TokType.Op : TokType.Number;
            if (t[i].Type != expect) 
                return false;
        }
        return true;
    }

    private static List<Token> ReduceMulDiv(List<Token> t)
    {
        var outList = new List<Token>();
        outList.Add(t[0]);

        int i = 1;
        while (i < t.Count)
        {
            var opTok = t[i]; 
            var rhsTok = t[i + 1];

            if (opTok.Op == '*' || opTok.Op == '/')
            {
                double lhs = outList[outList.Count - 1].Number;
                double rhs = rhsTok.Number;

                if (opTok.Op == '/')
                {
                    if (Math.Abs(rhs) < 1e-15) 
                        throw new DivideByZeroException();
                    outList[outList.Count - 1] = Token.Num(lhs / rhs);
                }
                else
                {
                    outList[outList.Count - 1] = Token.Num(lhs * rhs);
                }
                i += 2;
            }
            else
            {
                outList.Add(opTok);
                outList.Add(rhsTok);
                i += 2;
            }
        }
        return outList;
    }

    private static double ReduceAddSub(List<Token> t)
    {
        double result = t[0].Number;
        int i = 1;
        while (i < t.Count)
        {
            var opTok = t[i];
            var rhsTok = t[i + 1];

            if (opTok.Op == '+') result += rhsTok.Number;
            else if (opTok.Op == '-') result -= rhsTok.Number;
            else throw new Exception("Unexpected operator in Add/Sub pass.");

            i += 2;
        }
        return result;
    }
}
