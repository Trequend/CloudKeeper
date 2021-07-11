using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Enemy : MonoBehaviour
{
    private const string DieAnimationName = "Die";

    private static readonly List<Enemy> s_enemies = new List<Enemy>();

    [SerializeField] private FiguresList _figuresList;

    private bool _isDied;

    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        s_enemies.Add(this);
        StartCoroutine(
            OnScreenEnter(() => _figuresList.Show())
        );
    }

    private void OnDestroy()
    {
        s_enemies.Remove(this);
    }

    public bool TakeDamage(Figure figure)
    {
        if (_figuresList.TryRemoveFigure(figure))
        {
            if (_figuresList.FiguresCount == 0)
            {
                Die();
            }

            return true;
        }

        return false;
    }

    public void Die()
    {
        if (_isDied)
        {
            return;
        }

        _isDied = true;
        // The animation contains an event that raises Destroy
        _animator.Play(DieAnimationName);
        _figuresList.RemoveFigures(_figuresList.FiguresCount);
        _figuresList.Hide();
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    public void Attack(Character character)
    {
        if (_isDied)
        {
            return;
        }

        Die();
        character.Die();
    }

    public void AddFigure(Figure figure)
    {
        _figuresList.AddFigure(figure);
    }

    private IEnumerator OnScreenEnter(Action callback)
    {
        Transform transform = GetComponent<Transform>();
        Camera camera = Camera.main;
        yield return new WaitUntil(() =>
        {
            Vector2 position = camera.WorldToScreenPoint(transform.position);
            return Screen.safeArea.Contains(position);
        });
        
        callback();
    }

    public static void ForeachEnemyInScreen(Action<Enemy> action)
    {
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        Camera camera = Camera.main;
        ForeachEnemy(enemy =>
        {
            Vector2 position = camera.WorldToScreenPoint(enemy.transform.position);
            if (Screen.safeArea.Contains(position))
            {
                action(enemy);
            }
        });
    }

    public static void ForeachEnemy(Action<Enemy> action)
    {
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        foreach (Enemy enemy in s_enemies)
        {
            if (!enemy._isDied)
            {
                action(enemy);
            }
        }
    }
}
