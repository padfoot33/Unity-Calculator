using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

[RequireComponent(typeof(Button))]
public class KeypadButton : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] private TMP_Text label;

    [Header("Token to emit on click")]
    [SerializeField] private string token = "0";

    private void Awake()
    {
        if (label != null && string.IsNullOrEmpty(label.text))
            label.text = token;
    }

    private void Update()
    {
        if(token != "AC")
        {
            if (Input.GetKeyUp(token))
            {
                HandleClick();
            }
        }
        else
        {
            if (Input.GetKeyUp("c"))
            {
                HandleClick();
            }
        }
    }

    private void OnValidate()
    {
        // Keep editor view in sync when you change token
        if (label != null)
            label.text = token;
    }

    public void HandleClick()
    {
        CalculatorUI.Instance.OnKeyPressed(token);
    }

    //Only if we want to instantiate objects
    public void Configure(string newToken, string labelText = null, Sprite sprite = null)
    {
        token = newToken;
        if (label != null)
            label.text = string.IsNullOrEmpty(labelText) ? newToken : labelText;
    }

    public string Token => token;
}
