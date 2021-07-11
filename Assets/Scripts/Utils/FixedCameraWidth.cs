using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class FixedCameraWidth : MonoBehaviour
{
    [SerializeField] [Min(0.1f)] private float _halfWidth = 9.0f;

    private Camera _camera;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    private void Update()
    {
        _camera.orthographicSize = _halfWidth / _camera.aspect;
    }
}
