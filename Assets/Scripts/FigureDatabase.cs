using UnityEngine;

[CreateAssetMenu(fileName = "FigureDatabase", menuName = "Databases/FigureDatabase")]
public class FigureDatabase : ScriptableObject
{
    [SerializeField] private string _id;
    public string Id => _id;

    [SerializeField] private Figure[] _figures;

    public int GetFiguresCount()
    {
        return _figures.Length;
    }

    public Figure GetFigure(int index)
    {
        return _figures[index];
    }
}
