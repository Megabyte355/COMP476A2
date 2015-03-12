using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ClusterManager : MonoBehaviour
{
    public List<Cluster> clusterCollection;
    int layoutLayer;
    void Start ()
    {
        layoutLayer = GameObject.FindGameObjectWithTag("TileGraphGenerator").GetComponent<TileGraphGenerator>().layoutLayer;

        ComputePovNodeBridges();
        ComputeTileNodeBridges();
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

}
