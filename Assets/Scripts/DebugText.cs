using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class DebugText : MonoBehaviour
{
    public static DebugText Instance;

    public TMPro.TextMeshProUGUI debugText;
    public List<string> debugLines = new List<string>();

    private List<string> _allLines = new List<string>();

    const int lineCount = 30;

    void OnEnable()
    {
        Instance = this;

        HandleLog("<color=#008000>Opened debug view!</color>");
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    public void HandleLog(string logString, string stackTrace = "", LogType type = LogType.Log)
    {
        debugLines.Add($"\n[{DateTime.Now}] {logString}");
        _allLines.Add($"\n[{DateTime.Now}] {logString}");

        if (debugLines.Count > lineCount)
        {
            debugLines.RemoveAt(0);
        }

        string lines = "";
        for (int i = 0; i < debugLines.Count; i++)
        {
            lines += debugLines[i];
        }

        if (debugText)
            debugText.text = lines;
    }
}