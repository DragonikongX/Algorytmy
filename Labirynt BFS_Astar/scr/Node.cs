using System;
using System.Collections.Generic;
using UnityEngine;

public enum NodeType
{
    Open = 0,
    Blocked = 1,
}
public class Node : MonoBehaviour, IComparable<Node>
{
    [SerializeField] private float borderSize;
    [SerializeField] private GameObject tile;
    private NodeType nodeType = NodeType.Open;
    private Vector3 worldPosition;
    private float distanceTraveled = Mathf.Infinity;
    public float priority;

    private bool visited = false;
    private int xIndex;
    private int yIndex;

    private List<Node> neighbors;
    private Node previous = null;

    public NodeType NodeType { get { return nodeType; } set { nodeType = value; } }
    public Vector3 NodeWorldPos { get { return worldPosition; } set { worldPosition = value; } }
    public float DistanceTraveled { get { return distanceTraveled; } set { distanceTraveled = value; } }
    public int XIndex { get { return xIndex; } set { xIndex = value; } }
    public int YIndex { get { return yIndex; } set { yIndex = value; } }
    public List<Node> Neighbors { get { return neighbors; } set { neighbors = value; } }
    public Node Previous { get { return previous; } set { previous = value; } }

    public bool Visited { get => visited; set => visited = value; }

    public int CompareTo(Node other)
    {
        if (this.priority < other.priority)
            return 1;
        else if (this.priority > other.priority)
            return -1;
        else
            return 0;
    }
    public void ColorNode(Color color)
    {
        SpriteRenderer mySpriteRenderer = tile.GetComponent<SpriteRenderer>();
        mySpriteRenderer.color = color;
    }

    public void Init()
    {
        gameObject.name = "Node (" + xIndex + "," + yIndex + ")";
        gameObject.transform.position = worldPosition;
        tile.transform.localScale = new Vector3(tile.transform.localScale.x - borderSize, tile.transform.localScale.y - borderSize, tile.transform.localScale.z);
    }
}
