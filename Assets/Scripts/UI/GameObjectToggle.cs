using System;
using UnityEngine;
using UnityEngine.Events;

public class GameObjectToggle : MonoBehaviour
{
    [SerializeField] UnityEvent _onShow;

    [SerializeField] UnityEvent _showAnimationEnded;

    [SerializeField] UnityEvent _onHide;

    [SerializeField] UnityEvent _hideAnimationEnded;

    public void Show()
    {
        Show(null);
    }

    public void Show(Action onAnimationEnded)
    {
        _onShow?.Invoke();
        OnShow(() =>
        {
            _showAnimationEnded?.Invoke();
            onAnimationEnded?.Invoke();
        });
    }

    protected virtual void OnShow(Action onAnimationEnded)
    {
        onAnimationEnded();
    }

    public void Hide()
    {
        Hide(null);
    }

    public void Hide(Action onAnimationEnded)
    {
        _onHide?.Invoke();
        OnHide(() =>
        {
            _hideAnimationEnded?.Invoke();
            onAnimationEnded?.Invoke();
        });
    }

    protected virtual void OnHide(Action onAnimationEnded)
    {
        onAnimationEnded();
    }
}
