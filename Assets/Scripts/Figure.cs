using UnityEngine;

[System.Serializable]
public class Figure
{
    [SerializeField] private Sprite _sprite;
    public Sprite Sprite => _sprite;

    [SerializeField] private Color _color = Color.white;
    public Color Color => _color;
}
