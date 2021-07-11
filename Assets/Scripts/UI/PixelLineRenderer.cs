using System;
using UnityEngine;
using UnityEngine.UI;

public class PixelLineRenderer : Graphic
{
    private Texture2D _texture;

    private RectInt _textureRect;

    public override Texture mainTexture => _texture;

    [SerializeField] private Pen _pen;
    public Pen Pen
    {
        get => _pen;
        set
        {
            if (value == null)
            {
                throw new ArgumentNullException("Value");
            }

            _pen = value;
        }
    }

    private Color32[] _clearArray;

    private Vector2Int? _previousPoint = null;

    protected override void Awake()
    {
        base.Awake();
        if (_pen == null)
        {
            Debug.LogError("No pen");
            return;
        }

        if (_texture == null)
        {
            CreateTexture();
        }
    }

    protected override void OnRectTransformDimensionsChange()
    {
        base.OnRectTransformDimensionsChange();
        if (_texture == null)
        {
            CreateTexture();
        }
        else
        {
            ResizeTexture();
        }
    }

    private void CreateTexture()
    {
        Vector2Int size = ComputeTextureSize();
        _textureRect = new RectInt(0, 0, size.x, size.y);
        _texture = new Texture2D(size.x, size.y, TextureFormat.RGBA32, false)
        {
            filterMode = FilterMode.Point,
            hideFlags = HideFlags.DontSave,
            wrapMode = TextureWrapMode.Clamp
        };

        Clear();
    }

    private void ResizeTexture()
    {
        Vector2Int size = ComputeTextureSize();
        _textureRect = new RectInt(0, 0, size.x, size.y);
        _texture.Resize(size.x, size.y, TextureFormat.RGBA32, false);
        _clearArray = null;
        Clear();
    }

    private Vector2Int ComputeTextureSize()
    {
        Vector2 size = rectTransform.rect.size;
        return new Vector2Int(
            Mathf.RoundToInt(size.x),
            Mathf.RoundToInt(size.y)
        );
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        DestroyImmediate(_texture);
    }

    public void Clear()
    {
        _previousPoint = null;
        int size = _texture.width * _texture.height;
        if (_clearArray == null || _clearArray.Length != size)
        {
            _clearArray = new Color32[size];
            Color32 clearColor = new Color32(0, 0, 0, 0);
            for (int y = 0; y < _texture.height; y++)
            {
                int offset = y * _texture.width;
                for (int x = 0; x < _texture.width; x++)
                {
                    _clearArray[offset + x] = clearColor;
                }
            }
        }

        _texture.SetPixels32(_clearArray);
        _texture.Apply();
    }

    public void AddPoint(int x, int y)
    {
        AddPoint(new Vector2Int(x, y));
    }

    public void AddPoint(Vector2Int point)
    {
        if (_pen == null)
        {
#if UNITY_EDITOR
            Debug.LogWarning("No pen");
#endif
            return;
        }

        point.x = (int)(point.x / canvas.scaleFactor);
        point.y = (int)(point.y / canvas.scaleFactor);

        if (_previousPoint == null)
        {
            _pen.DrawDot(
                point,
                _texture.SetPixel,
                Color.white,
                _textureRect
            );
        }
        else
        {
            _pen.DrawLine(
                _previousPoint.Value,
                point,
                _texture.SetPixel,
                Color.white,
                _textureRect
            );
        }

        _previousPoint = point;
        _texture.Apply();
    }
}
