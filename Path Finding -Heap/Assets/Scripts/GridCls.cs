using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCls : MonoBehaviour
{
    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    Node[,] grid;

    float nodeDiameter;
    int gridSizeX, gridSizeY;
    void Start()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }

    public int MaxSize 
    {
        get { return gridSizeX* gridSizeY; }
    }

    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

        for ( int x = 0; x < gridSizeX; x++ )
        {
            for ( int y = 0; y < gridSizeY; y++ )
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));
                grid[x, y] = new Node(walkable, worldPoint, x, y);

            }
        }
    }

    public List<Node> GetNeighboors(Node node)
    {
        List<Node> neighboors = new List<Node>();
        for ( int x = -1; x <= 1; x++ )
        {
            for ( int y = -1; y <= 1; y++ )
            {
                if ( x == 0 && y == 0 )
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if ( checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY )
                {
                    neighboors.Add(grid[checkX, checkY]);
                }
            }
        }
        //Debug.Log("Neighs count: " + neighboors.Count);
        return neighboors;
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x / gridWorldSize.x) + 0.5f;
        float percentY = (worldPosition.z / gridWorldSize.y) + 0.5f;
        //Debug.Log("Before percent: " + percentX);
        percentX = Mathf.Clamp01(percentX);
        //Debug.Log("After percent: " + percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        //Debug.Log("X: " + x);
        return grid[x, y];
    }

    public List<Node> path;
    private void OnDrawGizmos()
    {
        //Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

        if ( grid != null )

            foreach ( Node n in grid )
            {
                //Gizmos.color = (n.walkable ? Color.white : Color.red);
                if ( path != null )
                {
                    if ( path.Contains(n) )
                    {
                        Gizmos.color = Color.white;
                        Gizmos.DrawCube(n.worldPosition, new Vector3(1f, 0.5f, 1f) * (nodeDiameter - .1f));
                    }
                }
                
                
            }

    }
}


