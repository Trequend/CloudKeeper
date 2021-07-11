using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class RestartButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObjectToggle _menuToggle;

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        _menuToggle.Hide(onAnimationEnded: () =>
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        });
    }
}