using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class GridMap : MonoBehaviour
{
    [SerializeField] LayerMask unwalkableMask;
    [SerializeField] Vector2 gridWorldSize;
    [SerializeField] float nodeRadius;
    Node[,] grid;
    Vector2 gridWorldPosition;

    public List<Node> path;

    float nodeDiameter;
    int gridSizeX, gridSizeY;
    private void Awake()
    {
        gridWorldPosition = new Vector2(transform.position.x, transform.position.y);
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }
    private void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector2 worldBottomLeft = gridWorldPosition + Vector2.left * gridWorldSize.x / 2 + Vector2.down * gridWorldSize.y / 2;
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector2 worldPoint = worldBottomLeft + Vector2.right * (x * nodeDiameter + nodeRadius) + Vector2.up * (y * nodeDiameter + nodeRadius);
                bool walkable = !Physics2D.OverlapCircle(worldPoint, nodeRadius, unwalkableMask);
                grid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }
    }
    /*
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 0));
        if (grid != null)
        {
            foreach (Node n in grid)
            {
                if (n.walkable)
                {
                    Gizmos.color = Color.white;
                }
                else
                {
                    Gizmos.color = Color.red;
                }
                if (path != null)
                {
                    if (path.Contains(n))
                    {
                        Gizmos.color = Color.black;
                    }
                }
                Gizmos.DrawCube(n.worldPosition, Vector2.one * (nodeDiameter - 0.1f));
            }
        }
    }
    */
    public Node GetNodeFromWorldPosition(Vector2 worldPosition)
    {
        Node nearestNode = grid[0, 0];
        foreach (Node n in grid)
        {
            if (Vector2.Distance(worldPosition, n.worldPosition) < Vector2.Distance(worldPosition, nearestNode.worldPosition))
            {
                nearestNode = n;
            }
        }
        return nearestNode;
    }
    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                {
                    continue;
                }
                int checkX = node.gridX + x;
                int checkY = node.gridY + y;
                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }
        return neighbours;
    }
    public bool LineOfSight(Node n1, Node n2)
    {
        return false;
        /*
        float x0 = n1.gridX, y0 = n1.gridY;
        float x1 = n2.gridX, y1 = n2.gridY;
        float dx = x1 - x0;
        float dy = y1 - y0;
        if(dx == 0 && dy == 0)
        {
            return grid[Mathf.FloorToInt(x0), Mathf.FloorToInt(y0)].walkable;
        }
        if(dx == 0 || dy == 0)
        {
            return true;
        }
        int xRay = (dx > 0) ? 1 : -1, xSign = (dx > 0) ? 1 : -1;
        int yRay = (dy > 0) ? 1 : -1, ySign = (dy > 0) ? 1 : -1;
        x0 = x0 + xRay * 1 / 2;
        y0 = y0 + yRay * 1 / 2;
        float m = dy / dx;
        float sx, sy;
        while (grid[Mathf.FloorToInt(x0 + xRay), Mathf.FloorToInt(y0 + yRay)] != n2)
        {
            if (grid[Mathf.FloorToInt(x0 + xRay - 1), Mathf.FloorToInt(y0 + yRay - 1)].walkable == false)
            {
                return false;
            }
            sx = Mathf.Sqrt(Mathf.Pow(xRay, 2) + Mathf.Pow(m * xRay, 2));
            sy = Mathf.Sqrt(Mathf.Pow(yRay, 2) + Mathf.Pow(yRay / m, 2));
            if(sx > sy)
            {
                yRay += ySign;
            }
            else if(sy > sx)
            {
                xRay += xSign;
            }
            else
            {
                xRay += xSign;
                yRay += ySign;
            }
        }
        return true;
        */
    }
}
