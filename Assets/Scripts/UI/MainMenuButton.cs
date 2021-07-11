using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class MainMenuButton : MonoBehaviour, IPointerClickHandler
{
    private const string MainMenuScene = "MainMenu";

    [SerializeField] private GameObjectToggle _menuToggle;

    [SerializeField] private PlayableDirector _exitCutscene;

    private int _actionsCount;

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        if (_exitCutscene != null)
        {
            _actionsCount++;
            _exitCutscene.stopped += ExitSceneStopped;
            _exitCutscene.Play();
        }

        _actionsCount++;
        _menuToggle.Hide(onAnimationEnded: ChangeSceneOrReduce);
    }

    private void ExitSceneStopped(PlayableDirector exitScene)
    {
        exitScene.stopped -= ExitSceneStopped;
        ChangeSceneOrReduce();
    }

    private void ChangeSceneOrReduce()
    {
        _actionsCount--;
        if (_actionsCount == 0)
        {
            SceneManager.LoadScene(MainMenuScene);
        }
    }
}
