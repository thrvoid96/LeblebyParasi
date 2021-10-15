using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CoinLineRendererController))]
public class CoinLineRendererController : MonoBehaviour
{
    #region Singleton

    public static CoinLineRendererController instance;

    private void Awake()
    {
        instance = this;
    }

    #endregion

    private LineRenderer lineRenderer;
    private Vector3 center;

    public Vector3 centerPoint
    {
        get { return center; }
        set { center = value; } 
    }


    // Start is called before the first frame update
    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    public Vector3[] GetPositions()
    {
        Vector3[] positions = new Vector3[lineRenderer.positionCount];
        lineRenderer.GetPositions(positions);
        return positions;
    }

    public float GetWidth()
    {
        return lineRenderer.startWidth;
    }

    public int GetPositionCount()
    {
        return lineRenderer.positionCount;
    }

    public void SetPositions(Vector3[] positions)
    {
        lineRenderer.SetPositions(positions);
    }

    public void SetColor(Color color)
    {
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
    }

}
