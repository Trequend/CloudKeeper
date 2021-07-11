using System;
using System.Collections;
using UnityEngine;

public class ParabolaMover : MonoBehaviour
{
    private Transform _transform;

    private Vector3 _targetPosition;

    private float _speed;

    private float _parameter;

    private float _side;

    private Action _callback;

    private void Awake()
    {
        _transform = GetComponent<Transform>();
    }

    public ParabolaMover SetSpeed(float speed)
    {
        _speed = speed;
        return this;
    }

    public ParabolaMover SetTargetPosition(Vector3 targetPosition)
    {
        Vector3 position = _transform.position;
        if (targetPosition.y == position.y)
        {
            position.y += float.Epsilon;
            _transform.position = position;
        }

        _targetPosition = targetPosition;
        _parameter = (position.x - targetPosition.x) / Mathf.Pow(position.y - targetPosition.y, 2.0f);
        _side = Mathf.Sign(position.y - targetPosition.y);
        return this;
    }

    public ParabolaMover SetCallback(Action callback)
    {
        _callback = callback;
        return this;
    }

    public void StartMoving()
    {
        StopAllCoroutines();
        StartCoroutine(Move());
    }

    private IEnumerator Move()
    {
        while (_targetPosition != _transform.position)
        {
            yield return new WaitForFixedUpdate();
            Vector3 position = _transform.position;
            position = Vector3.MoveTowards(position, _targetPosition, _speed * Time.fixedDeltaTime);
            position.y = _side * Mathf.Sqrt((position.x - _targetPosition.x) / _parameter) + _targetPosition.y;
            _transform.position = position;
        }

        _callback?.Invoke();
    }
}
