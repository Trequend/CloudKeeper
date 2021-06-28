using Newtonsoft.Json;
using System;

public class DatasetElement
{
    [JsonProperty("Id")] private int _id;
    [JsonIgnore] public int Id => _id;

    [JsonProperty("Pattern")] private string _pattern;
    [JsonIgnore] public string Pattern => _pattern;

    public DatasetElement(int id, string pattern)
    {
        if (string.IsNullOrWhiteSpace(pattern))
        {
            throw new ArgumentNullException(nameof(pattern));
        }

        _id = id;
        _pattern = pattern;
    }
}
