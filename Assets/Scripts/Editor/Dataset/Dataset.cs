using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dataset
{
    [SerializeField] private string _databaseId;
    public string DatabaseId => _databaseId;

    [SerializeField] private List<DatasetElement> _elements = new List<DatasetElement>();
    public List<DatasetElement> Elements => _elements;

    public Dataset(FigureDatabase database)
    {
        _databaseId = database.Id;
    }
}
