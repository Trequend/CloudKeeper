using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class Dataset
{
    [SerializeField] private string _databaseId;
    [JsonIgnore] public string DatabaseId => _databaseId;

    [SerializeField] private List<DatasetElement> _elements = new List<DatasetElement>();
    [JsonIgnore] public List<DatasetElement> Elements => _elements;

    private Dataset(string databaseId)
    {
        _databaseId = databaseId;
    }

    public Dataset(FigureDatabase database)
    {
        _databaseId = database.Id;
    }

    public static Dataset CopyForDatabase(FigureDatabase database, Dataset dataset)
    {
        Dataset copy = new Dataset(database);
        foreach (DatasetElement element in dataset.Elements)
        {
            copy.Elements.Add(element);
        }

        return copy;
    }

    public static void SaveToFile(Dataset dataset, string filename)
    {
        using StreamWriter writer = File.CreateText(filename);
        writer.WriteLine(dataset.DatabaseId);
        foreach (DatasetElement element in dataset.Elements)
        {
            writer.WriteLine($"{element.Id} {element.Pattern}");
        }
    }

    public static Dataset LoadFromFile(string filename)
    {
        using StreamReader reader = File.OpenText(filename);
        string databaseId = reader.ReadLine();
        if (string.IsNullOrWhiteSpace(databaseId))
        {
            throw new FileLoadException($"No id in \"{filename}\"", filename);
        }

        Dataset dataset = new Dataset(databaseId);
        string[] dataSeparator = new string[] { " " };
        while (!reader.EndOfStream)
        {
            string line = reader.ReadLine();
            string[] tokens = line.Split(dataSeparator, System.StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length != 2)
            {
                throw new FileLoadException(
                    $"Wrong entry in \"{filename}\". Entry index: {dataset.Elements.Count}. Expected: id pattern",
                    filename
                );
            }

            if (!int.TryParse(tokens[0], out int id))
            {
                throw new FileLoadException(
                    $"Wrong entry in \"{filename}\". Entry index: {dataset.Elements.Count}. Error: id must be a number",
                    filename
                );
            }

            if (tokens[1].Length != 32 * 32)
            {
                throw new FileLoadException(
                    $"Wrong entry in \"{filename}\". Entry index: {dataset.Elements.Count}. Error: wrong pattern size",
                    filename
                );
            }

            string pattern = tokens[1];
            dataset.Elements.Add(new DatasetElement(id, pattern));
        }

        return dataset;
    }
}
