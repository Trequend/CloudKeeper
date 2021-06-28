using System.IO;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;

public class DatasetGenerator : EditorWindow
{
    [MenuItem("Dataset generator", menuItem = "Neural network/Dataset generator")]
    public static void CreateWindow()
    {
        GetWindow<DatasetGenerator>("Dataset generator");
    }

    private FigureDatabase _database;

    private Dataset _dataset;
    private string _datasetPath = string.Empty;

    private void OnGUI()
    {
        FigureDatabase database = (FigureDatabase)EditorGUILayout.ObjectField("Figure database", _database, typeof(FigureDatabase), allowSceneObjects: false);
        if (database != _database)
        {
            _database = database;
            if (database == null)
            {
                _dataset = null;
                _datasetPath = string.Empty;
            }
            else
            {
                _dataset = new Dataset(database);
                _dataset.Elements.Add(new DatasetElement(0, "1100101010101001010101010"));
                _datasetPath = string.Empty;
            }
        }

        if (_database != null)
        {
            DatasetInfo();
        }
    }

    private void DatasetInfo()
    {
        using (new EditorGUILayout.HorizontalScope())
        {

            if (string.IsNullOrWhiteSpace(_datasetPath))
            {
                EditorGUILayout.LabelField("Dataset not saved");
            }
            else
            {
                EditorGUILayout.LabelField($"Dataset path: {_datasetPath}");
                if (GUILayout.Button("Save as"))
                {
                    SaveAs();
                }
            }

            if (GUILayout.Button("Save"))
            {
                if (string.IsNullOrWhiteSpace(_datasetPath))
                {
                    SaveAs();
                }

                SaveDataset();
            }
        }
    }

    private void SaveAs()
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
        File.WriteAllText(_datasetPath, JsonConvert.SerializeObject(_dataset, Formatting.Indented));
    }
}
