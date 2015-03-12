using UnityEngine;
using System.Collections;

public class PovNode : MonoBehaviour
{
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

    public void SetVisibility(bool visibility)
    {
        renderer.enabled = visibility;
    }
}
