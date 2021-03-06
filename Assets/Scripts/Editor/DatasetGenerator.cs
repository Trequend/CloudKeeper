using System.IO;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

public class DatasetGenerator : EditorWindow
{
    [MenuItem("Dataset generator", menuItem = "Neural network/Dataset generator")]
    public static void CreateWindow()
    {
        GetWindow<DatasetGenerator>("Dataset generator");
    }

    private readonly Color32 _clearColor = new Color32(204, 255, 254, 255);

    private FigureDatabase _database;

    private Dataset _dataset;

    private string _datasetPath;

    private bool DatasetHasUnsavedChanged
    {
        get => hasUnsavedChanges;
        set => hasUnsavedChanges = value;
    }

    private bool _datasetHasIrrevocableChanges;

    private string[] _figureNames;

    private int? _selectedFigureIndex;

    private readonly float _drawZonePadding = 10.0f;

    private readonly float _drawZoneThreshold = 5.0f;

    private RectInt _textureRect;

    private Texture2D _texture;

    private Color32[] _clearArray;

    private bool _drawInputCollecting;

    private readonly List<Vector2Int> _points = new List<Vector2Int>();

    private Pen _editorPen;

    private Pen _bufferPen;

    private readonly BitBuffer _figureBuffer = new BitBuffer(32, 32);

    private readonly float _toolsMaxHeight = 165.0f;

    private Vector2 _toolsScrollPosition;

    private FigureRecognizer _recognizer;

    private bool _useRecognizer;

    private bool _useFigureRotator;

    private float[] _angles = new float[] { 15.0f, -15.0f };

    private readonly Stack<int> _figureAddingHistory = new Stack<int>();

    private int _figureAddedAfterSave;

    private void Awake()
    {
        _texture = new Texture2D(1, 1, TextureFormat.RGBA32, false)
        {
            filterMode = FilterMode.Point,
            hideFlags = HideFlags.HideAndDontSave,
            wrapMode = TextureWrapMode.Clamp
        };

        _textureRect = new RectInt(0, 0, 1, 1);
    }

    private void OnDestroy()
    {
        DestroyImmediate(_texture);
        if (_recognizer != null)
        {
            _recognizer.Dispose();
        }
    }

    private void OnGUI()
    {
        DatabaseEditor();
        if (_database != null)
        {
            DatasetEditor();
        }

        if (_dataset != null)
        {
            ToolsEditor();
            FigureSelector();
            if (_selectedFigureIndex == null)
            {
                EditorGUILayout.LabelField("No selected figure");
                return;
            }

            FigureInformation();
            FigureDrawZone();
        }
    }

    public override void SaveChanges()
    {
        base.SaveChanges();
        SaveDataset();
    }

    private void DatabaseEditor()
    {
        FigureDatabase database = (FigureDatabase)EditorGUILayout.ObjectField(
            "Figure database",
            _database,
            typeof(FigureDatabase),
            allowSceneObjects: false
        );

        if (database != _database)
        {
            SetDatabase(database);
        }
    }

    private void SetDatabase(FigureDatabase database)
    {
        _useRecognizer = false;
        if (_database != null && _recognizer != null)
        {
            _recognizer.Dispose();
            _recognizer = null;
        }

        SetDataset(dataset: null, path: null);
        _database = database;
        if (database == null)
        {
            return;
        }

        if (_database.LoadNeuralNetwork() != null)
        {
            _recognizer = new FigureRecognizer(_database);
        }

        int count = _database.GetFiguresCount();
        _figureNames = new string[count];
        for (int i = 0; i < count; i++)
        {
            Figure figure = _database.GetFigure(i);
            _figureNames[i] = $"{figure.Name} (id: {i})";
        }

        if (count == 0)
        {
            _selectedFigureIndex = null;
        }
        else
        {
            _selectedFigureIndex = 0;
        }
    }

    private void DatasetEditor()
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            if (string.IsNullOrWhiteSpace(_datasetPath))
            {
                EditorGUILayout.LabelField("No dataset");
            }
            else
            {
                EditorGUILayout.PrefixLabel("Dataset path");
                EditorGUILayout.LabelField(new GUIContent(_datasetPath, _datasetPath));

                if (GUILayout.Button("Save as", GUILayout.MaxWidth(60.0f)))
                {
                    SaveDatasetAs();
                }

                using (new EditorGUI.DisabledGroupScope(!DatasetHasUnsavedChanged))
                {
                    if (GUILayout.Button("Save", GUILayout.MaxWidth(45.0f)))
                    {
                        if (string.IsNullOrWhiteSpace(_datasetPath))
                        {
                            SaveDatasetAs();
                        }

                        SaveDataset();
                    }
                }
            }

            if (GUILayout.Button("New", GUILayout.MaxWidth(45.0f)))
            {
                CreateDataset();
            }

            if (GUILayout.Button("Load", GUILayout.MaxWidth(45.0f)))
            {
                LoadDataset();
            }
        }
    }

    private void CreateDataset()
    {
        string path = EditorUtility.SaveFilePanel("Save dataset", Application.dataPath, "Dataset", "dataset");
        if (string.IsNullOrWhiteSpace(path))
        {
            return;
        }

        SetDataset(new Dataset(_database), path);
        SaveDataset();
    }

    private void SaveDatasetAs()
    {
        string path = EditorUtility.SaveFilePanel("Save dataset", Application.dataPath, "Dataset", "dataset");
        if (string.IsNullOrWhiteSpace(path))
        {
            return;
        }

        _datasetPath = path;
        SaveDataset();
    }

    private void SaveDataset()
    {
        try
        {
            Dataset.SaveToFile(_dataset, _datasetPath);
            _figureAddedAfterSave = 0;
            DatasetHasUnsavedChanged = false;
            _datasetHasIrrevocableChanges = false;
        }
        catch (Exception exception)
        {
            Debug.LogError($"Failed save. Error: {exception.Message}.");
        }
    }

    private void LoadDataset()
    {
        string path = EditorUtility.OpenFilePanel("Load dataset", Application.dataPath, "dataset");
        if (string.IsNullOrWhiteSpace(path))
        {
            return;
        }

        try
        {
            Dataset loadedDataset = Dataset.LoadFromFile(path);
            if (loadedDataset.DatabaseId != _database.Id)
            {
                bool ok = EditorUtility.DisplayDialog(
                    "Wrong id",
                    "The dataset contains an identifier for another database. Change dataset id?",
                    "Change",
                    "Cancel"
                );

                if (ok)
                {
                    SetDataset(Dataset.CopyForDatabase(_database, loadedDataset), path);
                }
                else
                {
                    return;
                }
            }
            else
            {
                SetDataset(loadedDataset, path);
            }
        }
        catch (Exception exception)
        {
            Debug.LogError($"Failed load. Error: {exception.Message}.");
        }
    }

    private void SetDataset(Dataset dataset, string path)
    {
        _dataset = dataset;
        _datasetPath = path;
        _figureAddedAfterSave = 0;
        DatasetHasUnsavedChanged = false;
        _datasetHasIrrevocableChanges = false;
        _figureAddingHistory.Clear();
    }

    private void ToolsEditor()
    {
        EditorGUILayout.LabelField("Tools");
        float scrollHeight = Mathf.Clamp(0.3f * position.height, 0.0f, _toolsMaxHeight);
        var scrollView = new EditorGUILayout.ScrollViewScope(_toolsScrollPosition, GUILayout.Height(scrollHeight));
        using (scrollView)
        {
            _toolsScrollPosition = scrollView.scrollPosition;
            PensEditor();
            RecognizerEditor();
            FigureRotatorEditor();
        }
    }

    private void PensEditor()
    {
        _editorPen = EditorGUILayout.ObjectField(
            new GUIContent("Editor pen"),
            _editorPen,
            typeof(Pen),
            allowSceneObjects: false
        ) as Pen;

        _bufferPen = EditorGUILayout.ObjectField(
            new GUIContent("Dataset pen"),
            _bufferPen,
            typeof(Pen),
            allowSceneObjects: false
        ) as Pen;
    }

    private void RecognizerEditor()
    {
        if (_recognizer == null)
        {
            return;
        }

        _useRecognizer = EditorGUILayout.BeginToggleGroup("Recognizer", _useRecognizer);
        EditorGUI.indentLevel++;

        _recognizer.Threshold = EditorGUILayout.FloatField("Threshold", _recognizer.Threshold);

        EditorGUI.indentLevel--;
        EditorGUILayout.EndToggleGroup();
    }

    private void FigureRotatorEditor()
    {
        _useFigureRotator = EditorGUILayout.BeginToggleGroup("Figure rotator", _useFigureRotator);
        EditorGUI.indentLevel++;
        
        int length = EditorGUILayout.IntField("Angles count", _angles.Length);
        if (length != _angles.Length)
        {
            _angles = new float[length];
        }

        for (int i = 0; i < length; i++)
        {
            _angles[i] = EditorGUILayout.FloatField($"Angle {i}", _angles[i]);
        }

        EditorGUI.indentLevel--;
        EditorGUILayout.EndToggleGroup();
    }

    private void FigureSelector()
    {
        if (_figureNames.Length == 0)
        {
            EditorGUILayout.LabelField("No figures in database");
            return;
        }

        using (new GUILayout.HorizontalScope())
        {
            EditorGUILayout.PrefixLabel("Figure");
            _selectedFigureIndex = EditorGUILayout.Popup(_selectedFigureIndex.Value, _figureNames);
        }
    }

    private void FigureInformation()
    {
        using (new GUILayout.HorizontalScope())
        {
            Figure figure = _database.GetFigure(_selectedFigureIndex.Value);

            Rect padding = GUILayoutUtility.GetRect(5.0f, 100.0f, GUILayout.Width(5.0f), GUILayout.Height(100.0f));
            Rect previewRect = GUILayoutUtility.GetRect(
                100.0f,
                100.0f,
                GUILayout.Width(100.0f),
                GUILayout.Height(100.0f)
            );

            EditorGUI.DrawRect(previewRect, Color.gray);
            if (figure.Sprite != null)
            {
                Texture texture = figure.Sprite.texture;
                Rect textureCoords = figure.Sprite.GetTextureCoords();
                GUI.DrawTextureWithTexCoords(previewRect, figure.Sprite.texture, textureCoords);
            }
            
            using (new GUILayout.VerticalScope(GUILayout.Height(100.0f)))
            {
                EditorGUILayout.LabelField($"Id: {_selectedFigureIndex.Value}");
                EditorGUILayout.LabelField($"Name: {figure.Name}");

                int countInDataset = _dataset.Elements.Count(element => element.Id == _selectedFigureIndex.Value);
                EditorGUILayout.LabelField($"Count in dataset: {countInDataset}");
                
                GUILayout.FlexibleSpace();

                EditorGUI.BeginDisabledGroup(_figureAddingHistory.Count == 0);

                if (GUILayout.Button("Undo figure adding"))
                {
                    UndoFigureAdding();
                }

                EditorGUI.EndDisabledGroup();
            }
        }
    }

    private void UndoFigureAdding()
    {
        if (_figureAddingHistory.Count == 0)
        {
            return;
        }

        if (_figureAddedAfterSave == 0)
        {
            DatasetHasUnsavedChanged = true;
            _datasetHasIrrevocableChanges = true;
        }
        else
        {
            _figureAddedAfterSave--;
            if (!_datasetHasIrrevocableChanges)
            {
                DatasetHasUnsavedChanged = _figureAddedAfterSave != 0;
            }
        }

        int elementCount = _figureAddingHistory.Pop();
        for (int i = 0; i < elementCount; i++)
        {
            _dataset.Elements.RemoveAt(_dataset.Elements.Count - 1);
        }
    }

    private void FigureDrawZone()
    {
        if (_editorPen == null || _bufferPen == null)
        {
            EditorGUILayout.LabelField("No pen");
            return;
        }

        Rect lastRect = GUILayoutUtility.GetLastRect();

        Vector2 offset = new Vector2(
            0.0f,
            lastRect.y + lastRect.height + _drawZonePadding
        );

        Rect zoneRect = new Rect(
            offset.x,
            offset.y,
            position.width,
            position.height - offset.y
        );

        if (zoneRect.height <= _drawZoneThreshold)
        {
            return;
        }

        if (Event.current.type == EventType.Repaint)
        {
            if (_texture.width != (int)zoneRect.width || _texture.height != (int)zoneRect.height)
            {
                ResizeTexture((int)zoneRect.width, (int)zoneRect.height);
            }
        }

        CollectDrawInput(zoneRect);
        GUI.DrawTexture(zoneRect, _texture);
    }

    private void ResizeTexture(int width, int height)
    {
        _textureRect = new RectInt(0, 0, width, height);
        _texture.Resize(width, height);
        ClearTexture();
    }

    private void ClearTexture()
    {
        int size = _texture.width * _texture.height;
        if (_clearArray == null || _clearArray.Length != size)
        {
            _clearArray = new Color32[size];
            for (int y = 0; y < _texture.height; y++)
            {
                int offset = y * _texture.width;
                for (int x = 0; x < _texture.width; x++)
                {
                    _clearArray[offset + x] = _clearColor;
                }
            }
        }

        _texture.SetPixels32(_clearArray);
        _texture.Apply();
    }

    private void CollectDrawInput(in Rect drawZone)
    {
        Vector2Int point = MousePositionToTexture(drawZone);
        switch (Event.current.type)
        {
            case EventType.MouseDown:
                if (_textureRect.Contains(point))
                {
                    StartDrawing();
                }
                break;
            case EventType.MouseUp:
                if (_drawInputCollecting)
                {
                    EndDrawing();
                }
                break;
        }

        if (_drawInputCollecting)
        {
            if (!_textureRect.Contains(point))
            {
                return;
            }

            if (_points.Count == 0)
            {
                _editorPen.DrawDot(point, _texture.SetPixel, Color.black, _textureRect);
            }
            else if (_points.Last() == point)
            {
                return;
            }
            else
            {
                _editorPen.DrawLine(_points.Last(), point, _texture.SetPixel, Color.black, _textureRect);
            }

            _points.Add(point);
            _texture.Apply();
            if (Event.current.type != EventType.Repaint)
            {
                Repaint();
            }
        }
    }

    private Vector2Int MousePositionToTexture(in Rect drawZone)
    {
        Vector2 mousePosition = Event.current.mousePosition;
        Vector2Int point = new Vector2Int((int)mousePosition.x, (int)mousePosition.y);
        point.x -= (int)drawZone.position.x;
        point.y -= (int)drawZone.position.y;
        point.y = (int)drawZone.height - point.y;
        return point;
    }

    private void StartDrawing()
    {
        _drawInputCollecting = true;
    }

    private void EndDrawing()
    {
        _drawInputCollecting = false;
        ProcessFigure();
        _points.Clear();
        _figureBuffer.Clear();
        ClearTexture();
        Repaint();
    }

    private void ProcessFigure()
    {
        if (_points.Count < 2)
        {
            return;
        }

        _figureBuffer.LineLoop(_bufferPen, _points);
        if (_useRecognizer)
        {
            Figure figure = _recognizer.Recognize(_figureBuffer, out int figureIndex);
            if (figure != null && figureIndex == _selectedFigureIndex.Value)
            {
                return;
            }
        }

        int elementCount = 1;
        AddFigureInDataset();
        if (_useFigureRotator && _angles.Length != 0)
        {
            foreach (List<Vector2Int> rotatedPoints in FigureRotator.Rotations(_points, _angles))
            {
                _figureBuffer.Clear();
                _figureBuffer.LineLoop(_bufferPen, rotatedPoints);
                AddFigureInDataset();
                elementCount++;
            }
        }

        _figureAddingHistory.Push(elementCount);
    }

    private void AddFigureInDataset()
    {
        DatasetHasUnsavedChanged = true;
        _figureAddedAfterSave++;
        
        _dataset.Elements.Add(new DatasetElement(
            _selectedFigureIndex.Value,
            _figureBuffer.ToOneLine()
        ));
    }
}
