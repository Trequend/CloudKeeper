using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Pen", menuName = "Tools/Pen")]
public class Pen : ScriptableObject, ISerializationCallbackReceiver
{
    [SerializeField] private int _thickness;

    [SerializeField] private bool[] _template;

    public void DrawLine<T>(
        in Vector2Int a,
        in Vector2Int b,
        Action<int, int, T> setPixel,
        T color,
        in RectInt drawArea
    ) {
        if (a == b)
        {
            DrawDot(a, setPixel, color, drawArea);
            return;
        }

        if (!drawArea.Contains(a))
        {
            Debug.Log(a);
            throw new ArgumentOutOfRangeException(nameof(a));
        }

        if (!drawArea.Contains(b))
        {
            Debug.Log(b);
            throw new ArgumentOutOfRangeException(nameof(b));
        }

        int deltaX = Math.Abs(b.x - a.x);
        int deltaY = Math.Abs(b.y - a.y);
        int stepX = b.x >= a.x ? 1 : -1;
        int stepY = b.y >= a.y ? 1 : -1;
        if (deltaY <= deltaX)
        {
            int d1 = deltaY << 1;
            int d2 = (deltaY - deltaX) << 1;
            int d = (deltaY << 1) - deltaX;
            Vector2Int point = a;
            DrawDot(point, setPixel, color, drawArea);
            point.x += stepX;
            for (int i = 1; i <= deltaX; i++, point.x += stepX)
            {
                if (d > 0)
                {
                    d += d2;
                    point.y += stepY;
                }
                else
                {
                    d += d1;
                }

                DrawDot(point, setPixel, color, drawArea);
            }
        }
        else
        {
            int d1 = deltaX << 1;
            int d2 = (deltaX - deltaY) << 1;
            int d = (deltaX << 1) - deltaY;
            Vector2Int point = a;
            DrawDot(point, setPixel, color, drawArea);
            point.y += stepY;
            for (int i = 1; i <= deltaY; i++, point.y += stepY)
            {
                if (d > 0)
                {
                    d += d2;
                    point.x += stepX;
                }
                else
                {
                    d += d1;
                }

                DrawDot(point, setPixel, color, drawArea);
            }
        }
    }

    public void DrawDot<T>(
        in Vector2Int point,
        Action<int, int, T> setPixel,
        T color,
        in RectInt drawArea
    ) {
        int halfThickness = _thickness / 2;
        Vector2Int minPoint = new Vector2Int(
            Clamp(point.x - halfThickness, drawArea.x, point.x),
            Clamp(point.y - halfThickness, drawArea.y, point.y)
        );

        Vector2Int maxPoint = new Vector2Int(
            Clamp(point.x + halfThickness, point.x, drawArea.x + drawArea.width - 1),
            Clamp(point.y + halfThickness, point.y, drawArea.y + drawArea.height - 1)
        );

        for (int y = minPoint.y; y <= maxPoint.y; y++)
        {
            int templateOffsetY = (_thickness - 1 - (y - point.y + halfThickness)) * _thickness;
            for (int x = minPoint.x; x <= maxPoint.x; x++)
            {
                int templateIndex = templateOffsetY + x - point.x + halfThickness;
                if (_template[templateIndex])
                {
                    setPixel(x, y, color);
                }
            }
        }
    }

    private int Clamp(int x, int min, int max)
    {
        return x < min ? min : (x > max ? max : x);
    }

    public static int ValidateThickness(int thickness)
    {
        if (thickness < 1)
        {
            return 1;
        }

        if (thickness % 2 == 0)
        {
            return thickness - 1;
        }

        return thickness;
    }

    public static bool[] ValidateTemplate(bool[] template, int thickness)
    {
        if (!IsValidTemplate(template, thickness))
        {
            return CreateDefaultTemplate(thickness);
        }

        return template;
    }

    private static bool[] CreateDefaultTemplate(int thickness)
    {
        if (!IsValidThickness(thickness))
        {
            throw new ArgumentException("Invalid thickness");
        }

        int size = thickness * thickness;
        bool[] template = new bool[size];
        for (int i = 0; i < size; i++)
        {
            template[i] = true;
        }

        return template;
    }

    public static bool IsValidThickness(int thickness)
    {
        return thickness >= 1 && thickness % 2 != 0;
    }

    public static bool IsValidTemplate(bool[] template, int thickness)
    {
        return template != null && template.Length == thickness * thickness;
    }

    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
        // Nothing to do
    }

    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
        _thickness = ValidateThickness(_thickness);
        _template = ValidateTemplate(_template, _thickness);
    }
}
