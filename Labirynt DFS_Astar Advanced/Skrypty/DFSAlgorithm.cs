using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public static class DFSAlgorithm
{
    private static int xSize;
    private static int ySize;

    private static Plane start, end;
    private static Stack<Plane> pathStack;

    private static Plane[,] labirynth;
    private static GameObject[,] labirynthObjects;
    private static MapGenerator.Materials materials;

    public static void InitPath(Plane[,] labirynthNew, int xStart, int yStart, int xEnd, int yEnd, GameObject[,] labirynthObjectsNew, MapGenerator.Materials materialsNew)
    {
        labirynth = labirynthNew;
        labirynthObjects = labirynthObjectsNew;
        materials = materialsNew;
        xSize = labirynth.GetLength(0); ySize = labirynth.GetLength(1);
        start = labirynth[xStart, yStart]; end = labirynth[xEnd, yEnd];
        pathStack = new Stack<Plane>();
        pathStack.Push(start);
    }

    public static List<Plane> NextStep()
    { 
        if(pathStack.Count > 0)
        {
            if (TimeController.TimePass()) { 
                Plane top = pathStack.Pop();
                top.Visited = true;
                List<Plane> neighbors = GetNeighbors(top, labirynth);
                foreach (Plane plane in neighbors)
                {
                    if (plane.IsWalkable && !plane.Visited)
                    {
                        plane.Parent = top;
                        if (plane == end)
                        {
                            return CalculatePath(plane);
                        }
                        else
                        {
                            labirynthObjects[plane.Xposition, plane.Yposition].GetComponent<Renderer>().material = materials.neighbour;
                            pathStack.Push(plane);
                        }
                    }
                }
                labirynthObjects[top.Xposition, top.Yposition].GetComponent<Renderer>().material = materials.closed;
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
