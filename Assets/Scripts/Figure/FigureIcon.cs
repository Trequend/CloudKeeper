using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FigureIcon : MonoBehaviour
{
    private const float AnimationTime = 0.5f;

    [SerializeField] private Image _image;

    private Figure _figure;

    public Figure Figure
    {
        get => _figure;
        set
        {
            _figure = value;
            if (_figure == null)
            {
                _image.enabled = false;
            }
            else
            {
                _image.enabled = true;
                _image.sprite = _figure.Sprite;
                _image.color = _figure.Color;
            }
        }
    }

    private RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        RectTransform parent = (RectTransform)_rectTransform.parent;
        Vector2 size = new Vector2(parent.sizeDelta.y, parent.sizeDelta.y);
        StartCoroutine(ResizeAnimation(
            AnimationTime,
            startSize: Vector2.zero,
            endSize: size
        ));
    }

    public void DestroyWithAnimation()
    {
        StopAllCoroutines();
        Coroutine resize = StartCoroutine(
            ResizeAnimation(
                AnimationTime,
                startSize: _rectTransform.sizeDelta,
                endSize: Vector2.zero
            )
        );

        StartCoroutine(
            Wait(resize, () => Destroy(gameObject))
        );
    }

    private IEnumerator Wait(Coroutine coroutine, Action callback)
    {
        yield return coroutine;
        callback();
    }

    private IEnumerator ResizeAnimation(float animationTime, Vector2 startSize, Vector2 endSize)
    {
        Vector2 Size(float time)
        {
            time /= animationTime;
            return startSize * (1.0f - time) + endSize * time;
        }

        float time = 0.0f;
        while (time < animationTime)
        {
            _rectTransform.sizeDelta = Size(time);
            yield return null;
            time += Time.deltaTime;
        }

        _rectTransform.sizeDelta = endSize;
    }
}
