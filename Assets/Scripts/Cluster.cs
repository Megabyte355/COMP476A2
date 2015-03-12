using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cluster : MonoBehaviour
{
    [SerializeField]
    List<TileNode> tileNodeList = new List<TileNode>();
    [SerializeField]
    List<PovNode> povNodeList = new List<PovNode>();

    public void AddNode(TileNode tn)
    {
        tileNodeList.Add (tn);
    }

    public void AddNode(PovNode pn)
    {
        povNodeList.Add (pn);
    }
}
