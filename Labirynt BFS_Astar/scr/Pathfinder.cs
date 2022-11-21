using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Mode
{
    BreadthFirstSearch = 0,
    AStar = 1
}
public class Pathfinder : MonoBehaviour
{
    [HideInInspector]
    public Mode pathfinderMode = Mode.BreadthFirstSearch;
    [SerializeField] private Maze maze;
    [SerializeField] private ButtonLogic buttonLogic;
    private Queue<Node> toVisitNodesBFS;
    private PriorityQueue<Node> toVisitNodesAStar;
    private List<Node> visitedNodes;
    private List<Node> pathNodes;
    private float timeStep = 0.01f;

    public Color startColor = Color.green;
    public Color goalColor = Color.red;
    public Color toVisitColor = Color.magenta;
    public Color visitedColor = Color.gray;
    public Color pathColor = Color.cyan;

    public IEnumerator BFS(Node start, Node goal)
    {
        toVisitNodesBFS = new Queue<Node>();
        visitedNodes = new List<Node>();
        pathNodes = new List<Node>();

        toVisitNodesBFS.Enqueue(start);

        while (toVisitNodesBFS.Count > 0)
        {
            Node currentNode = toVisitNodesBFS.Dequeue();

                if (currentNode.name == goal.name)
                    break;

            if (!visitedNodes.Contains(currentNode))
            {
                visitedNodes.Add(currentNode);

                List<Node> neighbors = currentNode.Neighbors;
                foreach (Node neighbor in neighbors)
                {
                    if (!visitedNodes.Contains(neighbor) && !toVisitNodesBFS.Contains(neighbor))
                    {
                        neighbor.Previous = currentNode;
                        toVisitNodesBFS.Enqueue(neighbor);
                    }
                }

                ColorNodes(start, goal);
               yield return new WaitForSeconds(timeStep);
            }
        }

        GetPathNodes(goal);
        StartCoroutine(ColorPathNodes(start, goal));
    }
    public IEnumerator AStar(Node start, Node goal)
    {
        toVisitNodesAStar = new PriorityQueue<Node>();
        visitedNodes = new List<Node>();
        pathNodes = new List<Node>();

        toVisitNodesAStar.Enqueue(start);
        start.DistanceTraveled = 0;

        while (toVisitNodesAStar.Count > 0)
        {
            Node currentNode = toVisitNodesAStar.Dequeue();

                if (currentNode.name == goal.name)
                    break;

            if (!visitedNodes.Contains(currentNode))
            {
                visitedNodes.Add(currentNode);

                List<Node> neighbors = currentNode.Neighbors;
                foreach (Node neighbor in neighbors)
                {
                    if (!visitedNodes.Contains(neighbor))
                    {
                        float distanceToNeighbor = maze.GetNodeDistance(currentNode, neighbor);
                        float distanceTraveled = distanceToNeighbor + currentNode.DistanceTraveled;

                        if (float.IsPositiveInfinity(neighbor.DistanceTraveled) || distanceTraveled < neighbor.DistanceTraveled)
                        {
                            neighbor.Previous = currentNode;
                            neighbor.DistanceTraveled = distanceTraveled;
                        }

                        if (!toVisitNodesAStar.Contains(neighbor))
                        {
                            int distanceToGoal = (int)maze.GetNodeDistance(neighbor, goal);
                            neighbor.priority = neighbor.DistanceTraveled + distanceToGoal;
                            toVisitNodesAStar.Enqueue(neighbor);
                        }
                    }
                }
            }

            ColorNodes(start, goal);
            yield return new WaitForSeconds(timeStep);
        }

        GetPathNodes(goal);
        StartCoroutine(ColorPathNodes(start, goal));
    }

    public void SetSpeed(float newValue)
    {
        timeStep = newValue;
    }

    private void ColorNodes(Node start, Node goal)
    {
        if (pathfinderMode == Mode.BreadthFirstSearch)
        {
            foreach (Node node in toVisitNodesBFS)
                node.ColorNode(toVisitColor);
        }
        else if (pathfinderMode == Mode.AStar)
        {
            List<Node> frontierNodes = toVisitNodesAStar.ToList();
            foreach (Node node in frontierNodes)
                node.ColorNode(toVisitColor);
        }
        foreach (Node node in visitedNodes)
            node.ColorNode(visitedColor);

        start.ColorNode(startColor);
        goal.ColorNode(goalColor);
    }
    private IEnumerator ColorPathNodes(Node start, Node goal)
    {
        foreach (Node pathNode in pathNodes)
        {
            if (pathNode != start && pathNode != goal)
                pathNode.ColorNode(pathColor);
            yield return new WaitForSeconds(0.05f);
        }
        buttonLogic.StopTime();
    }
    public List<Node> GetPathNodes(Node endNode)
    {
        pathNodes.Add(endNode);

        Node currentNode = endNode.Previous;

        while (currentNode != null)
        {
            pathNodes.Insert(0, currentNode);
            currentNode = currentNode.Previous;
        }

        return pathNodes;
    }
}
