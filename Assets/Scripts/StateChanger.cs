using UnityEngine;

[RequireComponent(typeof(Animator))]
public class StateChanger : MonoBehaviour
{
    private Animator _animator;

    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            _animator.Play("Attack1");
        }
    }
}
