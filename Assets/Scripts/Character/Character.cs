using UnityEngine;
using UnityEngine.Playables;

public class Character : MonoBehaviour
{
    [SerializeField] PlayableDirector _gameOverCutscene;

    private bool _isDied;

    public void Die()
    {
        if (_isDied)
        {
            return;
        }

        _isDied = true;
        _gameOverCutscene.Play();
    }
}
