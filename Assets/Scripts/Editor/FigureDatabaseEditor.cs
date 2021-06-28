using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FigureDatabase))]
public class FigureDatabaseEditor : Editor
{
    private SerializedProperty _propId;

    private SerializedProperty _propFigures;

    private void OnEnable()
    {
        _propId = serializedObject.FindProperty("_id");
        if (string.IsNullOrWhiteSpace(_propId.stringValue))
        {
            serializedObject.Update();
            UpdateGUID();
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        _propFigures = serializedObject.FindProperty("_figures");
    }

    public override void OnInspectorGUI()
    {
        GeneralInfo();
        DatabaseEditor();
    }

    private void GeneralInfo()
    {
        if (GUILayout.Button($"Database id - {_propId.stringValue}", GUI.skin.label))
        {
            GUIUtility.systemCopyBuffer = _propId.stringValue;
        }

        string pathToNeuralNetwork = Path.Combine(
            Application.dataPath,
            "Resources",
            "NeuralNetworks",
            $"{_propId.stringValue}.onnx"
        );

        EditorGUILayout.LabelField($"Have neural network: {File.Exists(pathToNeuralNetwork)}");
        EditorGUILayout.LabelField($"Figures count - {_propFigures.arraySize}");
    }

    private void DatabaseEditor()
    {
        serializedObject.Update();
        if (GUILayout.Button("Update id"))
        {
            UpdateGUID();
        }

        if (GUILayout.Button("Add"))
        {
            _propFigures.InsertArrayElementAtIndex(_propFigures.arraySize);
        }

        for (int i = 0; i < _propFigures.arraySize; i++)
        {
            SerializedProperty figure = _propFigures.GetArrayElementAtIndex(i);
            FigureEditor(figure, ref i);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void FigureEditor(SerializedProperty figure, ref int index)
    {
        SerializedProperty propName = figure.FindPropertyRelative("_name");
        SerializedProperty propSprite = figure.FindPropertyRelative("_sprite");
        SerializedProperty propColor = figure.FindPropertyRelative("_color");
        EditorGUILayout.Space(10.0f);

        using (new EditorGUILayout.HorizontalScope())
        {
            propSprite.objectReferenceValue = EditorGUI.ObjectField(
                GUILayoutUtility.GetRect(100.0f, 100.0f, GUILayout.Width(100.0f), GUILayout.Height(100.0f)),
                propSprite.objectReferenceValue,
                typeof(Sprite),
                allowSceneObjects: false
            );

            using (new EditorGUILayout.VerticalScope())
            {
                EditorGUILayout.LabelField($"Id: {index}");
                EditorGUILayout.PropertyField(propName, new GUIContent("Name"));
                EditorGUILayout.PropertyField(propColor, new GUIContent("Color"));
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Remove"))
                {
                    _propFigures.DeleteArrayElementAtIndex(index);
                    index--;
                }
            }
        }
    }

    private void UpdateGUID()
    {
        _propId.stringValue = GUID.Generate().ToString();
    }
}
