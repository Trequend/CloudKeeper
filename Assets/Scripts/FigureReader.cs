using System.Collections;
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
        _renderer.positionCount = 0;
        isReading = true;
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        isReading = false;
    }

    private void LateUpdate()
    {
        if (isReading)
        {
            Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            position.z = 0;
            if (previousPosition != position)
            {
                int index = _renderer.positionCount;
                _renderer.positionCount++;
                _renderer.SetPosition(index, position);
                previousPosition = position;
            }
        }
    }
}
