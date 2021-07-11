using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkipActionZone : MonoBehaviour, IPointerClickHandler
{
    private Action _onSkip;

    public void Activate(Action onSkip)
    {
        gameObject.SetActive(true);
        _onSkip = onSkip;
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
        _onSkip = null;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _onSkip?.Invoke();
        Deactivate();
    }
}
