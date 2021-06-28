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

        FillFigureBuffer();
        _points.Clear();
    }

    private void FillFigureBuffer()
    {
        _figureBuffer.LineLoop(_bitBufferPen, _points);
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
