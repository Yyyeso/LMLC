using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PizzaJoystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    [SerializeField] private RectTransform frame;
    [SerializeField] private RectTransform handle;
    [SerializeField] private Image imgHandle;
    [SerializeField] private float handleRange = 70;
    Vector3 input;
    public float Horizontal { get { return input.x; } }
    public float Vertical { get { return input.y; } }


    public void OnDrag(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(frame, eventData.position, eventData.pressEventCamera, out Vector2 localVector))
        {
            handle.anchoredPosition = (localVector.magnitude < handleRange) ? localVector : localVector.normalized * handleRange;
            input = localVector;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        handle.DOKill();
        imgHandle.DOKill();
        SetColor(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        input = Vector2.zero;
        handle.DOAnchorPos(input, 0.1f);
        SetColor(false);
    }

    private void SetColor(bool isOnDraged)
    {
        Color color = (isOnDraged) ? Color.black : Color.white;
        imgHandle.DOColor(color, 0.1f);
    }
}