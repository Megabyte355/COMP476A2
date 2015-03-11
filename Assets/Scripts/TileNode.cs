﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileNode : MonoBehaviour, System.IComparable<TileNode>
{
    [SerializeField]
    List<TileNode> neighbors = new List<TileNode>();
    public TileNode previous;

    public float costSoFar;
    public float heuristicValue;
    public float totalEstimatedValue;

    public void AddNeighbor(TileNode n)
    {
        if(!neighbors.Contains (n))
        {
            neighbors.Add (n);
        }
    }

    public int CompareTo(TileNode node)
    {
        int totalEstimateComparison = this.totalEstimatedValue.CompareTo(node.totalEstimatedValue);
        return (totalEstimateComparison != 0) ? totalEstimateComparison : this.heuristicValue.CompareTo(node.heuristicValue);
    }
}
