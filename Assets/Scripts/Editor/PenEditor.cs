using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Pen))]
public class PenEditor : Editor
{
    private SerializedProperty _propThickness;

    private SerializedProperty _propTemplate;

    private Vector2 _scrollPosition = Vector2.zero;

    private Color _enabledCellColor = Color.gray;

    private Color _disabledCellColor = new Color(0.3f, 0.3f, 0.3f);

    private void OnEnable()
    {
        _propThickness = serializedObject.FindProperty("_thickness");
        _propTemplate = serializedObject.FindProperty("_template");
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

        serializedObject.Update();
        EditorGUILayout.Space();
        GUILayout.Label("Template", boldTextSkin);
        using (var scroll = new EditorGUILayout.ScrollViewScope(_scrollPosition))
        {
            _scrollPosition = scroll.scrollPosition;
            TemplateEditor();
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void TemplateEditor()
    {
        GUIStyle cellSkin = new GUIStyle(GUI.skin.box)
        {
            padding = new RectOffset(5, 5, 5, 5),
            alignment = TextAnchor.MiddleCenter
        };

        int thickness = _propThickness.intValue;
        for (int i = 0; i < thickness; i++)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                Color buffer = GUI.color;
                for (int j = 0; j < thickness; j++)
                {
                    SerializedProperty element = _propTemplate.GetArrayElementAtIndex(i * thickness + j);
                    if (GUILayout.Button(string.Empty, cellSkin, GUILayout.Width(20.0f), GUILayout.Height(20.0f)))
                    {
                        element.boolValue = !element.boolValue;
                    }

                    Color color = element.boolValue ? _enabledCellColor : _disabledCellColor;
                    EditorGUI.DrawRect(GUILayoutUtility.GetLastRect(), color);
                }

                GUI.color = buffer;
            }
        }
    }
}
