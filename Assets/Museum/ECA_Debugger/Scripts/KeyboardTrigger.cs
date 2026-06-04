using System;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_InputField))]
public class KeyboardTrigger : MonoBehaviour
{
    private TMP_InputField textField;
    private TouchScreenKeyboard keyboard;

    void Start()
    {
        textField = GetComponent<TMP_InputField>();
        textField.onSelect.AddListener(delegate { OpenKeyboard(); });
    }
    
    public void OpenKeyboard()
    {
        #if !UNITY_EDITOR
        keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, false, false);
        #endif
    }
    
    void Update()
    {
        #if !UNITY_EDITOR
        if (keyboard != null && keyboard.active)
            if (textField.text != keyboard.text)
                textField.text = keyboard.text;
        #endif
    }
}