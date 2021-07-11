using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class FadeInOutToggle : GameObjectToggle
{
    [SerializeField] [Min(0.1f)] private float _animationTime = 0.1f;

    [SerializeField] private Vector2 _startScale = new Vector2(0.8f, 0.8f);

    [SerializeField] private Vector2 _endScale = new Vector2(1.0f, 1.0f);

    [SerializeField] [Range(0.0f, 1.0f)] private float _startAlpha = 0.0f;

    [SerializeField] [Range(0.0f, 1.0f)] private float _endAlpha = 1.0f;

    protected override void OnShow(Action onAnimationEnded)
    {
        StopAllCoroutines();
        gameObject.SetActive(true);
        StartCoroutine(
            Animation(
                _animationTime,
                isShow: true,
                onEnd: onAnimationEnded
            )
        );
    }

    protected override void OnHide(Action onAnimationEnded)
    {
        StopAllCoroutines();
        StartCoroutine(
            Animation(
                _animationTime,
                isShow: false,
                onEnd: onAnimationEnded
            )
        );
    }

    private IEnumerator Animation(float animationTime, bool isShow, Action onEnd)
    {
        Transform transform = GetComponent<Transform>();
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();

        canvasGroup.blocksRaycasts = false;

        Vector2 startScale = isShow ? _startScale : _endScale;
        Vector2 endScale = isShow ? _endScale : _startScale;

        float startAlpha = isShow ? _startAlpha : _endAlpha;
        float endAlpha = isShow ? _endAlpha : _startAlpha;

        Vector2 Scale(float time)
        {
            time /= animationTime;
            return startScale * (1.0f - time) + endScale * time;
        }

        float Alpha(float time)
        {
            time /= animationTime;
            return startAlpha * (1.0f - time) + endAlpha * time;
        }

        float time = 0.0f;
        while (time < animationTime)
        {
            transform.localScale = Scale(time);
            canvasGroup.alpha = Alpha(time);
            yield return null;
            time += Time.unscaledDeltaTime;
        }

        transform.localScale = endScale;
        canvasGroup.alpha = endAlpha;

        if (!isShow)
        {
            gameObject.SetActive(false);
        }

        canvasGroup.blocksRaycasts = true;
        onEnd();
    }
}
