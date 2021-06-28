using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dataset
{
    [SerializeField] [JsonProperty("DatabaseId")] private string _databaseId;
    [JsonIgnore] public string DatabaseId => _databaseId;

    [SerializeField] [JsonProperty("Elements")] private List<DatasetElement> _elements = new List<DatasetElement>();
    [JsonIgnore] public List<DatasetElement> Elements => _elements;

    [JsonConstructor]
    private Dataset()
    {
        // For deserialization
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
}
