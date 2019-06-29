using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CreatePHPClasses : EditorWindow
{
    private Dictionary<string, bool> generationResults = new Dictionary<string, bool>();

    [MenuItem("PHP/Create PHP Classes")]
    public static void Initialize()
    {
        var window = (CreatePHPClasses)EditorWindow.GetWindow(typeof(CreatePHPClasses), true, "Create PHP Classes");
    }

    private void Generate()
    {
        generationResults = PHPClassWriter.WriteAll();
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Generate"))
        {
            Generate();
        }

        GUILayout.BeginVertical();
        foreach (var task in generationResults)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(task.Key, GUILayout.ExpandWidth(true));
            GUILayout.Label(task.Value ? "✓" : "", GUILayout.ExpandWidth(false));
            GUILayout.EndHorizontal();
        }

        GUILayout.EndVertical();
    }
}