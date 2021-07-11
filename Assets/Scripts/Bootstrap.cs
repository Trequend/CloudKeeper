using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrap : MonoBehaviour
{
    private const string PathToSoundtrack = "Music/Soundtrack";

    private const string GameSceneName = "MainMenu";

    private AudioClip _soundtrack;

    private void Start()
    {
        Application.targetFrameRate = Screen.currentResolution.refreshRate;
        StartCoroutine(LoadGame());
    }

    private IEnumerator LoadGame()
    {
        yield return StartCoroutine(LoadSoundtrack());

        AsyncOperation load = SceneManager.LoadSceneAsync(GameSceneName, LoadSceneMode.Single);
        load.completed += OnLoadCompleted;
    }

    private IEnumerator LoadSoundtrack()
    {
        ResourceRequest load = Resources.LoadAsync<AudioClip>(PathToSoundtrack);
        yield return new WaitUntil(() => load.isDone);
        _soundtrack = (AudioClip)load.asset;
    }

    private void OnLoadCompleted(AsyncOperation load)
    {
        Music.Play(_soundtrack);
        load.completed -= OnLoadCompleted;
    }
}
