using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioVolume : MonoBehaviour
{
    private const string VolumeParameter = "Volume";

    private const string MusicVolumeParameter = "MusicVolume";

    private const string SoundsVolumeParameter = "SoundsVolume";

    public const float Max = 20.0f;

    public const float Normal = 0.0f;

    public const float Min = -80.0f;

    public const string DefaultSnapshot = "Default";

    public const string MenuSnapshot = "Menu";

    [SerializeField] AudioMixer _mixer;

    private static AudioVolume _instance;

    public static float Common
    {
        get => PlayerPrefs.GetFloat(VolumeParameter, Normal);
        set => _instance.SetVolume(value);
    }

    public static float Music
    {
        get => PlayerPrefs.GetFloat(MusicVolumeParameter, Normal);
        set => _instance.SetMusicVolume(value);
    }

    public static float Sounds
    {
        get => PlayerPrefs.GetFloat(SoundsVolumeParameter, Normal);
        set => _instance.SetSoundsVolume(value);
    }

    private void Awake()
    {
        if (_instance != null)
        {
            throw new System.Exception($"{nameof(AudioVolume)} already exists");
        }

        _instance = this;
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        SetVolume(Common);
        SetMusicVolume(Music);
        SetSoundsVolume(Sounds);
    }

    private void OnDestroy()
    {
        _instance = null;
        SceneManager.activeSceneChanged -= OnActiveSceneChanged;
    }

    private void OnActiveSceneChanged(Scene previous, Scene current)
    {
        ResetSnapshot();
    }

    private void SetVolume(float value)
    {
        SetParameterValue(VolumeParameter, value);
    }

    private void SetMusicVolume(float value)
    {
        SetParameterValue(MusicVolumeParameter, value);
    }

    private void SetSoundsVolume(float value)
    {
        SetParameterValue(SoundsVolumeParameter, value);
    }

    private void SetParameterValue(string name, float value)
    {
        value = Mathf.Clamp(value, Min, Max);
        _mixer.SetFloat(name, value);
        PlayerPrefs.SetFloat(name, value);
    }

    public static void ResetSnapshot()
    {
        SetSnapshot(DefaultSnapshot);
    }

    public static void SetSnapshot(string name)
    {
        AudioMixer mixer = _instance._mixer;
        mixer.FindSnapshot(name).TransitionTo(0.0f);
    }
}
