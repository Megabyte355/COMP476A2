using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileGraphNavigator : MonoBehaviour
{
    enum PathAlgorithm {AStar, Dijkstra, Cluster}
    enum GraphMethod {Grid, PointOfView}

    PathAlgorithm selectedAlgorithm;
    GraphMethod selectedGraph;

    public List<TileNode> pathList = new List<TileNode>();
    public List<TileNode> openList = new List<TileNode>();
    public List<TileNode> closedList = new List<TileNode>();

    public TileNode startNode;
    public TileNode endNode;

    [SerializeField]
    TileGraphGenerator graphGenerator;
    List<TileNode> tileNodes;
    List<PovNode> povNodes;
    int nodeIndex = 0;

    [SerializeField]
    Npc npc;

    void Start ()
    {
        selectedAlgorithm = PathAlgorithm.AStar;
        selectedGraph = GraphMethod.Grid;
        tileNodes = graphGenerator.tileNodeList;
        povNodes = graphGenerator.povNodeList;
        ComputeStartNode ();
        endNode = startNode.GetNeighbors()[0];
        startNode.costSoFar = 0.0f;
        startNode.heuristicValue = (endNode.transform.position - startNode.transform.position).magnitude;
        ComputeNewPath ();
    }

    void Update ()
    {
        if(Input.GetKeyDown (KeyCode.A))
        {
            selectedAlgorithm++;
            if((int)selectedAlgorithm > 2)
            {
                selectedAlgorithm = 0;
            }
        }
        else if(Input.GetKeyDown (KeyCode.S))
        {
            selectedGraph++;
            if((int)selectedGraph > 1)
            {
                selectedGraph = 0;
            }

            // Reset
            endNode = null;
            pathList.Clear ();
            openList.Clear ();
            closedList.Clear ();
            ComputeStartNode ();
            npc.ResetTarget();

            // Adjust visibility
            switch(selectedGraph)
            {
            case GraphMethod.Grid:
                foreach(TileNode n in tileNodes)
                {
                    n.SetVisibility(true);
                }
                foreach(PovNode p in povNodes)
                {
                    p.SetVisibility(false);
                }
                break;
            case GraphMethod.PointOfView:
                foreach(TileNode n in tileNodes)
                {
                    n.SetVisibility(false);
                }
                foreach(PovNode p in povNodes)
                {
                    p.SetVisibility(true);
                }
                break;
            default:
                break;
            }
        }
        else if(Input.GetMouseButtonDown(0))
        {
            // If it's a cube, select new path

            if(selectedGraph == GraphMethod.Grid)
            {
                Ray mouseClickRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if(Physics.Raycast (mouseClickRay, out hit))
                {
                    TileNode targetNode = hit.collider.GetComponent<TileNode>();
                    if(targetNode != null && targetNode != startNode && targetNode != endNode)
                    {
                        endNode = targetNode;
                        nodeIndex = 1;
                        ComputeNewPath ();
                        if(pathList.Count != 0)
                        {
                            npc.SetTarget (pathList[nodeIndex].transform);
                        }
                    }
                }
            }
            else if (selectedGraph == GraphMethod.PointOfView)
            {
                // TODO
            }

        }

        if(pathList.Count != 0 && nodeIndex != pathList.Count - 1)
        {
            // There are more nodes to follow
            if((npc.transform.position - pathList[nodeIndex].transform.position).magnitude < graphGenerator.overlapSphereRadius)
            {
                nodeIndex++;
                npc.SetTarget (pathList[nodeIndex].transform);
            }
        }



        // Set color of nodes
        foreach(TileNode node in tileNodes)
        {
            node.ResetColor();
        }

        if(pathList.Count > 1) {
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

    void OnGUI() 
    {   
        GUI.Box(new Rect(10, 10, 275, 80), "Selected options");
        GUI.Box(new Rect(10, 30, 275, 30), "(press 'a' to change) Algorithm: " + selectedAlgorithm.ToString ());
        GUI.Box(new Rect(10, 60, 275, 30), "(press 's' to change) Graph: " + selectedGraph.ToString ());
        GUI.Box(new Rect(10, 100, 275, 80), "Legend");
        GUI.Box(new Rect(10, 120, 275, 30), "Start node -> Blue, End node -> Red");
        GUI.Box(new Rect(10, 150, 275, 30), "Visited node -> Yellow, Path node -> Green");   
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
        ComputeStartNode ();

        // Switch case for algorithms
        switch(selectedAlgorithm)
        {
        case PathAlgorithm.AStar:
            calculateAStar ();
            break;
        case PathAlgorithm.Cluster:
            calculateCluster();
            break;
        case PathAlgorithm.Dijkstra:
            calculateDijkstra ();
            break;
        default:
            break;
        }
    }

    void calculateAStar()
    {
        openList.Add (startNode);
        visitNodeAStar(startNode);

        while(openList.Count > 0 && openList[0] != endNode)
        {
            visitNodeAStar (openList[0]);
        }

        ComposePathList();
    }

    void calculateDijkstra()
    {
        openList.Add (startNode);
        visitNodeDijkstra(startNode);
        
        while(openList.Count > 0 && openList[0] != endNode)
        {
            visitNodeDijkstra(openList[0]);
        }
        
        ComposePathList();
    }

    void calculateCluster()
    {
        Cluster startCluster = startNode.GetCluster ();
        Cluster endCluster = endNode.GetCluster ();

        if(startCluster == endCluster)
        {
            // Simply use A* algorithm
            calculateAStar ();
        }
        else
        {
            List<TileNode> interClusterPath;
            if(startCluster.bestPathToCluster.TryGetValue(endCluster, out interClusterPath))
            {


                // Save a backup
                TileNode originalStart = startNode;
                TileNode originalEnd = endNode;

                pathList.Clear ();
                openList.Clear ();
                closedList.Clear ();

                List<TileNode> beginning = ComputePathAStarWithoutCleaning(originalStart, interClusterPath[0]);
                List<TileNode> beginOpenList = new List<TileNode>(openList);
                List<TileNode> beginClosedList = new List<TileNode>(closedList);

                pathList.Clear ();
                openList.Clear ();
                closedList.Clear ();

                List<TileNode> ending = ComputePathAStarWithoutCleaning(interClusterPath[interClusterPath.Count - 1], originalEnd);
                List<TileNode> endOpenList = new List<TileNode>(openList);
                List<TileNode> endClosedList = new List<TileNode>(closedList);

                pathList.Clear ();
                openList.Clear ();
                closedList.Clear ();

                // Compose pathList
                pathList.AddRange (beginning);
                pathList.AddRange (interClusterPath);
                pathList.AddRange (ending);

                openList.AddRange (beginOpenList);
                closedList.AddRange (beginClosedList);
                openList.AddRange (endOpenList);
                closedList.AddRange (endClosedList);

                // Restore backup
                startNode = originalStart;
                endNode = originalEnd;

                nodeIndex = 1;
                npc.ResetTarget();

            }
        }
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

    void visitNodeDijkstra(TileNode node)
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
                float heuristicValue = 0.0f;
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
        openList.Sort((TileNode x, TileNode y) => { return x.costSoFar.CompareTo (y.costSoFar); });
    }

    void ComposePathList()
    {
        pathList.Add (endNode);
        while(pathList.Count > 0 && pathList[pathList.Count - 1] != startNode)
        {
            TileNode previous = pathList[pathList.Count - 1].previous;
            if(previous == null)
            {
                pathList.Clear ();
                return;
            }
            pathList.Add (previous);
        }
        pathList.Reverse ();
    }

    void Reset()
    {
        endNode = null;
        pathList.Clear ();
        openList.Clear ();
        closedList.Clear ();
        ComputeStartNode ();
        npc.ResetTarget();

        endNode = startNode.GetNeighbors()[0];
        startNode.costSoFar = 0.0f;
        startNode.heuristicValue = (endNode.transform.position - startNode.transform.position).magnitude;
    }
    
    private void UpdateNodeValues(TileNode n, TileNode prev, float cost, float heuristic, float total)
    {
        n.previous = prev;
        n.costSoFar = cost;
        n.heuristicValue = heuristic;
        n.totalEstimatedValue = total;
    }

    public List<TileNode> ComputePathAStarWithoutCleaning(TileNode start, TileNode end)
    {
        startNode = start;
        endNode = end;

        pathList.Clear ();
        openList.Clear ();
        closedList.Clear ();
        calculateAStar ();

        return new List<TileNode>(pathList);
    }

    public List<TileNode> ComputePathAStar(TileNode start, TileNode end)
    {
        List<TileNode> result = ComputePathAStarWithoutCleaning(start, end);
        
        // Clean up
        Reset ();
        
        return result;
    }

    public float CalculateCost(TileNode start, TileNode end)
    {
        List<TileNode> path = ComputePathAStar (start, end);

        return CalculateCost (path);
    }

    public float CalculateCost(List<TileNode> path)
    {
        float cumulativeCost = 0.0f;
        for(int i = 0; i < path.Count - 1; i++)
        {
            cumulativeCost += (path[i].transform.position - path[i + 1].transform.position).magnitude;
        }
        
        return cumulativeCost;
    }
}
