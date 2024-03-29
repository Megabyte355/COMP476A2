﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PovNode : MonoBehaviour, System.IComparable<PovNode>
{
    [SerializeField]
    Cluster cluster;
    [SerializeField]
    List<PovNode> visibleNeighborsInCluster = new List<PovNode>();
    [SerializeField]
    List<PovNode> neighbors = new List<PovNode>();
    public PovNode previous;

    [SerializeField]
    Color originalColor;

    public float costSoFar;
    public float heuristicValue;
    public float totalEstimatedValue;

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
                cluster.Bind(this);
            }
            else if(clusterChild != null)
            {
                clusterChild.Bind(this);
            }
        }
    }

    public void AddVisibleNeighborInCluster(PovNode node)
    {
        visibleNeighborsInCluster.Add (node);
    }

    public List<PovNode> GetVisibleNeighborsInCluster()
    {
        return visibleNeighborsInCluster;
    }

    public void AddNeighbor(PovNode node)
    {
        neighbors.Add (node);
    }
    
    public List<PovNode> GetNeighbors()
    {
        return neighbors;
    }

    public void SetVisibility(bool visibility)
    {
        renderer.enabled = visibility;
    }

    public void ResetColor()
    {
        transform.renderer.material.color = originalColor;
    }

    public void SetCluster(Cluster c)
    {
        cluster = c;
    }

    public int CompareTo(PovNode node)
    {
        int totalEstimateComparison = this.totalEstimatedValue.CompareTo(node.totalEstimatedValue);
        return (totalEstimateComparison != 0) ? totalEstimateComparison : this.heuristicValue.CompareTo(node.heuristicValue);
    }
}
