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

        EditorGUILayout.LabelField($"Has neural network: {File.Exists(pathToNeuralNetwork)}");
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
            SerializedProperty propFigure = _propFigures.GetArrayElementAtIndex(i);
            FigureEditor(propFigure, ref i);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void FigureEditor(SerializedProperty propFigure, ref int index)
    {
        SerializedProperty propName = propFigure.FindPropertyRelative("_name");
        SerializedProperty propSprite = propFigure.FindPropertyRelative("_sprite");
        SerializedProperty propColor = propFigure.FindPropertyRelative("_color");
        EditorGUILayout.Space(10.0f);

        using (new EditorGUILayout.HorizontalScope())
        {
            Rect spriteRect = GUILayoutUtility.GetRect(100.0f, 100.0f, GUILayout.Width(100.0f), GUILayout.Height(100.0f));
            Figure figure = null;
            using (new EditorGUILayout.VerticalScope())
            {
                EditorGUILayout.LabelField($"Id: {index}");
                EditorGUILayout.ObjectField(propFigure, new GUIContent("Asset"));
                figure = propFigure.objectReferenceValue as Figure;
                if (figure != null)
                {
                    EditorGUILayout.LabelField($"Name: {figure.Name}");
                }
                
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Remove"))
                {
                    _propFigures.DeleteArrayElementAtIndex(index);
                    index--;
                }
            }

            EditorGUI.DrawRect(spriteRect, Color.gray);
            if (figure != null && figure.Sprite != null)
            {
                Color buffer = GUI.color;
                GUI.color = figure.Color;
                GUI.DrawTextureWithTexCoords(spriteRect, figure.Sprite.texture, figure.Sprite.GetTextureCoords());
                GUI.color = buffer;
            }
        }
    }

    private void UpdateGUID()
    {
        _propId.stringValue = GUID.Generate().ToString();
    }
}
