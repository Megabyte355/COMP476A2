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

    public List<PovNode> povPathList = new List<PovNode>();
    public List<PovNode> openPovList = new List<PovNode>();
    public List<PovNode> closedPovList = new List<PovNode>();

    public TileNode startNode;
    public TileNode endNode;

    public PovNode startPovNode;
    public PovNode endPovNode;

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
            // Select new path
            Ray mouseClickRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast (mouseClickRay, out hit))
            {

                TileNode targetNode = hit.collider.GetComponent<TileNode>();
                PovNode targetPovNode = hit.collider.GetComponent<PovNode>();
                if(selectedGraph == GraphMethod.Grid && targetNode != null && targetNode != startNode && targetNode != endNode)
                {
                    endNode = targetNode;
                    nodeIndex = 1;
                    ComputeNewPath ();
                    if(pathList.Count != 0)
                    {
                        npc.SetTarget (pathList[nodeIndex].transform);
                    }
                }
                else if(selectedGraph == GraphMethod.PointOfView && targetPovNode != null && targetPovNode != startPovNode && targetPovNode != endPovNode) 
                {
                    endPovNode = targetPovNode;
                    nodeIndex = 1;
                    ComputeNewPath ();
                    if(povPathList.Count != 0)
                    {
                        npc.SetTarget (povPathList[nodeIndex].transform);
                    }
                }
            }

        }

        if(selectedGraph == GraphMethod.Grid && pathList.Count != 0 && nodeIndex != pathList.Count - 1)
        {
            // There are more nodes to follow
            if((npc.transform.position - pathList[nodeIndex].transform.position).magnitude < graphGenerator.overlapSphereRadius)
            {
                nodeIndex++;
                npc.SetTarget (pathList[nodeIndex].transform);
            }
        }
        else if(selectedGraph == GraphMethod.PointOfView && povPathList.Count != 0 && nodeIndex != povPathList.Count - 1)
        {
            if((npc.transform.position - povPathList[nodeIndex].transform.position).magnitude < graphGenerator.overlapSphereRadius)
            {
                nodeIndex++;
                npc.SetTarget (povPathList[nodeIndex].transform);
            }
        }

        // Set color of nodes
        foreach(TileNode node in tileNodes)
        {
            node.ResetColor();
        }

        foreach(PovNode node in povNodes)
        {
            node.ResetColor();
        }

        if(selectedGraph == GraphMethod.Grid)
        {
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
        else if (selectedGraph == GraphMethod.PointOfView)
        {
            if(povPathList.Count > 1) {
                foreach (PovNode node in openPovList)
                {
                    node.renderer.material.color = Color.yellow;
                }
                foreach (PovNode node in closedPovList)
                {
                    node.renderer.material.color = Color.yellow;
                }
                foreach (PovNode node in povPathList)
                {
                    node.renderer.material.color = Color.green;
                }
            }
            if(startPovNode != null)
            {
                startPovNode.renderer.material.color = Color.blue;
            }
            if(endPovNode != null) 
            {
                endPovNode.renderer.material.color = Color.red;
            }
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
        if(selectedGraph == GraphMethod.Grid)
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
        else if(selectedGraph == GraphMethod.PointOfView)
        {
            PovNode potentialStart = povNodes[0];
            float minDistance = (potentialStart.transform.position - npc.transform.position).magnitude;
            foreach(PovNode n in povNodes)
            {
                float currentDistance = (n.transform.position - npc.transform.position).magnitude;
                if(currentDistance < minDistance)
                {
                    potentialStart = n;
                    minDistance = currentDistance;
                }
            }
            startPovNode = potentialStart;
        }

    }

    void ComputeNewPath()
    {
        pathList.Clear ();
        povPathList.Clear ();

        openList.Clear ();
        openPovList.Clear ();
        closedList.Clear ();
        closedPovList.Clear ();
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
        if(selectedGraph == GraphMethod.Grid)
        {
            openList.Add (startNode);
            visitNodeAStar(startNode);
            
            while(openList.Count > 0 && openList[0] != endNode)
            {
                visitNodeAStar (openList[0]);
            }
            
            ComposePathList();
        }
        else if(selectedGraph == GraphMethod.PointOfView)
        {
            openPovList.Add (startPovNode);
            visitNodeAStar(startPovNode);

            while(openPovList.Count > 0 && openPovList[0] != endPovNode)
            {
                visitNodeAStar (openPovList[0]);
            }

            ComposePovPathList();
        }
    }

    void calculateDijkstra()
    {

        if(selectedGraph == GraphMethod.Grid)
        {
            openList.Add (startNode);
            visitNodeDijkstra(startNode);
            
            while(openList.Count > 0 && openList[0] != endNode)
            {
                visitNodeDijkstra(openList[0]);
            }
            
            ComposePathList();
        }
        else if(selectedGraph == GraphMethod.PointOfView)
        {
            openPovList.Add (startPovNode);
            visitNodeDijkstra(startPovNode);
            
            while(openPovList.Count > 0 && openPovList[0] != endPovNode)
            {
                visitNodeDijkstra(openPovList[0]);
            }
            
            ComposePovPathList();
        }

    }

    void calculateCluster()
    {
        if(selectedGraph == GraphMethod.PointOfView)
        {
            // Not supported
            return;
        }

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

    void visitNodeAStar(PovNode node)
    {
        closedPovList.Add (node);
        openPovList.Remove (node);
        
        List<PovNode> neighbors = node.GetNeighbors();

        foreach(PovNode currentNeighbor in neighbors)
        {
            if(!Physics.Linecast (node.transform.position, currentNeighbor.transform.position, 1 << graphGenerator.layoutLayer))
            {
                // Neighbor stats
                float distance = (currentNeighbor.transform.position - node.transform.position).magnitude;
                float costSoFar = node.costSoFar + distance;
                float heuristicValue = (endPovNode.transform.position - currentNeighbor.transform.position).magnitude;
                float totalEstimatedValue = costSoFar + heuristicValue;
                
                bool inClosedList = closedPovList.Contains(currentNeighbor);
                bool inOpenList = openPovList.Contains(currentNeighbor);
                bool betterHeuristicFound = totalEstimatedValue < currentNeighbor.totalEstimatedValue;
                
                if(inClosedList && betterHeuristicFound)
                {
                    UpdateNodeValues (currentNeighbor, node, costSoFar, heuristicValue, totalEstimatedValue);
                    closedPovList.Remove (currentNeighbor);
                    openPovList.Add (currentNeighbor);
                }
                else if (inOpenList && betterHeuristicFound)
                {
                    UpdateNodeValues (currentNeighbor, node, costSoFar, heuristicValue, totalEstimatedValue);
                }
                else if (!inClosedList && !inOpenList)
                {
                    UpdateNodeValues (currentNeighbor, node, costSoFar, heuristicValue, totalEstimatedValue);
                    openPovList.Add (currentNeighbor);
                }
            }
        }
        openPovList.Sort();
    }


    void visitNodeDijkstra(PovNode node)
    {
        closedPovList.Add (node);
        openPovList.Remove (node);
        
        List<PovNode> neighbors = node.GetNeighbors();
        
        foreach(PovNode currentNeighbor in neighbors)
        {
            if(!Physics.Linecast (node.transform.position, currentNeighbor.transform.position, 1 << graphGenerator.layoutLayer))
            {
                // Neighbor stats
                float distance = (currentNeighbor.transform.position - node.transform.position).magnitude;
                float costSoFar = node.costSoFar + distance;
                float heuristicValue = 0.0f;
                float totalEstimatedValue = costSoFar + heuristicValue;
                
                bool inClosedList = closedPovList.Contains(currentNeighbor);
                bool inOpenList = openPovList.Contains(currentNeighbor);
                bool betterHeuristicFound = totalEstimatedValue < currentNeighbor.totalEstimatedValue;
                
                if(inClosedList && betterHeuristicFound)
                {
                    UpdateNodeValues (currentNeighbor, node, costSoFar, heuristicValue, totalEstimatedValue);
                    closedPovList.Remove (currentNeighbor);
                    openPovList.Add (currentNeighbor);
                }
                else if (inOpenList && betterHeuristicFound)
                {
                    UpdateNodeValues (currentNeighbor, node, costSoFar, heuristicValue, totalEstimatedValue);
                }
                else if (!inClosedList && !inOpenList)
                {
                    UpdateNodeValues (currentNeighbor, node, costSoFar, heuristicValue, totalEstimatedValue);
                    openPovList.Add (currentNeighbor);
                }
            }
        }
        openPovList.Sort();
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

    void ComposePovPathList()
    {
        povPathList.Add (endPovNode);
        while(povPathList.Count > 0 && povPathList[povPathList.Count - 1] != startPovNode)
        {
            PovNode previous = povPathList[povPathList.Count - 1].previous;
            if(previous == null)
            {
                povPathList.Clear ();
                return;
            }
            povPathList.Add (previous);
        }
        povPathList.Reverse ();
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

    private void UpdateNodeValues(PovNode n, PovNode prev, float cost, float heuristic, float total)
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
