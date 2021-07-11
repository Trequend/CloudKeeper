using UnityEngine;

[CreateAssetMenu(fileName = "Figures", menuName = "Figures/Figure")]
public class Figure : ScriptableObject
{
    [SerializeField] private string _name;
    public string Name => _name;

    [SerializeField] private Sprite _sprite;
    public Sprite Sprite => _sprite;

    [SerializeField] private Color _color = Color.white;
    public Color Color => _color;
}
