using UnityEngine;

public class DrawUILine : MonoBehaviour
{
    public RectTransform lineRectTransform;

    public void DrawLine(Vector2 start, Vector2 end, float mult)
    {
        // Calculate the distance and angle between start and end points
        float distance = Vector2.Distance(start, end);
        float angle = Mathf.Atan2(end.y - start.y, end.x - start.x) * Mathf.Rad2Deg;

        // Set the RectTransform properties to draw the line
        lineRectTransform.sizeDelta = new Vector2(distance / mult, lineRectTransform.sizeDelta.y);
        lineRectTransform.position = start;
        lineRectTransform.rotation = Quaternion.Euler(0, 0, angle);
    }
}