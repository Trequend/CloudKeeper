using System;

public class DatasetElement
{
    private int _id;
    public int Id => _id;

    private string _pattern;
    public string Pattern => _pattern;

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
