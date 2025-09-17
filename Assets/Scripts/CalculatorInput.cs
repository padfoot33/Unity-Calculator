using UnityEngine;
using UnityEngine.InputSystem; // New Input System
using TMPro;
using UnityEngine.InputSystem.Controls;

public class CalculatorInput : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CalculatorUI calculatorUI;
    [SerializeField] private InputActionAsset actionsAsset;

    private InputAction actDigit, actDot, actAdd, actSub, actMul, actDiv, actEquals, actClear, actReset;

    private void Awake()
    {
        if (calculatorUI == null)
            calculatorUI = GetComponent<CalculatorUI>();

        // Find the action map + actions by name
        var map = actionsAsset.FindActionMap("Calculator", throwIfNotFound: true);
        actDigit = map.FindAction("Digit", true);
        actDot = map.FindAction("Dot", true);
        actAdd = map.FindAction("Add", true);
        actSub = map.FindAction("Sub", true);
        actMul = map.FindAction("Mul", true);
        actDiv = map.FindAction("Div", true);
        actEquals = map.FindAction("Equals", true);
        actClear = map.FindAction("Clear", true);
        actReset = map.FindAction("Reset", true);
    }

    private void OnEnable()
    {
        actDigit.performed += OnDigit;
        actDot.performed += ctx => calculatorUI.OnKeyPressed(".");
        actAdd.performed += ctx => calculatorUI.OnKeyPressed("+");
        actSub.performed += ctx => calculatorUI.OnKeyPressed("-");
        actMul.performed += ctx => calculatorUI.OnKeyPressed("*");
        actDiv.performed += ctx => calculatorUI.OnKeyPressed("/");
        actEquals.performed += ctx => calculatorUI.OnKeyPressed("=");
        actClear.performed += ctx => calculatorUI.OnKeyPressed("C");
        actReset.performed += ctx => calculatorUI.OnKeyPressed("AC");

        actDigit.Enable(); actDot.Enable(); actAdd.Enable(); actSub.Enable();
        actMul.Enable(); actDiv.Enable(); actEquals.Enable();
        actClear.Enable(); actReset.Enable();
    }

    private void OnDisable()
    {
        actDigit.performed -= OnDigit;
        actDot.performed -= ctx => calculatorUI.OnKeyPressed(".");
        actAdd.performed -= ctx => calculatorUI.OnKeyPressed("+");
        actSub.performed -= ctx => calculatorUI.OnKeyPressed("-");
        actMul.performed -= ctx => calculatorUI.OnKeyPressed("*");
        actDiv.performed -= ctx => calculatorUI.OnKeyPressed("/");
        actEquals.performed -= ctx => calculatorUI.OnKeyPressed("=");
        actClear.performed -= ctx => calculatorUI.OnKeyPressed("C");
        actReset.performed -= ctx => calculatorUI.OnKeyPressed("AC");

        actDigit.Disable(); actDot.Disable(); actAdd.Disable(); actSub.Disable();
        actMul.Disable(); actDiv.Disable(); actEquals.Disable();
        actClear.Disable(); actReset.Disable();
    }

    private void OnDigit(InputAction.CallbackContext ctx)
    {
        var keyControl = ctx.control as KeyControl;
        if (keyControl == null) return;

        string token = KeyToDigit(keyControl.keyCode);
        if (token != null)
            calculatorUI.OnKeyPressed(token);
    }

    private string KeyToDigit(Key keyCode)
    {
        switch (keyCode)
        {
            case Key.Digit0: case Key.Numpad0: return "0";
            case Key.Digit1: case Key.Numpad1: return "1";
            case Key.Digit2: case Key.Numpad2: return "2";
            case Key.Digit3: case Key.Numpad3: return "3";
            case Key.Digit4: case Key.Numpad4: return "4";
            case Key.Digit5: case Key.Numpad5: return "5";
            case Key.Digit6: case Key.Numpad6: return "6";
            case Key.Digit7: case Key.Numpad7: return "7";
            case Key.Digit8: case Key.Numpad8: return "8";
            case Key.Digit9: case Key.Numpad9: return "9";
            default: return null;
        }
    }
}
