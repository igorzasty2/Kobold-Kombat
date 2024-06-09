using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicRay : MonoBehaviour
{
    [SerializeField] protected LayerMask wallLayerMask;
    protected LineRenderer lineRenderer;
    protected Vector2 direction;
    protected virtual void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
        lineRenderer.positionCount = 2;
    }
    public void SetLinePosition(Vector2 startPos, Vector2 endPos)
    {
        float distance = Vector2.Distance(startPos, endPos);
        direction = endPos - startPos;
        RaycastHit2D rayCastHit = Physics2D.Raycast(startPos, direction, distance, wallLayerMask);
        if (rayCastHit)
        {
            lineRenderer.SetPosition(0, startPos);
            lineRenderer.SetPosition(1, rayCastHit.point);
        }
        else
        {
            lineRenderer.SetPosition(0, startPos);
            lineRenderer.SetPosition(1, endPos);
        }
    }
    public void SetLineVisibility(bool isVisible)
    {
        lineRenderer.enabled = isVisible;
    }
    public Vector2 GetDirection()
    {
        return direction;
    }
}
