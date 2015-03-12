using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PovNode : MonoBehaviour
{
    [SerializeField]
    List<PovNode> visibleNeighbors = new List<PovNode>();

    void Awake ()
    {
        TileGraphGenerator generator = GameObject.FindGameObjectWithTag("TileGraphGenerator").GetComponent<TileGraphGenerator>();
        generator.AddPovNode(this);
        renderer.enabled = false;

        RaycastHit hit;
        if(Physics.Raycast(transform.position, -Vector3.up, out hit))
        {
            Cluster cluster = hit.collider.transform.GetComponent<Cluster>();
            ClusterChild clusterChild = hit.collider.transform.GetComponent<ClusterChild>();
            if(cluster != null)
            {
                cluster.AddNode(this);
            }
            else if(clusterChild != null)
            {
                clusterChild.AddNode(this);
            }
        }
    }

    public void AddVisibleNeighbor(PovNode node)
    {
        visibleNeighbors.Add (node);
    }

    public List<PovNode> GetVisibleNeighbors()
    {
        return visibleNeighbors;
    }

    public void SetVisibility(bool visibility)
    {
        renderer.enabled = visibility;
    }
}
