using System;
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

    public void Clear()
    {
        int size = Width * Height;
        for (int i = 0; i < size; i++)
        {
            _data[i] = false;
        }
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
