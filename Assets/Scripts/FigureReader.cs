using UnityEngine;
using UnityEngine.EventSystems;

public class FigureReader : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    private LineRenderer _renderer;

    private bool isReading = false;
    private Vector3 previousPosition = new Vector3(0, 0, -10);

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
#if !UNITY_EDITOR
        if (isReading)
        {
            return;
        }
#endif
        _renderer.positionCount = 0;
        isReading = true;
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
#if !UNITY_EDITOR
        if (Input.touches[0].phase != TouchPhase.Ended)
        {
            return;
        }
#endif
        isReading = false;
    }

    private void LateUpdate()
    {
        if (isReading)
        {

#if UNITY_EDITOR
            Vector3 inputPosition = Input.mousePosition;
#else
            Vector3 inputPosition = Input.touches[0].position;
#endif
            Vector3 position = Camera.main.ScreenToWorldPoint(inputPosition);
            position.z = 0;
            if (Vector3.Distance(previousPosition, position) > 0.1f)
            {
                int index = _renderer.positionCount;
                _renderer.positionCount++;
                _renderer.SetPosition(index, position);
                previousPosition = position;
            }
        }
    }
}
