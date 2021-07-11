using UnityEngine;

public static class SpriteExtensions
{
    public static Rect GetTextureCoords(this Sprite sprite)
    {
        Texture texture = sprite.texture;
        Vector2 size = new Vector2(texture.width, texture.height);
        Rect rect = sprite.rect;
        Rect textureCoords = new Rect(
            rect.x / size.x,
            rect.y / size.y,
            rect.width / size.x,
            rect.height / size.y
        );

        return textureCoords;
    }
}
