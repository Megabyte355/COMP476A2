using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ClusterManager : MonoBehaviour
{
    public List<Cluster> clusterCollection;
    void Start ()
    {
        int layoutLayer = GameObject.FindGameObjectWithTag("TileGraphGenerator").GetComponent<TileGraphGenerator>().layoutLayer;

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
                foreach(PovNode povNode1 in currentCluster.povNodeList)
                {
                    foreach(PovNode povNode2 in currentNeighbor.povNodeList)
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

                // TEST
                currentCluster.povExitNodeKeys.Add (currentNeighbor);
                currentCluster.povExitNodeValues.Add (currentMin);
            }
        }
    }

}
