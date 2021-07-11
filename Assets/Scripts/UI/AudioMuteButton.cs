using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AudioMuteButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image _icon;

    [SerializeField] private Sprite _muted;

    [SerializeField] private Sprite _unmuted;

    private bool Muted
    {
        get => AudioVolume.Common == AudioVolume.Min;
        set => AudioVolume.Common = value ? AudioVolume.Min : AudioVolume.Normal;
    }

    private void Start()
    {
        UpdateIcon();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Muted = !Muted;
        UpdateIcon();
    }

    private void UpdateIcon()
    {
        _icon.sprite = Muted ? _muted : _unmuted;
    }
}
