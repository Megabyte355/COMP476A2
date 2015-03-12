using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileNode : MonoBehaviour, System.IComparable<TileNode>
{
    [SerializeField]
    Cluster cluster;
    [SerializeField]
    List<TileNode> neighbors = new List<TileNode>();
    public TileNode previous;

    [SerializeField]
    Color originalColor;

    public float costSoFar;
    public float heuristicValue;
    public float totalEstimatedValue;

    void Awake()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, -Vector3.up, out hit))
        {
            Cluster cluster = hit.collider.transform.GetComponent<Cluster>();
            ClusterChild clusterChild = hit.collider.transform.GetComponent<ClusterChild>();
            if(cluster != null)
            {
                cluster.Bind(this);
            }
            else if(clusterChild != null)
            {
                clusterChild.Bind(this);
            }
        }
    }

    public void AddNeighbor(TileNode n)
    {
        if(!neighbors.Contains (n))
        {
            neighbors.Add (n);
        }
    }

    public List<TileNode> GetNeighbors()
    {
        return neighbors;
    }

    public int CompareTo(TileNode node)
    {
        int totalEstimateComparison = this.totalEstimatedValue.CompareTo(node.totalEstimatedValue);
        return (totalEstimateComparison != 0) ? totalEstimateComparison : this.heuristicValue.CompareTo(node.heuristicValue);
    }

    public void ResetColor()
    {
        transform.renderer.material.color = originalColor;
    }

    public void SetVisibility(bool visibility)
    {
        renderer.enabled = visibility;
    }

    public Cluster GetCluster()
    {
        return cluster;
    }

    public void SetCluster(Cluster c)
    {
        cluster = c;
    }
}
