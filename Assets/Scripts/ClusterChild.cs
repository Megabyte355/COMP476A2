using UnityEngine;
using System.Collections;

public class ClusterChild : MonoBehaviour
{
    [SerializeField]
    Cluster parentCluster;

    public void Bind(TileNode tn)
    {
        parentCluster.Bind(tn);
    }

    public void Bind(PovNode pn)
    {
        parentCluster.Bind(pn);
    }
}
