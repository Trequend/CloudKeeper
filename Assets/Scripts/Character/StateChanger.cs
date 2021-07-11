using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class StateChanger : MonoBehaviour
{
    private const string AttackAnimation = "Attack1";

    private const string HitAnimation = "Hit";

    private const string FallingAnimation = "Falling";

    [SerializeField] private FigureReader _reader;

    [SerializeField] private AudioSource _attackSound;

    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    void Start()
    {
        _reader.FigureReaded += OnFigureReaded;
    }

    private void OnDestroy()
    {
        _reader.FigureReaded -= OnFigureReaded;
    }

    private void OnFigureReaded(Figure figure)
    {
        if (figure != null)
        {
            PlayAttack();
        }
    }

    public void PlayAttack()
    {
        _animator.Play(AttackAnimation);
        _attackSound.Play();
    }

    public void PlayHit()
    {
        _animator.Play(HitAnimation);
    }

    public void StartFalling()
    {
        _animator.Play(FallingAnimation);
        StartCoroutine(Fall());
    }

    private IEnumerator Fall()
    {
        Transform transform = GetComponent<Transform>();
        while (true)
        {
            transform.Translate(new Vector3(0, -9.8f * Time.deltaTime, 0.0f));
            yield return null;
        }
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
