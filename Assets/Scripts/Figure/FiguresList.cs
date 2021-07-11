using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FiguresList : MonoBehaviour
{
    [SerializeField] private FigureIcon _iconPrefab;

    [SerializeField] [Min(0.01f)] private float _animationTime = 1.0f;

    [SerializeField] [Min(1.0f)] private int _maxFiguresCount = 3;

    private readonly List<FigureIcon> _figuresIcons = new List<FigureIcon>();

    public int FiguresCount => _figuresIcons.Count;

    private RectTransform _rectTransform;

    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        SetupTransform();
        SetupCanvasGroup();
    }

    private void SetupTransform()
    {
        _rectTransform = GetComponent<RectTransform>();
        int childCount = _rectTransform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Destroy(_rectTransform.GetChild(i).gameObject);
        }
    }

    private void SetupCanvasGroup()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        if (_canvasGroup == null)
        {
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        _canvasGroup.alpha = 0.0f;
    }

    public void AddFigure(Figure figure)
    {
        if (_figuresIcons.Count == _maxFiguresCount)
        {
            throw new System.OverflowException($"Max figures count - {_maxFiguresCount}");
        }

        FigureIcon icon = Instantiate(_iconPrefab, _rectTransform);
        icon.Figure = figure;
        _figuresIcons.Add(icon);
    }

    public bool TryRemoveFigure(Figure figure)
    {
        for (int i = 0; i < _figuresIcons.Count; i++)
        {
            FigureIcon icon = _figuresIcons[i];
            if (icon.Figure == figure)
            {
                _figuresIcons.RemoveAt(i);
                icon.DestroyWithAnimation();
                return true;
            }
        }

        return false;
    }

    public void RemoveFigures(int count)
    {
        if (count < 0)
        {
            count = 0; 
        }

        if (count > _figuresIcons.Count)
        {
            count = _figuresIcons.Count;
        }

        for (int i = 0; i < count; i++)
        {
            _figuresIcons.First().DestroyWithAnimation();
            _figuresIcons.RemoveAt(0);
        }
    }

    public void Show()
    {
        StopAllCoroutines();
        StartCoroutine(
            FadeAnimation(
                _animationTime,
                alphaStart: 0.0f,
                alphaEnd: 1.0f
            )
        );
    }

    public void Hide()
    {
        StopAllCoroutines();
        StartCoroutine(
            FadeAnimation(
                _animationTime,
                alphaStart: 1.0f,
                alphaEnd: 0.0f
            )
        );
    }

    private IEnumerator FadeAnimation(float animationTime, float alphaStart, float alphaEnd)
    {
        float Alpha(float time)
        {
            time /= animationTime;
            return alphaStart * (1.0f - time) + alphaEnd * time;
        }

        float time = 0.0f;
        while (time < animationTime)
        {
            _canvasGroup.alpha = Alpha(time);
            yield return null;
            time += Time.deltaTime;
        }

        _canvasGroup.alpha = alphaEnd;
    }
}
