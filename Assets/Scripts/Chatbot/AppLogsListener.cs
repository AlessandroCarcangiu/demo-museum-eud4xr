using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class AppLogsListener : MonoBehaviour
{
    public TMP_Text DebugText;
    string output = "Logger:";
    string stack = "";
    
    private bool isListening = false;
    void Awake()
    {
        Application.logMessageReceived += HandleLog;
        isListening = true;
    }

    private void OnEnable()
    {
        if (!isListening)
        {
            Application.logMessageReceived += HandleLog;
            isListening = true;
        }
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
        isListening = false;
    }
    
    void HandleLog(string logString, string stackTrace, LogType type)
    {
        List<string> TO_STRIP = new() { "[MRTK3]" };
        // if (TO_STRIP.Any(logString.Contains))
        // {
            // return;
        // }
        output += "\n\n>" + logString + " (Stack: " + stackTrace + ")";
        stack = stackTrace;

        DebugText.text = output;

    }
}
