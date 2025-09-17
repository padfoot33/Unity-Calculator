using UnityEngine;
using TMPro;
using System;
using System.Text;

public class CalculatorUI : MonoBehaviour
{
    public static CalculatorUI Instance { get; private set; }
    [Header("UI References")]
    [SerializeField] private TMP_Text expressionText;
    [SerializeField] private TMP_Text resultText;

    [Header("Settings")]
    [Tooltip("Max characters allowed in the expression display.")]
    [SerializeField] private int maxExpressionLen = 64;

    [Tooltip("Decimal digits to show when not an integer.")]
    [SerializeField] private int maxFractionDigits = 10;

    private readonly StringBuilder _expr = new StringBuilder(64);
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    public void OnKeyPressed(string token)
    {
        if (string.IsNullOrEmpty(token)) return;

        switch (token)
        {
            case "=":
                Evaluate();
                break;

            case "AC":
                ResetAll();
                break;

            case "C":
                Backspace();
                break;

            case ".":
                AppendDot();
                break;

            case "+":
            case "-":
            case "*":
            case "/":
                AppendOperator(token[0]);
                break;

            default:
                // treat anything else as digits (0–9)
                AppendDigitToken(token);
                break;
        }

        RefreshExpressionLabel();
    }

    private void AppendDigitToken(string token)
    {
        if (_expr.Length >= maxExpressionLen) return;

        // Only allow digits in this path
        for (int i = 0; i < token.Length; i++)
        {
            if (!char.IsDigit(token[i])) return;
        }

        _expr.Append(token);
    }

    private void AppendDot()
    {
        if (_expr.Length >= maxExpressionLen) return;

        // If expression is empty, start a number like "0."
        if (_expr.Length == 0)
        {
            _expr.Append("0.");
            return;
        }

        // Disallow dot right after another operator (start "0.")
        char last = _expr[_expr.Length - 1];
        if (IsOperator(last))
        {
            _expr.Append("0.");
            return;
        }

        // Only if current number segment doesn't already contain '.'
        var seg = GetCurrentNumberSegment();
        if (seg.IndexOf('.') >= 0) return;

        _expr.Append('.');
    }

    private void AppendOperator(char op)
    {
        if (_expr.Length == 0)
        {
            // Only allow leading '-' as a sign
            if (op == '-') _expr.Append(op);
            return;
        }

        char last = _expr[_expr.Length - 1];

        if (IsOperator(last))
        {
            // allow "+-" or "*-" or "/-" as (operator + unary minus)
            if (op == '-' && last != '-')
            {
                _expr.Append(op);
                return;
            }

            // Replace last operator with the new one (prevents sequences like '++', '**', etc.)
            _expr[_expr.Length - 1] = op;
            return;
        }

        if (_expr.Length < maxExpressionLen)
            _expr.Append(op);
    }

    private void Backspace()
    {
        if (_expr.Length == 0) return;
        _expr.Remove(_expr.Length - 1, 1);
    }

    private void ResetAll()
    {
        _expr.Clear();
        if (resultText != null) resultText.text = "";
    }

    private void Evaluate()
    {
        if (_expr.Length == 0)
        {
            if (resultText != null) resultText.text = "";
            return;
        }

        // Prevent ending on an operator (common user typo); try to be forgiving by trimming it.
        if (IsOperator(_expr[_expr.Length - 1]))
        {
            // If it ends with a unary minus after an operator (like "5*-"), it's invalid; just show Error.
            if (_expr.Length >= 2 && IsOperator(_expr[_expr.Length - 2]) && _expr[_expr.Length - 1] == '-')
            {
                if (resultText != null) resultText.text = "Error";
                return;
            }
            _expr.Remove(_expr.Length - 1, 1);
            RefreshExpressionLabel();
        }

        // Evaluate via your ExpressionEvaluator
        if (ExpressionEvaluator.TryEvaluate(_expr.ToString(), out double value))
        {
            if (resultText != null) resultText.text = FormatNumber(value);
        }
        else
        {
            if (resultText != null) resultText.text = "Error";
        }
    }

   
    private void RefreshExpressionLabel()
    {
        if (expressionText != null)
            expressionText.text = _expr.ToString();
    }

    private static bool IsOperator(char c) => c == '+' || c == '-' || c == '*' || c == '/';

    // Returns the substring representing the current number segment
    // (the chars after the most recent binary operator, ignoring a leading sign)
    private string GetCurrentNumberSegment()
    {
        if (_expr.Length == 0) return "";

        int i = _expr.Length - 1;

        // Walk backward until you hit an operator that's truly binary.
        // Handle a possible unary minus just after an operator/start.
        int start = 0;
        for (int k = _expr.Length - 1; k >= 0; k--)
        {
            char c = _expr[k];
            if (IsOperator(c))
            {
                // If this is a '-' and is acting as a sign (unary), keep going.
                bool isUnaryMinus = (c == '-') && (k == 0 || IsOperator(_expr[k - 1]));
                if (!isUnaryMinus)
                {
                    start = k + 1;
                    break;
                }
            }
            if (k == 0) start = 0;
        }

        return _expr.ToString(start, _expr.Length - start);
    }

    private string FormatNumber(double v)
    {
        // If very close to an integer, show integer
        double rounded = Math.Round(v);
        if (Math.Abs(v - rounded) < 1e-10)
            return rounded.ToString();

        // Otherwise show trimmed decimals up to maxFractionDigits
        string fmt = "0." + new string('#', Mathf.Clamp(maxFractionDigits, 0, 15));
        return v.ToString(fmt, System.Globalization.CultureInfo.InvariantCulture);
    }
}
