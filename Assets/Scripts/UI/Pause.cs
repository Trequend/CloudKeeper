using UnityEngine;

[RequireComponent(typeof(GameObjectToggle))]
public class Pause : MonoBehaviour
{
    private GameObjectToggle _toggle;

    private bool _isShown;

    private void Awake()
    {
        _toggle = GetComponent<GameObjectToggle>();
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
            Show();
        }
    }

    public void Show()
    {
        if (_isShown)
        {
            return;
        }

        _isShown = true;
        _toggle.Show();
        Time.timeScale = 0.0f;
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
