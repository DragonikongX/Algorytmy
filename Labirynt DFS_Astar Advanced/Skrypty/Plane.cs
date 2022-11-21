using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plane
{
    private int xPosition;
    private int yPosition;
    private bool isWalkable;
    private bool visited;
    private Plane parent;

    private int gCost;
    private int hCost;
    private int fCost;

    public Plane() { }
    public Plane(int indexX, int indexY)
    {
        this.xPosition = indexX;
        this.yPosition = indexY;
        this.isWalkable = true;
        this.SetDefaultValues();
    }

    public bool IsWalkable { get => isWalkable; set => isWalkable = value; }
    public bool Visited { get => visited; set => visited = value; }
    public int Xposition { get => xPosition; }
    public int Yposition { get => yPosition; }
    public int GCost { get => gCost; set => gCost = value; }
    public int HCost { get => hCost; set => hCost = value; }
    public int FCost { get => fCost; set => fCost = value; }
    public Plane Parent { get => parent; set => parent = value; }

    public void CalculateFCost()
    {
        this.fCost = this.gCost + this.hCost;
    }

    public void SetDefaultValues()
    {
        this.gCost = int.MaxValue;
        this.hCost = int.MaxValue;
        this.CalculateFCost();
        this.parent = null;
        this.visited = false;
    }

}