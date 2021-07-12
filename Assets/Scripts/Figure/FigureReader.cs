using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class FigureReader : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private PixelLineRenderer _renderer;

    [SerializeField] private Pen _bitBufferPen;

    [SerializeField] private FigureDatabase _figureDatabase;

    [SerializeField] [Min(0.1f)] private float _cleanupDelay = 0.3f;

    private readonly BitBuffer _figureBuffer = new BitBuffer(32, 32);

    private FigureRecognizer _recognizer;

    public delegate void FigureReadedEventHandler(Figure figure);

    public event FigureReadedEventHandler _figureReaded;
    public event FigureReadedEventHandler FigureReaded
    {
        add => _figureReaded += value;
        remove => _figureReaded -= value;
    }

    private readonly List<Vector2Int> _points = new List<Vector2Int>();

    private Coroutine _reading;

    private Coroutine _cleanupRenderer;

    private void Start()
    {
        _recognizer = new FigureRecognizer(_figureDatabase);

        // The first recognition is slow. Do it at the beginning
        _recognizer.Recognize(_figureBuffer);
    }

    private void OnDestroy()
    {
        _recognizer.Dispose();
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        if (_reading == null)
        {
            StartReading();
        }
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
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
        if (_cleanupRenderer != null)
        {
            StopCoroutine(_cleanupRenderer);
            _cleanupRenderer = null;
            _renderer.Clear();
        }

        _renderer.color = Color.white;
        _reading = StartCoroutine(Read());
    }

    private IEnumerator Read()
    {
        while (true)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            Vector3 inputPosition = Input.touches[0].position;
#else
            Vector3 inputPosition = Input.mousePosition;
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

    private void EndReading()
    {
        StopCoroutine(_reading);
        _reading = null;
        ProcessPoints();
        Figure figure = _recognizer.Recognize(_figureBuffer);
        _figureReaded?.Invoke(figure);
        if (figure == null)
        {
            _renderer.color = Color.red;
        }
        else
        {
            _renderer.color = figure.Color;
        }

        _figureBuffer.Clear();
        _cleanupRenderer = StartCoroutine(CleanupRender());
    }

    private void ProcessPoints()
    {
        if (_bitBufferPen == null)
        {
            _points.Clear();
            Debug.LogWarning("No bit buffer pen");
            return;
        }

        FillFigureBuffer();
        _points.Clear();
    }

    private void FillFigureBuffer()
    {
        _figureBuffer.LineLoop(_bitBufferPen, _points);
    }

    private IEnumerator CleanupRender()
    {
        yield return new WaitForSecondsRealtime(_cleanupDelay);
        _renderer.Clear();
        _cleanupRenderer = null;
    }
}
