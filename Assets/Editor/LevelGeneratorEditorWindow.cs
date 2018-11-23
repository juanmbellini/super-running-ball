using Boo.Lang.Runtime;
using UnityEditor;
using UnityEngine;

public class LevelGeneratorEditorWindow : EditorWindow {

    /// <summary>
    /// The level manager to be used.
    /// </summary>
    private LevelManager _levelManager;

    /// <summary>
    /// The selected level to be created.
    /// </summary>
    private int _selectedLevel;


    [MenuItem("Window/Level Generator")]
    private static void InitWindow() {
        CreateInstance<LevelGeneratorEditorWindow>()
            .InitializeWindow();
    }

    private void InitializeWindow() {
        _levelManager = FindObjectOfType<LevelManager>();
        _selectedLevel = 1;
        name = "Level creator";
        position = new Rect(10, 10, 256, 256);
        Show(true);
    }

    private void OnGUI() {
        GUILayout.Label("Parameters", EditorStyles.boldLabel);
        _selectedLevel = EditorGUILayout.IntSlider("Level", _selectedLevel, 1, 10);
        if (GUILayout.Button("Create")) {
            _levelManager.CreateLevel(_selectedLevel);
        }
        if (GUILayout.Button("Clear")) {
            _levelManager.ClearLevel();
        }
    }

    private void OnInspectorUpdate() {
        Repaint();
    }
}