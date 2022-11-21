using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public static class AStarAlgorithm
{
    private static int xSize;
    private static int ySize;

    private static List<Plane> openList = null;
    private static List<Plane> closedList = null;
    private static Plane start , end;

    private static Plane[,] labirynth = null;
    private static GameObject[,] labirynthObjects = null;
    private static MapGenerator.Materials materials;

    public static void InitPath(Plane[,] labirynthNew, int xStart, int yStart, int xEnd, int yEnd, GameObject[,] labirynthObjectsNew, MapGenerator.Materials materialsNew)
    {
        labirynth = labirynthNew; 
        labirynthObjects = labirynthObjectsNew;
        materials = materialsNew;
        xSize = labirynth.GetLength(0); ySize = labirynth.GetLength(1);
        start = labirynth[xStart, yStart]; end = labirynth[xEnd, yEnd];
        start.GCost = 0; start.HCost = CalculateCost(start,end); start.CalculateFCost();
        if (labirynth[xStart, yStart] != labirynth[xEnd, yEnd])
        {
            openList = new List<Plane> {start};
            closedList = new List<Plane>();
        }
    }

    public static List<Plane> NextStep()
    {
        if (openList.Count > 0)
        {
            if (TimeController.TimePass())
            {
                Plane newNode = LeastFCost(openList);
                if (newNode == end)
                {
                    return CalculatePath(end);
                }
                openList.Remove(newNode);
                closedList.Add(newNode);
                foreach (Plane neighbour in GetNeighbors(newNode, labirynth))
                {
                    if (closedList.Contains(neighbour))
                    {
                        continue;
                    }
                    if (!neighbour.IsWalkable)
                    {
                        closedList.Add(neighbour);
                        continue;
                    }
                    labirynthObjects[neighbour.Xposition, neighbour.Yposition].GetComponent<Renderer>().material = materials.neighbour;
                    int lowerCost = newNode.GCost + CalculateCost(newNode, neighbour);
                    if (lowerCost < neighbour.GCost)
                    {
                        neighbour.Parent = newNode;
                        neighbour.GCost = lowerCost;
                        neighbour.HCost = CalculateCost(neighbour, end);
                        neighbour.CalculateFCost();

                        if (!openList.Contains(neighbour))
                        {
                            openList.Add(neighbour);
                        }
                    }
                }
                labirynthObjects[newNode.Xposition, newNode.Yposition].GetComponent<Renderer>().material = materials.closed;
                TimeController.TimeZero();
            }
        }
        return null;
    }

    private static List<Plane> GetNeighbors(Plane plane, Plane[,] planeList)
    {
        List<Plane> neighbors = new List<Plane>();
        if (CheckSize(plane.Xposition + 1, plane.Yposition))
        {
            neighbors.Add(planeList[plane.Xposition + 1, plane.Yposition]);
        }
        if (CheckSize(plane.Xposition - 1, plane.Yposition))
        {
            neighbors.Add(planeList[plane.Xposition - 1, plane.Yposition]);
        }
        if (CheckSize(plane.Xposition, plane.Yposition + 1))
        {
            neighbors.Add(planeList[plane.Xposition, plane.Yposition + 1]);
        }
        if (CheckSize(plane.Xposition, plane.Yposition - 1))
        {
            neighbors.Add(planeList[plane.Xposition, plane.Yposition - 1]);
        }
        return neighbors;
    }

    private static int CalculateCost(Plane start, Plane end)
    {
        int xDistance = Mathf.Abs(start.Xposition - end.Xposition);
        int yDistance = Mathf.Abs(start.Yposition - end.Yposition);
        int answer = (xDistance + yDistance);
        return answer;
    }

    private static Plane LeastFCost(List<Plane> planeList)
    {
        Plane lowestFCostNode = planeList[0];
        for (int index = 1; index < planeList.Count; index++)
        {
            if (planeList[index].FCost < lowestFCostNode.FCost)
            {
                lowestFCostNode = planeList[index];
            }
            else if(planeList[index].FCost == lowestFCostNode.FCost && planeList[index].HCost == lowestFCostNode.HCost)
            {
                lowestFCostNode = planeList[index];
            }
        }
        return lowestFCostNode;
    }

    private static List<Plane> CalculatePath(Plane Node)
    {
        List<Plane> path = new List<Plane>();
        path.Add(Node);
        Plane currentNode = Node;
        while (currentNode.Parent != null)
        {
            path.Add(currentNode.Parent);
            currentNode = currentNode.Parent;
        }
        path.Reverse();
        return path;
    }

    private static bool CheckSize(int xPlanePosition, int yPlanePosition)
    {
        if (xPlanePosition >= 0 && xPlanePosition < xSize)
        {
            if (yPlanePosition >= 0 && yPlanePosition < ySize)
            {
                return true;
            }
            return false;
        }
        return false;
    }
}
