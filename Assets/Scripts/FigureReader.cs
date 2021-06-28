using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class FigureReader : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private PixelLineRenderer _renderer;
    [SerializeField] private Pen _bitBufferPen;

    private readonly BitBuffer _figureBuffer = new BitBuffer(32, 32);

    public delegate void FigureReadedEventHandler(BitBuffer figure);

    public event FigureReadedEventHandler _figureReaded;
    public event FigureReadedEventHandler FigureReaded
    {
        add => _figureReaded += value;
        remove => _figureReaded -= value;
    }

    private readonly List<Vector2Int> _points = new List<Vector2Int>();

    private Coroutine _reading;

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        if (_reading == null)
        {
            StartReading();
        }
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
#if !UNITY_EDITOR
        if (Input.touches[0].phase != TouchPhase.Ended)
        {
            return;
        }
#endif
        if (_reading != null)
        {
            EndReading();
        }
    }

    private void StartReading()
    {
        _reading = StartCoroutine(Read());
    }

    private void EndReading()
    {
        StopCoroutine(_reading);
        _reading = null;
        _renderer.Clear();
        ProcessPoints();
        System.IO.File.WriteAllText("test.txt", _figureBuffer.ToString());
        _figureBuffer.Clear();
        // Recoginze figure
    }

    private void ProcessPoints()
    {
        if (_bitBufferPen == null)
        {
            _points.Clear();
#if UNITY_EDITOR
            Debug.LogWarning("No bit buffer pen");
#endif
            return;
        }

        if (_points.Count < 2)
        {
            _points.Clear();
            return;
        }

        Vector2Int minPoint = new Vector2Int(int.MaxValue, int.MaxValue);
        Vector2Int maxPoint = new Vector2Int(int.MinValue, int.MinValue);
        foreach (Vector2Int point in _points)
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
        FillFigureBuffer(new RectInt(center - size / 2, size));
        _points.Clear();
    }

    private void FillFigureBuffer(RectInt pointsBoundingBox)
    {
        Vector2Int ScreenToFigureBuffer(Vector2Int point)
        {
            point -= pointsBoundingBox.min;
            Vector2 normalized = new Vector2(
                (float)point.x / pointsBoundingBox.size.x,
                (float)point.y / pointsBoundingBox.size.y
            );

            return new Vector2Int(
                Mathf.RoundToInt(normalized.x * (_figureBuffer.Width - 1)),
                Mathf.RoundToInt(normalized.y * (_figureBuffer.Height - 1))
            );
        }

        for (int i = 0; i < _points.Count - 1; i++)
        {
            _bitBufferPen.DrawLine(
                ScreenToFigureBuffer(_points[i]),
                ScreenToFigureBuffer(_points[i + 1]),
                _figureBuffer.SetValue,
                true,
                _figureBuffer.Rect
            );
        }
    }

    private IEnumerator Read()
    {
        while (true)
        {
#if UNITY_EDITOR
            Vector3 inputPosition = Input.mousePosition;
            if (!PositionInScreen(inputPosition))
            {
                yield return null;
            }
#else
            Vector3 inputPosition = Input.touches[0].position;
#endif
            Vector2Int position = new Vector2Int(
                Mathf.RoundToInt(inputPosition.x),
                Mathf.RoundToInt(inputPosition.y)
            );

            if (_points.Count == 0 || _points.Last() != position)
            {
                _renderer.AddPoint(position);
                _points.Add(position);
            }
            
            yield return null;
        }
    }

#if UNITY_EDITOR
    private bool PositionInScreen(in Vector3 position)
    {
        return position.x > 0 && position.x < Screen.width
            && position.y > 0 && position.y < Screen.height;
    }
#endif
}
