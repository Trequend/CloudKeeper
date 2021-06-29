using System.IO;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

    private string _datasetPath = string.Empty;

    private string[] _figureNames;

    private int[] _countInDataset;

    private int? _selectedFigureIndex;

    private readonly float _drawZonePadding = 10.0f;

    private RectInt _textureRect;

    private Texture2D _texture;

    private Color32[] _clearArray;

    private bool _drawInputCollecting;

    private readonly List<Vector2Int> _points = new List<Vector2Int>();

    private Pen _editorPen;

    private Pen _bufferPen;

    private BitBuffer _figureBuffer = new BitBuffer(32, 32);

    private FigureRecognizer _recognizer;

    private bool _useRecognizer;

    private void Awake()
    {
        _texture = new Texture2D(1, 1)
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
            _recognizer.Destroy();
            _recognizer = null;
        }

        _dataset = null;
        _datasetPath = string.Empty;
        _database = database;
        if (database == null)
        {
            return;
        }

        if (_database.LoadNeuralNetwork() != null)
        {
            _recognizer = new FigureRecognizer(new FigureDatabase[] { _database });
        }

        int count = _database.GetFiguresCount();
        _figureNames = new string[count];
        _countInDataset = new int[count];
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
                if (GUILayout.Button("New", GUILayout.MaxWidth(45.0f)))
                {
                    CreateDataset();
                }
            }
            else
            {
                EditorGUILayout.LabelField(new GUIContent($"Dataset path: {_datasetPath}", _datasetPath));
                if (GUILayout.Button("Save as", GUILayout.MaxWidth(60.0f)))
                {
                    SaveDatasetAs();
                }

                if (GUILayout.Button("Save", GUILayout.MaxWidth(45.0f)))
                {
                    if (string.IsNullOrWhiteSpace(_datasetPath))
                    {
                        SaveDatasetAs();
                    }

                    SaveDataset();
                }
            }

            if (GUILayout.Button("Load", GUILayout.MaxWidth(45.0f)))
            {
                LoadDataset();
            }
        }
    }

    private void CreateDataset()
    {
        string path = EditorUtility.SaveFilePanel("Save dataset", Application.dataPath, "Dataset", "json");
        if (string.IsNullOrWhiteSpace(path))
        {
            return;
        }

        _datasetPath = path;
        SetDataset(new Dataset(_database));
        SaveDataset();
    }

    private void SaveDatasetAs()
    {
        string path = EditorUtility.SaveFilePanel("Save dataset", Application.dataPath, "Dataset", "json");
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
            using StreamWriter writer = File.CreateText(_datasetPath);
            JsonSerializer serializer = new JsonSerializer()
            {
                Formatting = Formatting.Indented
            };
            serializer.Serialize(writer, _dataset);
        }
        catch (Exception exception)
        {
            Debug.LogError($"Failed save. Error: {exception.Message}.");
        }
    }

    private void LoadDataset()
    {
        string path = EditorUtility.OpenFilePanel("Load dataset", Application.dataPath, "json");
        if (string.IsNullOrWhiteSpace(path))
        {
            return;
        }

        try
        {
            using StreamReader reader = File.OpenText(path);
            using JsonTextReader jsonReader = new JsonTextReader(reader);
            JsonSerializer serializer = new JsonSerializer();
            Dataset loadedDataset = serializer.Deserialize<Dataset>(jsonReader);
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
                    SetDataset(Dataset.CopyForDatabase(_database, loadedDataset));
                }
                else
                {
                    return;
                }
            }
            else
            {
                SetDataset(loadedDataset);
            }

            _datasetPath = path;
        }
        catch (Exception exception)
        {
            Debug.LogError($"Failed load. Error: {exception.Message}.");
        }
    }

    private void SetDataset(Dataset dataset)
    {
        _dataset = dataset;
        for (int i = 0; i < _countInDataset.Length; i++)
        {
            _countInDataset[i] = 0;
        }

        foreach (DatasetElement element in _dataset.Elements)
        {
            _countInDataset[element.Id]++;
        }
    }

    private void ToolsEditor()
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

        if (_recognizer == null)
        {
            return;
        }

        _useRecognizer = EditorGUILayout.BeginToggleGroup("Recognizer", _useRecognizer);
        _recognizer.Threshold = EditorGUILayout.FloatField("Threshold", _recognizer.Threshold);
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
                Vector2 size = new Vector2(texture.width, texture.height);
                Rect rect = figure.Sprite.rect;
                Rect textureCoords = new Rect(
                    rect.x / size.x,
                    rect.y / size.y,
                    rect.width / size.x,
                    rect.height / size.y
                );

                
                GUI.DrawTextureWithTexCoords(previewRect, figure.Sprite.texture, textureCoords);
            }
            
            using (new GUILayout.VerticalScope())
            {
                EditorGUILayout.LabelField($"Id: {_selectedFigureIndex.Value}");
                EditorGUILayout.LabelField($"Name: {figure.Name}");
                EditorGUILayout.LabelField($"Count in dataset: {_countInDataset[_selectedFigureIndex.Value]}");
            }
        }
    }

    private void FigureDrawZone()
    {
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
        switch (Event.current.type)
        {
            case EventType.MouseDown:
                StartDrawing();
                break;
            case EventType.MouseUp:
                EndDrawing();
                break;
        }

        if (_drawInputCollecting)
        {
            Vector2 mousePosition = Event.current.mousePosition;
            Vector2Int point = new Vector2Int((int)mousePosition.x, (int)mousePosition.y);
            point.x -= (int)drawZone.position.x;
            point.y -= (int)drawZone.position.y;
            point.y = (int)drawZone.height - point.y;
            if (!_textureRect.Contains(point))
            {
                return;
            }

            if (_points.Count == 0)
            {
                _editorPen.DrawDot(point, _texture.SetPixel, Color.black, _textureRect);
                _points.Add(point);
                _texture.Apply();
                if (Event.current.type != EventType.Repaint)
                {
                    Repaint();
                }
            }
            else if (_points.Last() != point)
            {
                _editorPen.DrawLine(_points.Last(), point, _texture.SetPixel, Color.black, _textureRect);
                _points.Add(point);
                _texture.Apply();
                if (Event.current.type != EventType.Repaint)
                {
                    Repaint();
                }
            }

            
        }
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
        if (Event.current.type != EventType.Repaint)
        {
            Repaint();
        }
    }

    private void ProcessFigure()
    {
        _figureBuffer.LineLoop(_bufferPen, _points);
        if (_useRecognizer)
        {
            Figure figure = _recognizer.Recognize(_figureBuffer, out int figureIndex);
            if (figure != null && figureIndex == _selectedFigureIndex.Value)
            {
                return;
            }
            else
            {
                Debug.Log("New element");
            }
        }

        _countInDataset[_selectedFigureIndex.Value]++;
        _dataset.Elements.Add(new DatasetElement(
            _selectedFigureIndex.Value,
            _figureBuffer.ToOneLine()
        ));
    }
}
