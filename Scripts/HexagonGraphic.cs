using UnityEngine;

public class HexagonGraphic : MonoBehaviour, ICanvasRaycastFilter
{
    public float Radius;
    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform, screenPoint, eventCamera, out Vector2 pivotToCursorVector);

        Vector2 pivotOffsetRatio = rectTransform.pivot - new Vector2(0.5f, 0.5f);
        Vector2 pivotOffset = Vector2.Scale(rectTransform.rect.size, pivotOffsetRatio);
        Vector2 centerToCursorVector = pivotToCursorVector + pivotOffset;

        return (centerToCursorVector.magnitude < Radius);
    }
}
