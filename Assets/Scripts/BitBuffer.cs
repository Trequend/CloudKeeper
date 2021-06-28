using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class BitBuffer
{
    public int Width { get; }

    public int Height { get; }

    public RectInt Rect { get; }

    private readonly bool[] _data;

    public BitBuffer(int width, int height)
    {
        if (width <= 0)
        {
            throw new ArgumentException("Width must be greater than 0");
        }

        if (height <= 0)
        {
            throw new ArgumentException("Height must be greater than 0");
        }

        _data = new bool[width * height];
        Width = width;
        Height = height;
        Rect = new RectInt(0, 0, width, height);
    }

    public bool this[int x, int y]
    {
        get => GetValue(x, y);
        set => SetValue(x, y, value);
    }

    public void SetValue(int x, int y, bool value)
    {
        _data[GetIndex(x, y)] = value;
    }

    public bool GetValue(int x, int y)
    {
        return _data[GetIndex(x, y)];
    }

    private int GetIndex(int x, int y)
    {
        return (Height - 1 - y) * Width + x;
    }

    public void LineLoop(Pen pen, List<Vector2Int> points)
    {
        if (points.Count < 2)
        {
            return;
        }

        RectInt pointsBoundingBox = ComputeBoundingBox(points);
        Vector2Int BoundingBoxToBuffer(Vector2Int point)
        {
            point -= pointsBoundingBox.min;
            Vector2 normalized = new Vector2(
                (float)point.x / pointsBoundingBox.size.x,
                (float)point.y / pointsBoundingBox.size.y
            );

            return new Vector2Int(
                Mathf.RoundToInt(normalized.x * (Width - 1)),
                Mathf.RoundToInt(normalized.y * (Height - 1))
            );
        }

        for (int i = 0; i < points.Count - 1; i++)
        {
            pen.DrawLine(
                BoundingBoxToBuffer(points[i]),
                BoundingBoxToBuffer(points[i + 1]),
                SetValue,
                true,
                Rect
            );
        }
    }

    private RectInt ComputeBoundingBox(IEnumerable<Vector2Int> points)
    {
        Vector2Int minPoint = new Vector2Int(int.MaxValue, int.MaxValue);
        Vector2Int maxPoint = new Vector2Int(int.MinValue, int.MinValue);
        foreach (Vector2Int point in points)
        {
            for (int i = 0; i < 2; i++)
            {
                if (point[i] < minPoint[i])
                {
                    minPoint[i] = point[i];
                }

                if (point[i] > maxPoint[i])
                {
                    maxPoint[i] = point[i];
                }
            }
        }

        Vector2Int size = maxPoint - minPoint;
        int maxSize = size.x > size.y ? size.x : size.y;
        Vector2Int center = size / 2 + minPoint;
        size.Set(maxSize, maxSize);
        return new RectInt(center - size / 2, size);
    }

    public void Clear()
    {
        int size = Width * Height;
        for (int i = 0; i < size; i++)
        {
            _data[i] = false;
        }
    }

    public string ToOneLine()
    {
        StringBuilder builder = new StringBuilder();
        for (int y = Height - 1; y >= 0; y--)
        {
            for (int x = 0; x < Width; x++)
            {
                builder.Append(this[x, y] ? "1" : "0");
            }
        }

        return builder.ToString();
    }

    public override string ToString()
    {
        return ToString(new RectInt(0, 0, Width, Height));
    }

    public string ToString(in RectInt region)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append("BitBuffer (");
        if (region.x != 0 || region.y != 0)
        {
            builder.Append("X: ");
            builder.Append(region.x);
            builder.Append("; Y: ");
            builder.Append(region.y);
            builder.Append("; ");
        }

        builder.Append("Size: ");
        builder.Append(Width.ToString());
        builder.Append("x");
        builder.Append(Height.ToString());
        builder.AppendLine(")");
        for (int y = region.height - 1; y >= 0; y--, builder.AppendLine())
        {
            for (int x = 0; x < region.width; x++)
            {
                builder.Append(this[x + region.x, y + region.y] ? "1" : "0");
            }
        }

        return builder.ToString();
    }
}
