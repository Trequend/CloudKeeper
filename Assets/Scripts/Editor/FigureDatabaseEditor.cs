using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FigureDatabase))]
public class FigureDatabaseEditor : Editor
{
    private SerializedProperty _id;
    private SerializedProperty _propFigures;

    private void OnEnable()
    {
        _id = serializedObject.FindProperty("_id");
        if (string.IsNullOrWhiteSpace(_id.stringValue))
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
        if (GUILayout.Button($"Database id - {_id.stringValue}", GUI.skin.label))
        {
            Debug.Log("Database id copied in clipboard");
            GUIUtility.systemCopyBuffer = _id.stringValue;
        }

        string pathToNeuralNetwork = Path.Combine(Application.dataPath, "Resources", "NeuralNetworks", $"{_id.stringValue}.onnx");
        EditorGUILayout.LabelField($"Have neural network: {File.Exists(pathToNeuralNetwork)}");
        EditorGUILayout.LabelField($"Figures count - {_propFigures.arraySize}");
    }

    private void DatabaseEditor()
    {
        serializedObject.Update();
        if (GUILayout.Button("Add"))
        {
            _propFigures.InsertArrayElementAtIndex(_propFigures.arraySize);
            UpdateGUID();
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
        SerializedProperty propSprite = figure.FindPropertyRelative("_sprite");
        SerializedProperty propColor = figure.FindPropertyRelative("_color");
        EditorGUILayout.Space(10.0f);

        using (new EditorGUILayout.HorizontalScope())
        {
            propSprite.objectReferenceValue = EditorGUI.ObjectField(
                GUILayoutUtility.GetRect(80, 80, GUILayout.Width(80.0f), GUILayout.Height(80.0f)),
                propSprite.objectReferenceValue,
                typeof(Sprite),
                allowSceneObjects: false
            );
            using (new EditorGUILayout.VerticalScope())
            {
                EditorGUILayout.LabelField($"Id: {index}");
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField("Color");
                    propColor.colorValue = EditorGUILayout.ColorField(propColor.colorValue);
                }

                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Remove"))
                {
                    _propFigures.DeleteArrayElementAtIndex(index);
                    index--;
                    UpdateGUID();
                }
            }
        }
    }

    private void UpdateGUID()
    {
        _id.stringValue = GUID.Generate().ToString();
    }
}
