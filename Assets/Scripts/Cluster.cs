using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cluster : MonoBehaviour
{
    public List<TileNode> tileNodeList = new List<TileNode>();
    public List<PovNode> povNodeList = new List<PovNode>();

    public List<Cluster> neighborClusters;
    public Dictionary<Cluster, PovNode> povExitNodes = new Dictionary<Cluster, PovNode>();


    // TEST
    public List<Cluster> povExitNodeKeys;
    public List<PovNode> povExitNodeValues;

    void Start()
    {
        int layoutLayer = GameObject.FindGameObjectWithTag("TileGraphGenerator").GetComponent<TileGraphGenerator>().layoutLayer;
        foreach(PovNode node in povNodeList)
        {
            foreach(PovNode potentialNeighbor in povNodeList)
            {
                if(node != potentialNeighbor && !Physics.Linecast(node.transform.position, potentialNeighbor.transform.position, 1 << layoutLayer))
                {
                    node.AddVisibleNeighbor(potentialNeighbor);
                }
            }
        }
    }

    public void Bind(TileNode tn)
    {
        tileNodeList.Add (tn);
        tn.SetCluster(this);
    }

    public void Bind(PovNode pn)
    {
        povNodeList.Add (pn);
        pn.SetCluster (this);
    }
}
