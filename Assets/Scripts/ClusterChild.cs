using UnityEngine;
using System.Collections;

public class ClusterChild : MonoBehaviour
{
    [SerializeField]
    Cluster parentCluster;

    public void AddNode(TileNode tn)
    {
        parentCluster.AddNode(tn);
    }

    public void AddNode(PovNode pn)
    {
        parentCluster.AddNode(pn);
    }
}
