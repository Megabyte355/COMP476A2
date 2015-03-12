using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ClusterManager : MonoBehaviour
{
    public List<Cluster> clusterCollection;
    int layoutLayer;

    TileGraphNavigator tileGraphNavigator;
    void Start ()
    {
        layoutLayer = GameObject.FindGameObjectWithTag("TileGraphGenerator").GetComponent<TileGraphGenerator>().layoutLayer;
        tileGraphNavigator = GameObject.FindGameObjectWithTag("TileGraphNavigator").GetComponent<TileGraphNavigator>();

        ComputePovNodeBridges();
        ComputeTileNodeBridges();
        ComputeTileNodeClusterTable();
    }


    void ComputePovNodeBridges()
    {
        float minDistance;
        foreach(Cluster currentCluster in clusterCollection)
        {
            foreach(Cluster currentNeighbor in currentCluster.neighborClusters)
            {
                
                List<PovNode> nodesInCurrent = currentCluster.povNodeList;
                List<PovNode> nodesInNeighbor = currentNeighbor.povNodeList;
                
                PovNode currentMin = nodesInCurrent[0];
                //PovNode neighborMin = nodesInNeighbor[0];
                minDistance = (nodesInCurrent[0].transform.position - nodesInNeighbor[0].transform.position).magnitude;
                
                // Find the two nodes with the shortest connection
                foreach(PovNode povNode1 in nodesInCurrent)
                {
                    foreach(PovNode povNode2 in nodesInNeighbor)
                    {
                        if(!Physics.Linecast (povNode1.transform.position, povNode2.transform.position, 1 << layoutLayer))
                        {
                            float distance = (povNode1.transform.position - povNode2.transform.position).magnitude;
                            if(distance < minDistance)
                            {
                                minDistance = distance;
                                currentMin = povNode1;
                                //neighborMin = povNode2;
                            }
                        }
                    }
                }
                
                // Save exit nodes on the cluster itself
                currentCluster.povExitNodes.Add (currentNeighbor,currentMin);
                
                // TODO: TEST
                currentCluster.povExitNodeKeys.Add (currentNeighbor);
                currentCluster.povExitNodeValues.Add (currentMin);
            }
        }
    }

    void ComputeTileNodeBridges()
    {
        float minDistance;
        foreach(Cluster currentCluster in clusterCollection)
        {
            foreach(Cluster currentNeighbor in currentCluster.neighborClusters)
            {
                
                List<TileNode> nodesInCurrent = currentCluster.tileNodeList;
                List<TileNode> nodesInNeighbor = currentNeighbor.tileNodeList;
                
                TileNode currentMin = nodesInCurrent[0];
                minDistance = (nodesInCurrent[0].transform.position - nodesInNeighbor[0].transform.position).magnitude;
                
                // Find the two nodes with the shortest connection
                foreach(TileNode povNode1 in nodesInCurrent)
                {
                    foreach(TileNode povNode2 in nodesInNeighbor)
                    {
                        if(!Physics.Linecast (povNode1.transform.position, povNode2.transform.position, 1 << layoutLayer))
                        {
                            float distance = (povNode1.transform.position - povNode2.transform.position).magnitude;
                            if(distance < minDistance)
                            {
                                minDistance = distance;
                                currentMin = povNode1;
                            }
                        }
                    }
                }
                
                // Save exit nodes on the cluster itself
                currentCluster.tileExitNodes.Add (currentNeighbor,currentMin);
                
                // TODO: TEST
                currentCluster.tileExitNodeKeys.Add (currentNeighbor);
                currentCluster.tileExitNodeValues.Add (currentMin);
            }
        }
    }

    void ComputeTileNodeClusterTable()
    {
        List<Cluster> closedList = new List<Cluster>();
        List<Cluster> openList = new List<Cluster>();

        TileNode veryStart;
        TileNode veryEnd;

        foreach(Cluster currentCluster in clusterCollection)
        {
            foreach(Cluster otherCluster in clusterCollection)
            {

                if(currentCluster != otherCluster)
                {
                    //List<TileNode> currentExitNodes =  currentCluster.tileExitNodes.Values;
                    List<TileNode> currentExitNodes =  new List<TileNode>(currentCluster.tileExitNodes.Values);
                    List<TileNode> otherExitNodes =  new List<TileNode>(otherCluster.tileExitNodes.Values);
                    float minDistance = -1.0f;
                    List<TileNode> bestPath = null;
                    
                    foreach(TileNode currentNode in currentExitNodes)
                    {
                        foreach(TileNode otherNode in otherExitNodes)
                        {
                            List<TileNode> potentialPath = tileGraphNavigator.ComputePathAStar(currentNode, otherNode);
                            float distance = tileGraphNavigator.CalculateCost(potentialPath);
                            
                            // First computation or better path found
                            if(minDistance < 0 || distance < minDistance)
                            {
                                minDistance = distance;
                                bestPath = potentialPath;
                            }
                        }
                    }
                    
                    // Store best path in this cluster's "table"
                    currentCluster.bestPathToCluster.Add (otherCluster, bestPath);
//                    bestPathKeys.Add (otherCluster);
                }

            }
        }


//        foreach(Cluster currentCluster in clusterCollection)
//        {
//            closedList.Add(currentCluster);
//            foreach(Cluster neighborCluster in currentCluster.neighborClusters)
//            {
//
//            }
//        }






//        TileNode start = clusterCollection[3].tileExitNodeValues[0];
//        TileNode end = clusterCollection[3].tileExitNodeValues[1];
//        TileGraphNavigator nav = GameObject.FindGameObjectWithTag("TileGraphNavigator").GetComponent<TileGraphNavigator>();
//        float distance = nav.CalculateCost(start,end);
//        
//        Debug.Log("From Custer Manager: " + distance);
    }


    // TODO: TEST
//    public List<Cluster> bestPathKeys;
}
