using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Pen))]
public class PenEditor : Editor
{
    private SerializedProperty _propThickness;
    private SerializedProperty _propTemplate;

    private Vector2 _scrollPosition = Vector2.zero;

    private Texture2D _texture;

    private void OnEnable()
    {
        _texture = new Texture2D(10, 10, TextureFormat.RGBA32, false)
        {
            hideFlags = HideFlags.DontSave,
        };

        for (int i = 0; i < _texture.height; i++)
        {
            for (int j = 0; j < _texture.width; j++)
            {
                _texture.SetPixel(i, j, Color.white);
            }
        }
        _texture.Apply();

        _propThickness = serializedObject.FindProperty("_thickness");
        _propTemplate = serializedObject.FindProperty("_template");
    }

    private void OnDisable()
    {
        DestroyImmediate(_texture);
        _texture = null;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(_propThickness);
        serializedObject.ApplyModifiedProperties();
        ValidateThickness();
        DrawPenTemplate();
    }

    private void ValidateThickness()
    {
        serializedObject.Update();
        _propThickness.intValue = Pen.ValidateThickness(_propThickness.intValue);
        int templateSize = _propThickness.intValue * _propThickness.intValue;
        if (_propTemplate.arraySize != templateSize)
        {
            _propTemplate.arraySize = templateSize;
            for (int i = 0; i < templateSize; i++)
            {
                _propTemplate.GetArrayElementAtIndex(i).boolValue = true;
            }
        }
        serializedObject.ApplyModifiedPropertiesWithoutUndo();
    }

    private void DrawPenTemplate()
    {
        GUIStyle boldTextSkin = new GUIStyle(GUI.skin.label)
        {
            fontStyle = FontStyle.Bold
        };

        GUIStyle cellSkin = new GUIStyle(GUI.skin.box)
        {
            padding = new RectOffset(5, 5, 5, 5),
            alignment = TextAnchor.MiddleCenter
        };

        serializedObject.Update();
        int thickness = _propThickness.intValue;
        EditorGUILayout.Space();
        GUILayout.Label("Template", boldTextSkin);
        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
        for (int i = 0; i < thickness; i++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int j = 0; j < thickness; j++)
            {
                SerializedProperty element = _propTemplate.GetArrayElementAtIndex(i * thickness + j);
                Texture texture = element.boolValue ? _texture : null;
                if (GUILayout.Button(texture, cellSkin, GUILayout.Width(20.0f), GUILayout.Height(20.0f)))
                {
                    element.boolValue = !element.boolValue;
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();
        serializedObject.ApplyModifiedProperties();
    }
}
