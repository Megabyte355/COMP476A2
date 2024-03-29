﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileGraphGenerator : MonoBehaviour
{
    public Vector3 levelSize;
    public int tileDensity; 
    private Vector3 tileSize;
    public float overlapSphereRadius;
    public int layoutLayer;

    public List<TileNode> tileNodeList = new List<TileNode>();
    public List<PovNode> povNodeList = new List<PovNode>();

    [SerializeField]
    TileNode tileNodePrefab;
    GameObject gridNodes;
    GameObject povNodes;

    void Awake ()
    {
        gridNodes = GameObject.FindGameObjectWithTag("GridNodes");
        povNodes = GameObject.FindGameObjectWithTag("PovNodes");
        tileSize = new Vector3(levelSize.x / tileDensity, 0, levelSize.z / tileDensity);
        overlapSphereRadius = tileSize.magnitude / 2;
        GenerateTiles();
        AssignTileNodeNeighbors();
    }

    void Start ()
    {
        AssignPovNodeNeighbors();
    }

    void GenerateTiles()
    {
        for(int col = 0; col < tileDensity; col++)
        {
            for(int row = 0; row < tileDensity; row++)
            {
                float x = (row * tileSize.x) + (-levelSize.x / 2 + tileSize.x / 2);
                float y = 0.75f;
                float z = (-col * tileSize.z) + (levelSize.z / 2 - tileSize.z / 2);

                Vector3 currentLocation = new Vector3(x, y, z);

                if(Physics.OverlapSphere(currentLocation, overlapSphereRadius, 1 << layoutLayer).Length == 0)
                {
                    TileNode n = Instantiate (tileNodePrefab, currentLocation, Quaternion.identity) as TileNode;
                    n.transform.parent = gridNodes.transform;
                    tileNodeList.Add (n);
                }
            }
        }
    }

    void AssignTileNodeNeighbors()
    {
        foreach(TileNode currentNode in tileNodeList) {
            
            TileNode top = tileNodeList.Find (n => (n.transform.position - currentNode.transform.position) == (new Vector3(0, 0, tileSize.z)));
            TileNode bottom = tileNodeList.Find (n => (n.transform.position - currentNode.transform.position) == (new Vector3(0, 0, -tileSize.z)));
            
            TileNode left = tileNodeList.Find (n => (n.transform.position - currentNode.transform.position) == (new Vector3(-tileSize.x, 0, 0)));
            TileNode right = tileNodeList.Find (n => (n.transform.position - currentNode.transform.position) == (new Vector3(tileSize.x, 0, 0)));
            
            TileNode topLeft = tileNodeList.Find (n => (n.transform.position - currentNode.transform.position) == (new Vector3(-tileSize.x, 0, tileSize.z)));
            TileNode topRight = tileNodeList.Find (n => (n.transform.position - currentNode.transform.position) == (new Vector3(tileSize.x, 0, tileSize.z)));
            
            TileNode bottomLeft = tileNodeList.Find (n => (n.transform.position - currentNode.transform.position) == (new Vector3(-tileSize.x, 0, -tileSize.z)));
            TileNode bottomRight = tileNodeList.Find (n => (n.transform.position - currentNode.transform.position) == (new Vector3(tileSize.x, 0, -tileSize.z)));
            
            if(top != null) { currentNode.AddNeighbor(top); }
            if(bottom != null) { currentNode.AddNeighbor(bottom); }
            if(left != null) { currentNode.AddNeighbor(left); }
            if(right != null) { currentNode.AddNeighbor(right); }
            if(topLeft != null) { currentNode.AddNeighbor(topLeft); }
            if(topRight != null) { currentNode.AddNeighbor(topRight); }
            if(bottomLeft != null) { currentNode.AddNeighbor(bottomLeft); }
            if(bottomRight != null) { currentNode.AddNeighbor(bottomRight); }
        }
    }

    void AssignPovNodeNeighbors()
    {
        foreach(PovNode currentNode in povNodeList)
        {
            foreach(PovNode potentialNeighbor in povNodeList)
            {
                if(currentNode != potentialNeighbor && !Physics.Linecast(currentNode.transform.position, potentialNeighbor.transform.position, 1 << layoutLayer))
                {
                    currentNode.AddNeighbor(potentialNeighbor);
                }
            }
        }
    }

    public void AddPovNode(PovNode node)
    {
        povNodeList.Add(node);
    }
}
