using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Music : MonoBehaviour
{
    private AudioSource _audioSource;

    private static Music _instance;

    private void Awake()
    {
        if (_instance != null)
        {
            throw new System.Exception($"{nameof(Music)} already exists");
        }

        _instance = this;
        _audioSource = GetComponent<AudioSource>();
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        _instance = null;
    }

    public static void Play(AudioClip clip)
    {
        _instance._audioSource.clip = clip;
        Play();
    }

    public static void Play()
    {
        _instance._audioSource.Play();
    }

    public static void Stop()
    {
        _instance._audioSource.Stop();
    }
}
