using UnityEngine;

[RequireComponent(typeof(Animator))]
public class StateChanger : MonoBehaviour
{
    private Animator _animator;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            _animator.Play("Attack1");
        }
    }
}
