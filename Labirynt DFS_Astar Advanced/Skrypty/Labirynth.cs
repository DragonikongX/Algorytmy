using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Labirynth
{
    private int xSize = 0, ySize = 0;
    private int xStart = 0, yStart = 0;
    private int xEnd = 0, yEnd = 0;
    private Plane[,] labirynth = null;
    private List<Plane> walls = null;
    private List<Plane> passages = null;

    public int XStart { get => xStart; set => xStart = value; }
    public int YStart { get => yStart; set => yStart = value; }
    public int XEnd { get => xEnd; set => xEnd = value; }
    public int YEnd { get => yEnd; set => yEnd = value; }

    public Labirynth(int xSize, int ySize, int xStart, int yStart, int xEnd, int yEnd)
    {
        this.xStart = xStart;
        this.yStart = yStart;
        this.xEnd = xEnd;
        this.yEnd = yEnd;
        this.xSize = xSize;
        this.ySize = ySize;

        this.labirynth = new Plane[this.xSize, this.ySize];
        for (int indexX = 0; indexX < this.xSize; indexX++)
        {
            for (int indexY = 0; indexY < this.ySize; indexY++)
            {
                this.labirynth[indexX, indexY] = new Plane(indexX, indexY);
            }
        }
        walls = new List<Plane>();
        passages = new List<Plane>();
        PrimsLabirynthGenerator(this.xStart, this.yStart);
    }

    public Plane GetPlane(int indexX, int indexY)
    {
        return labirynth[indexX, indexY];
    }

    public void PrimsLabirynthGenerator(int xStart, int yStart)
    {
        for (int indexX = 0; indexX < this.xSize; indexX++)
        {
            for (int indexY = 0; indexY < this.ySize; indexY++)
            {
                this.labirynth[indexX, indexY].IsWalkable = false;
            }
        }
        labirynth[xStart, yStart].IsWalkable = true;
        this.passages.Add(labirynth[xStart, yStart]);
        walls.AddRange(GetNeighborWalls(xStart, yStart, false));
        while (walls.Count > 0)
        {
            int randomF = Random.Range(0, walls.Count);
            List<Plane> neighbors = GetNeighborWalls(walls[randomF].Xposition, walls[randomF].Yposition, true);
            int randomN = Random.Range(0, neighbors.Count);
            InBetween(walls[randomF].Xposition, walls[randomF].Yposition, neighbors[randomN].Xposition, neighbors[randomN].Yposition);
            walls[randomF].IsWalkable = true;
            this.passages.Add(walls[randomF]);
            walls.AddRange(GetNeighborWalls(walls[randomF].Xposition, walls[randomF].Yposition, false));
            walls.RemoveAt(randomF);
        }

    }

    public void LabirynthInitAStar(GameObject [,] labirynthObjects, MapGenerator.Materials materials)
    {
        this.ResetPlanes();
        AStarAlgorithm.InitPath(labirynth, this.xStart, this.yStart, this.xEnd, this.yEnd, labirynthObjects, materials);
    }

    public void LabirynthInitDFS(GameObject[,] labirynthObjects, MapGenerator.Materials materials)
    {
        this.ResetPlanes();
        DFSAlgorithm.InitPath(labirynth, this.xStart, this.yStart, this.xEnd, this.yEnd, labirynthObjects, materials);
    }

    public List<Plane> ReturnPassages()
    {
        return this.passages;
    }

    private void ResetPlanes()
    {
        foreach(Plane plane in this.labirynth)
        {
            plane.SetDefaultValues();
        }
    }

    private List<Plane> GetNeighborWalls(int xPlanePosition, int yPlanePosition, bool state)
    {

        List<Plane> neighbors = new List<Plane>();
        if (CheckSize(xPlanePosition + 2, yPlanePosition) &&
            labirynth[xPlanePosition + 2, yPlanePosition].IsWalkable == state &&
            labirynth[xPlanePosition + 2, yPlanePosition].Visited == false &&
            !walls.Contains(labirynth[xPlanePosition + 2, yPlanePosition]))
        {
            neighbors.Add(labirynth[xPlanePosition + 2, yPlanePosition]);
        }
        if (CheckSize(xPlanePosition - 2, yPlanePosition) &&
            labirynth[xPlanePosition - 2, yPlanePosition].IsWalkable == state &&
            labirynth[xPlanePosition - 2, yPlanePosition].Visited == false &&
            !walls.Contains(labirynth[xPlanePosition - 2, yPlanePosition]))
        {
            neighbors.Add(labirynth[xPlanePosition - 2, yPlanePosition]);
        }
        if (CheckSize(xPlanePosition, yPlanePosition + 2) &&
            labirynth[xPlanePosition, yPlanePosition + 2].IsWalkable == state &&
            labirynth[xPlanePosition, yPlanePosition + 2].Visited == false &&
            !walls.Contains(labirynth[xPlanePosition, yPlanePosition + 2]))
        {
            neighbors.Add(labirynth[xPlanePosition, yPlanePosition + 2]);
        }
        if (CheckSize(xPlanePosition, yPlanePosition - 2) &&
            labirynth[xPlanePosition, yPlanePosition - 2].IsWalkable == state &&
            labirynth[xPlanePosition, yPlanePosition - 2].Visited == false &&
            !walls.Contains(labirynth[xPlanePosition, yPlanePosition - 2]))
        {
            neighbors.Add(labirynth[xPlanePosition, yPlanePosition - 2]);
        }
        return neighbors;
    }

    private bool CheckSize(int xPlanePosition, int yPlanePosition)
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

    private void InBetween(int xStartPosition, int yStartPosition, int xEndPosition, int yEndPosition)
    {
        if (xStartPosition == xEndPosition)
        {
            if (yStartPosition > yEndPosition)
            {
                labirynth[xStartPosition, yStartPosition - 1].IsWalkable = true;
                this.passages.Add(labirynth[xStartPosition, yStartPosition - 1]);
            }
            else
            {
                labirynth[xStartPosition, yStartPosition + 1].IsWalkable = true;
                this.passages.Add(labirynth[xStartPosition, yStartPosition + 1]);
            }
        }
        else
        {
            if (xStartPosition > xEndPosition)
            {
                labirynth[xStartPosition - 1, yStartPosition].IsWalkable = true;
                this.passages.Add(labirynth[xStartPosition - 1, yStartPosition]);
            }
            else
            {
                labirynth[xStartPosition + 1, yStartPosition].IsWalkable = true;
                this.passages.Add(labirynth[xStartPosition + 1, yStartPosition]);
            }
        }
    }

}