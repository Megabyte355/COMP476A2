using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileGraphNavigator : MonoBehaviour
{
    enum NavigationMode {}

    public List<TileNode> pathList = new List<TileNode>();
    public List<TileNode> openList = new List<TileNode>();
    public List<TileNode> closedList = new List<TileNode>();

    public TileNode startNode;
    public TileNode endNode;

    [SerializeField]
    TileGraphGenerator graphGenerator;
    List<TileNode> tileNodes;
    int nodeIndex = 0;

    [SerializeField]
    Npc npc;

    void Start ()
    {
        tileNodes = graphGenerator.tileNodeList;
        ComputeStartNode ();
        endNode = startNode.GetNeighbors()[0];
        startNode.costSoFar = 0.0f;
        startNode.heuristicValue = (endNode.transform.position - startNode.transform.position).magnitude;
        ComputeNewPath ();
    }

    void Update ()
    {
        if(Input.GetButtonDown ("Jump"))
        {
            // Change algorithms

//            startNode.ResetColor();
//            Debug.Log("Reset color");
        }
        else if(Input.GetMouseButtonDown(0))
        {
            // If it's a cube, select new path

            Ray mouseClickRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast (mouseClickRay, out hit))
            {
                TileNode targetNode = hit.collider.GetComponent<TileNode>();
                if(targetNode != null)
                {
                    endNode = targetNode;
                    nodeIndex = 1;
                    ComputeNewPath ();
                }
            }

        }

        if(nodeIndex != pathList.Count - 1)
        {
            // There are more nodes to follow

            //npc.SetTarget (endNode.transform);

        }
//        else if(pathList[pathList.Count - 1] != endNode)
//        {
//            // End node has changed - The path needs to be recomputed
//            nodeIndex = 1;
//            ComputeNewPath();
//        }
        else
        {
            // End node reaached
        }



        // Set color of nodes
        //targetNode.renderer.material.color = Color.red;
//        startNode.renderer.material.SetColor (

        foreach(TileNode node in tileNodes) {
//            node.TurnVisible ();
            node.ResetColor();
        }
        foreach (TileNode node in openList)
        {
            node.renderer.material.color = Color.yellow;
        }
        foreach (TileNode node in closedList)
        {
            node.renderer.material.color = Color.yellow;
        }
        foreach (TileNode node in pathList)
        {
            node.renderer.material.color = Color.green;
        }
        if(startNode != null)
        {
            startNode.renderer.material.color = Color.blue;
        }
        if(endNode != null) 
        {
            endNode.renderer.material.color = Color.red;
        }
    }

    void ComputeStartNode()
    {
        TileNode potentialStart = tileNodes[0];
        float minDistance = (potentialStart.transform.position - npc.transform.position).magnitude;
        foreach(TileNode n in tileNodes)
        {
            float currentDistance = (n.transform.position - npc.transform.position).magnitude;
            if(currentDistance < minDistance)
            {
                potentialStart = n;
                minDistance = currentDistance;
            }
        }

        startNode = potentialStart;
    }

    void ComputeNewPath()
    {
        pathList.Clear ();
        openList.Clear ();
        closedList.Clear ();

        // TODO
        ComputeStartNode ();
        nodeIndex = 0;

        // Switch case for algorithms
        calculateAStar ();


    }


    void calculateAStar()
    {
        openList.Add (startNode);
        visitNodeAStar(startNode);

        // complete the path
        while(openList[0] != endNode)
        {
            visitNodeAStar (openList[0]);
        }

        pathList.Add (endNode);
        while(pathList[pathList.Count - 1] != startNode)
        {
            pathList.Add (pathList[pathList.Count - 1].previous);
        }
        pathList.Reverse ();

    }

    void visitNodeAStar(TileNode node)
    {
        closedList.Add (node);
        openList.Remove (node);

        List<TileNode> neighbors = node.GetNeighbors();
        
        foreach(TileNode currentNeighbor in neighbors)
        {
            if(!Physics.Linecast (node.transform.position, currentNeighbor.transform.position, 1 << graphGenerator.layoutLayer))
            {
                // Neighbor stats
                float distance = (currentNeighbor.transform.position - node.transform.position).magnitude;
                float costSoFar = node.costSoFar + distance;
                float heuristicValue = (endNode.transform.position - currentNeighbor.transform.position).magnitude;
                float totalEstimatedValue = costSoFar + heuristicValue;

                bool inClosedList = closedList.Contains(currentNeighbor);
                bool inOpenList = openList.Contains(currentNeighbor);
                bool betterHeuristicFound = totalEstimatedValue < currentNeighbor.totalEstimatedValue;

                if(inClosedList && betterHeuristicFound)
                {
                    UpdateNodeValues (currentNeighbor, node, costSoFar, heuristicValue, totalEstimatedValue);
                    closedList.Remove (currentNeighbor);
                    openList.Add (currentNeighbor);
                }
                else if (inOpenList && betterHeuristicFound)
                {
                    UpdateNodeValues (currentNeighbor, node, costSoFar, heuristicValue, totalEstimatedValue);
                }
                else if (!inClosedList && !inOpenList)
                {
                    UpdateNodeValues (currentNeighbor, node, costSoFar, heuristicValue, totalEstimatedValue);
                    openList.Add (currentNeighbor);
                }
            }
        }
        openList.Sort();
    }

    private void UpdateNodeValues(TileNode n, TileNode prev, float cost, float heuristic, float total)
    {
        n.previous = prev;
        n.costSoFar = cost;
        n.heuristicValue = heuristic;
        n.totalEstimatedValue = total;
    }


}
