using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maze : MonoBehaviour
{
    [SerializeField] private Node nodePrefab;
    private Node[,] nodes;
    private List<Node> blockedNodes;
    private Stack<Node> nodeList;
    [SerializeField] private int column;
    [SerializeField] private int row;
    [SerializeField] private Pathfinder pathfinder;
    [SerializeField] private ButtonLogic buttonLogic;

    private int xStart = 0;
    private int yStart = 0;
    private int xGoal = 30;
    private int yGoal = 30;

    private Vector2[] directions =
    {
        new Vector2(0, 1f),
        new Vector2(0, -1f),
        new Vector2(1f, 0),
        new Vector2(-1f, 0),
        new Vector2(1f, 1f),
        new Vector2(1f, -1f),
        new Vector2(-1f, 1f),
        new Vector2(-1f, -1f)
    };

    private bool startPathfinder = false;

    public Node[,] GetNodes { get { return nodes; } }
    public int GetColumn { get { return column; } }
    public int GetRow { get { return row; } }
    public bool StartPathfinder { set { startPathfinder = value; } }

    public void CreateMaze()
    {
        nodeList = new Stack<Node>();
        nodeList.Push(nodes[xStart, yStart]);

        int index = 0;
        while (nodeList.Count > 0)
        {
            Node cell = nodeList.Pop();
            cell.NodeType = NodeType.Open;
            cell.ColorNode(Color.white);
            cell.Visited = true;
            List<Node> neighbors = GetNeighbors(cell);
            if (neighbors.Count > 0)
            {
                Node randIdx = neighbors[Random.Range(0, neighbors.Count)];
                foreach (Node neighbor in neighbors)
                {
                    Fill(cell, neighbor);
                    if (neighbor != randIdx)
                    {
                        nodeList.Push(neighbor);
                    }
                }
                nodeList.Push(randIdx);
            }
            index++;
            if (index > 10000) break;
        }
    }

    private void Fill(Node start, Node end)
    {
        if (start.XIndex == end.XIndex)
        {
            if (start.YIndex > end.YIndex)
            {
                nodes[start.XIndex, start.YIndex - 1].NodeType = NodeType.Open;
                nodes[start.XIndex, start.YIndex - 1].ColorNode(Color.white);
                nodes[start.XIndex, start.YIndex - 1].Visited = true;
            }
            else
            {
                nodes[start.XIndex, start.YIndex + 1].NodeType = NodeType.Open;
                nodes[start.XIndex, start.YIndex + 1].ColorNode(Color.white);
                nodes[start.XIndex, start.YIndex + 1].Visited = true;
            }
        }
        else
        {
            if (start.XIndex > end.XIndex)
            {
                nodes[start.XIndex - 1, start.YIndex].NodeType = NodeType.Open;
                nodes[start.XIndex - 1, start.YIndex].ColorNode(Color.white);
                nodes[start.XIndex - 1, start.YIndex].Visited = true;
            }
            else
            {
                nodes[start.XIndex + 1, start.YIndex].NodeType = NodeType.Open;
                nodes[start.XIndex + 1, start.YIndex].ColorNode(Color.white);
                nodes[start.XIndex + 1, start.YIndex].Visited = true;
            }
        }
    }
    private void SetPath()
    {
        blockedNodes = new List<Node>();

        for (int y = 0; y < column; y++)
        {
            for (int x = 0; x < row; x++)
            {
                List<Node> neighbors = new List<Node>();

                foreach (Vector2 direction in directions)
                {
                    int xNew = nodes[x, y].XIndex + (int)direction.x;
                    int yNew = nodes[x, y].YIndex + (int)direction.y;
                    bool withinRange = (yNew >= 0 && yNew < column && xNew >= 0 && xNew < row);

                    if (withinRange && nodes[xNew, yNew].NodeType == NodeType.Open)
                        neighbors.Add(nodes[xNew, yNew]);
                    else if (withinRange && nodes[xNew, yNew].NodeType == NodeType.Blocked)
                        blockedNodes.Add(nodes[xNew, yNew]);
                }

                nodes[x, y].Neighbors = neighbors;
            }
        }
    }
    public float GetNodeDistance(Node source, Node target)
    {
        int dx = Mathf.Abs(source.XIndex - target.XIndex);
        int dy = Mathf.Abs(source.YIndex - target.YIndex);

        int min = Mathf.Min(dx, dy);
        int max = Mathf.Max(dx, dy);

        int diagonalSteps = min;
        int straightSteps = max - min;

        return (1.4f * diagonalSteps + straightSteps);
    }
    private void Init()
    {
        buttonLogic.RealTime();
        Debug.Log(Time.realtimeSinceStartup);
        nodes = new Node[column, row];

        for (int y = 0; y < column; y++)
        {
            for (int x = 0; x < row; x++)
            {
                Node newNode = Instantiate(nodePrefab);

                newNode.NodeType = NodeType.Blocked;
                newNode.XIndex = x;
                newNode.YIndex = y;
                newNode.NodeWorldPos = new Vector3(x, y, 0);
                newNode.Visited = false;
                nodes[x, y] = newNode;

                newNode.Init();

                newNode.ColorNode(Color.black);
            }
        }
        CreateMaze();
        nodes[xStart, yStart].ColorNode(Color.green);
        nodes[xGoal, yGoal].ColorNode(Color.red);
    }

    public void Clear()
    {
        for (int y = 0; y < column; y++)
        {
            for (int x = 0; x < row; x++)
            {
                if(nodes[x, y].NodeType == NodeType.Open)
                {
                    nodes[x, y].ColorNode(Color.white);
                }
            }
        }
        nodes[xStart, yStart].ColorNode(Color.green);
        nodes[xGoal, yGoal].ColorNode(Color.red);
    }

    private List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();
        if (node.XIndex + 2 >= 0 && node.XIndex + 2 < column)
        {
            if (node.YIndex >= 0 && node.YIndex < row)
            {
                if (nodes[node.XIndex + 2, node.YIndex].Visited == false && !nodeList.Contains(nodes[node.XIndex + 2, node.YIndex]))
                {
                    nodes[node.XIndex + 2, node.YIndex].ColorNode(Color.grey);
                    neighbors.Add(nodes[node.XIndex + 2, node.YIndex]);
                }
            }
        }

        if (node.XIndex - 2 >= 0 && node.XIndex - 2 < column)
        {
            if (node.YIndex >= 0 && node.YIndex < row)
            {
                if (nodes[node.XIndex - 2, node.YIndex].Visited == false && !nodeList.Contains(nodes[node.XIndex - 2, node.YIndex]))
                {
                    nodes[node.XIndex - 2, node.YIndex].ColorNode(Color.green);
                    neighbors.Add(nodes[node.XIndex - 2, node.YIndex]);
                }
            }
        }
        if (node.XIndex >= 0 && node.XIndex < column)
        {
            if (node.YIndex + 2 >= 0 && node.YIndex + 2< row)
            {
                if (nodes[node.XIndex, node.YIndex + 2].Visited == false && !nodeList.Contains(nodes[node.XIndex, node.YIndex + 2]))
                {
                    nodes[node.XIndex, node.YIndex + 2].ColorNode(Color.magenta);
                    neighbors.Add(nodes[node.XIndex, node.YIndex + 2]);
                }
            }
        }
        if (node.XIndex >= 0 && node.XIndex < column)
        {
            if (node.YIndex - 2 >= 0 && node.YIndex - 2 < row)
            {
                if (nodes[node.XIndex, node.YIndex - 2].Visited == false && !nodeList.Contains(nodes[node.XIndex, node.YIndex - 2]))
                {
                    nodes[node.XIndex, node.YIndex - 2].ColorNode(Color.cyan);
                    neighbors.Add(nodes[node.XIndex, node.YIndex - 2]);
                }
            }
        }     
        return neighbors;
    }

    private void Start()
    {
        Init();
    }
    private void Update()
    {
        SetPath();
        if (startPathfinder == true)
        {
            if (pathfinder.pathfinderMode == Mode.BreadthFirstSearch)
                StartCoroutine(pathfinder.BFS(nodes[yStart, xStart], nodes[yGoal, xGoal]));
            else if (pathfinder.pathfinderMode == Mode.AStar)
                StartCoroutine(pathfinder.AStar(nodes[yStart, xStart], nodes[yGoal, xGoal]));
            startPathfinder = false;
        }
    }
}
