using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileGraphNavigator : MonoBehaviour
{
    enum NavigationMode {}

    public List<TileNode> pathList = new List<TileNode>();
    public List<TileNode> openList = new List<TileNode>();
    public List<TileNode> closedList = new List<TileNode>();

    public TileNode startNode;
    public TileNode endNode;

    [SerializeField]
    TileGraphGenerator graphGenerator;
    List<TileNode> tileNodes;

    void Start ()
    {
        // Reference to tileNodeList
        tileNodes = graphGenerator.tileNodeList;
    }

    void Update ()
    {
        if(Input.GetButtonDown ("Jump"))
        {
            // Change algorithms
        }
        else if(Input.GetMouseButtonDown(0))
        {
            // If it's a cube, select new path

            Ray mouseClickRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast (mouseClickRay, out hit))
            {
                TileNode targetNode = hit.collider.GetComponent<TileNode>();
                if(targetNode != null)
                {
                    endNode = targetNode;

                }
            }



        }



        // Set color of nodes
        //targetNode.renderer.material.color = Color.red;


    }

}
