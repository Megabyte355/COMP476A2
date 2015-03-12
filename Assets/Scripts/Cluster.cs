using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cluster : MonoBehaviour
{
    [SerializeField]
    List<TileNode> tileNodeList = new List<TileNode>();
    [SerializeField]
    List<PovNode> povNodeList = new List<PovNode>();

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

    public void AddNode(TileNode tn)
    {
        tileNodeList.Add (tn);
    }

    public void AddNode(PovNode pn)
    {
        povNodeList.Add (pn);
    }
}
