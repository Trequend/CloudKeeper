using UnityEngine;
using UnityEngine.UI;

public class VolumeSliders : MonoBehaviour
{
    [SerializeField] private Slider _music;

    [SerializeField] private Slider _sounds;

    private void Start()
    {
        _music.minValue = _sounds.minValue = 0.0f;
        _music.maxValue = _sounds.maxValue = 1.0f;

        _music.onValueChanged.AddListener(SetMusicVolume);
        _sounds.onValueChanged.AddListener(SetSoundsVolume);

        _music.value = ConvertToLinear(AudioVolume.Music);
        _sounds.value = ConvertToLinear(AudioVolume.Sounds);
    }

    private void SetMusicVolume(float value)
    {
        AudioVolume.Music = ConvertToLogarithmic(value);
    }

    private void SetSoundsVolume(float value)
    {
        AudioVolume.Sounds = ConvertToLogarithmic(value);
    }

    private float ConvertToLinear(float value)
    {
        return Mathf.Pow(10.0f, value / AudioVolume.Max);
    }

    private float ConvertToLogarithmic(float value)
    {
        return value == 0.0f ? AudioVolume.Min : Mathf.Log10(value) * AudioVolume.Max;
    }
}
