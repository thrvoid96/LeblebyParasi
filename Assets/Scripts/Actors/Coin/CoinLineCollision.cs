using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CoinLineRendererController), typeof(BoxCollider))]
public class CoinLineCollision : MonoBehaviour
{

    private BoxCollider boxCol;

    private void Start()
    {
        boxCol = GetComponent<BoxCollider>();
    }

    private void OnEnable()
    {
        EventManager.StartListening(Events.PositionsSet, PositionsSet);
    }


    private void OnDisable()
    {
        EventManager.StopListening(Events.PositionsSet, PositionsSet);
    }

    private void PositionsSet(EventParam param)
    {
         CoinLineRendererController.instance.SetPositions(param.linePositions);
         CalculateColliderPoints();
    }

    private void CalculateColliderPoints()
    {
        Vector3[] positions = CoinLineRendererController.instance.GetPositions();

        float width = CoinLineRendererController.instance.GetWidth();
        float distance = Vector3.Distance(positions[0], positions[1]);

        if (distance != 0)
        {
            boxCol.enabled = true;
            boxCol.size = new Vector3(width, 0.5f, Vector3.Distance(positions[0], positions[1]) - 1.1f);
        }
        else
        {
            boxCol.enabled = false;
            return;
        }
        

        var centerPoint = Vector3.Lerp(positions[0], positions[1], 0.5f);
        CoinLineRendererController.instance.centerPoint = centerPoint;

        var direction = positions[0] - positions[1];

        transform.position = centerPoint;
        transform.rotation = Quaternion.LookRotation(direction);       

    }
}