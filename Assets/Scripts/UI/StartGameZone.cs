using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class StartGameZone : MonoBehaviour, IPointerClickHandler
{
    private const string GameScene = "Game";

    [SerializeField] private PlayableDirector _exitCutscene;

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        _exitCutscene.stopped += OnExitCutsceneStopped;
        _exitCutscene.Play();
    }

    private void OnExitCutsceneStopped(PlayableDirector exitCutscene)
    {
        _exitCutscene.stopped -= OnExitCutsceneStopped;
        SceneManager.LoadScene(GameScene);
    }
}
