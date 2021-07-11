using UnityEngine;

[RequireComponent(typeof(GameObjectToggle))]
public class Pause : MonoBehaviour
{
    [SerializeField] private GameObject _menuBackground;

    [SerializeField] private GameObject _menu;

    private CanvasGroup _menuBackgroundCanvasGroup;

    private CanvasGroup _menuCanvasGroup;

    private GameObjectToggle _toggle;

    private bool _isShown;

    private void Awake()
    {
        _toggle = GetComponent<GameObjectToggle>();
        _menuBackgroundCanvasGroup = _menuBackground.GetComponent<CanvasGroup>();
        _menuCanvasGroup = _menu.GetComponent<CanvasGroup>();
    }

#if !UNITY_ANDROID || UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_isShown)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }
    }
#endif

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            Show(immediately: true);
        }
    }

    public void Show()
    {
        Show(immediately: false);
    }

    public void ShowImmediately()
    {
        Show(immediately: true);
    }

    private void Show(bool immediately)
    {
        if (_isShown)
        {
            return;
        }

        _isShown = true;
        Time.timeScale = 0.0f;
        if (immediately)
        {
            _menuBackground.SetActive(true);
            _menuBackgroundCanvasGroup.alpha = 1.0f;
            _menu.SetActive(true);
            _menuCanvasGroup.alpha = 1.0f;
        }
        else
        {
            _toggle.Show();
        }
    }

    public void Hide()
    {
        if (!_isShown)
        {
            return;
        }

        _isShown = false;
        _toggle.Hide();
        Time.timeScale = 1.0f;
    }
}
