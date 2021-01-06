using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class PathFindingCls : MonoBehaviour
{
    public Transform seeker, target;

    GridCls grid;

    void Awake()
    {
        grid = GetComponent<GridCls>();
    }

    private void Update()
    {
        if(Input.GetButtonDown("Jump"))
            FindPath(seeker.position, target.position);
    }

    void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
        HashSet<Node> closedSet = new HashSet<Node>();

        openSet.Add(startNode);

        while ( openSet.Count > 0 )
        {

            Node currentNode = openSet.RemoveFirst();
            
            closedSet.Add(currentNode);
            if ( currentNode == targetNode )
            {
                sw.Stop();
                print("Path found " + sw.ElapsedMilliseconds + "ms");
                RetracePath(startNode, targetNode);
                return;
            }

            foreach ( Node neighboor in grid.GetNeighboors(currentNode) )
            {
                //Debug.Log(!neighboor.walkable + "  " + closedSet.Contains(neighboor));
                if ( !neighboor.walkable || closedSet.Contains(neighboor) )
                {
                    continue;
                }
                
                int newMovementCostToNeighboor = currentNode.gCost + GetDistance(currentNode, neighboor);
                //Debug.Log("Cost: " + newMovementCostToNeighboor);
                if ( newMovementCostToNeighboor < neighboor.gCost || !openSet.Contains(neighboor) )
                {
                    neighboor.gCost = newMovementCostToNeighboor;
                    neighboor.hCost = GetDistance(neighboor, targetNode);
                    neighboor.parent = currentNode;

                    if ( !openSet.Contains(neighboor) )
                        openSet.Add(neighboor);
                }
            }
        }
        //Debug.Log("//////////////////////////////////////////////");
    }

    void RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while ( currentNode != startNode )
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;

        }
        path.Reverse();

        grid.path = path;
    }

    //çapraz maliyet 14*
    //yatay veya dikey maliyet 10*
    int GetDistance(Node nodeA, Node nodeB)
    {
        int distX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int distY = Mathf.Abs(nodeA.gridY - nodeB.gridY);
        //Debug.Log(" NodeA=NodeB X:" + nodeA.gridX + " " + nodeB.gridX);
        if ( distX > distY )
            return 14 * distY + 10 * (distX - distY);
        return 14 * distX + 10 * (distY - distX);
    }


}
