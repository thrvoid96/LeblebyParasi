using UnityEngine;

public class SwipeDrawer : MonoBehaviour
{
    private LineRenderer lineRenderer;

    private float zOffset = 10;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        SwipeDetector.OnSwipe += SwipeDetector_OnSwipe;
    }

    private void SwipeDetector_OnSwipe(SwipeData data)
    {
        Vector3[] positions = new Vector3[2];
        positions[0] = Camera.main.ScreenToWorldPoint(new Vector3(data.fingerDownPos.x, data.fingerDownPos.y, zOffset));
        positions[1] = Camera.main.ScreenToWorldPoint(new Vector3(data.fingerUpPos.x, data.fingerUpPos.y, zOffset));
        lineRenderer.positionCount = 2;
        lineRenderer.SetPositions(positions);
    }
}