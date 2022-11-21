using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MapGenerator : MonoBehaviour
{
    [System.Serializable]
    public struct Materials
    {
        public Material wall;
        public Material pasage;
        public Material current;
        public Material end;
        public Material success;
        public Material closed;
        public Material neighbour;
    }
    [SerializeField]
    private Vector2Int mapSize;
    [SerializeField]
    private GameObject plane;
    [SerializeField]
    private Materials materials;
    [SerializeField]
    private Text timer;

    private Vector2Int startPosition;
    private Vector2Int endPosition;
    private Vector3 planeBounds;
    private Labirynth labirynth = null;
    private GameObject[,] labirynthObjects = null;
    private List<Plane> aStarPath = null;
    private List<Plane> DFSPath = null;

    private static int draw = 0;
    private static float time = 0.0f;

    public Vector2Int MapSize { get => mapSize; set => mapSize = value; }

    private void Awake()
    {
        this.aStarPath = new List<Plane>();
        this.DFSPath = new List<Plane>();
        this.planeBounds = this.plane.GetComponent<BoxCollider>().size;
        this.labirynthObjects = new GameObject[mapSize.x, mapSize.y];
    }

    private void Start()
    {
        TimeController.TimerStart = true;
        CreateMap();
        TimeController.TimerStart = false;
        TimeController.ResetTimer();
    }

    private void Update()
    {
        if (draw == 1)
        {
            this.labirynthObjects[this.startPosition.x, this.startPosition.y].GetComponent<Renderer>().material = materials.end;
            timer.text = TimeController.Timer();
            if (TimeController.TimePass())
            {
                List<Plane> newPath = null;
                if ((newPath = DFSAlgorithm.NextStep()) != null)
                {
                    draw = 0;
                    if (newPath.Count > 0)
                    {
                        this.DFSPath = newPath;
                        for (int index = 1; index < this.DFSPath.Count - 1; index++)
                        {
                            this.labirynthObjects[this.DFSPath[index].Xposition, this.DFSPath[index].Yposition].GetComponent<Renderer>().material = this.materials.success;
                        }
                        TimeController.TimerStart = false;
                        TimeController.ResetTimer();
                    }
                }
            }
            this.labirynthObjects[this.endPosition.x, this.endPosition.y].GetComponent<Renderer>().material = materials.end;
        }
        else if(draw == 2)
        {
            this.labirynthObjects[this.startPosition.x, this.startPosition.y].GetComponent<Renderer>().material = materials.end;
            timer.text = TimeController.Timer();
            if (TimeController.TimePass())
            {
                List<Plane> newPath = null;
                if ((newPath = AStarAlgorithm.NextStep()) != null)
                {
                    draw = 0;
                    if (newPath.Count > 0)
                    {
                        this.aStarPath = newPath;
                        for (int index = 1; index < this.aStarPath.Count - 1; index++)
                        {
                            this.labirynthObjects[this.aStarPath[index].Xposition, this.aStarPath[index].Yposition].GetComponent<Renderer>().material = this.materials.success;
                        }
                    }
                    TimeController.TimerStart = false;
                    TimeController.ResetTimer();
                }
            }
            this.labirynthObjects[this.endPosition.x, this.endPosition.y].GetComponent<Renderer>().material = materials.end;
        }
    }

    public void SetStartEnd(int xStart, int yStart, int xEnd, int yEnd)
    {
        StopDrawing();
        if (xStart != xEnd || yStart != yEnd)
        {
            if ((xStart >= 0 && xStart < this.mapSize.x) && (yStart >= 0 && yStart < this.mapSize.y))
            {
                if (this.labirynth.GetPlane(xStart, yStart).IsWalkable)
                {
                    this.labirynthObjects[this.startPosition.x, this.startPosition.y].GetComponent<Renderer>().material = materials.pasage;
                    this.startPosition = new Vector2Int(xStart, yStart);
                    this.labirynth.XStart = this.startPosition.x;
                    this.labirynth.YStart = this.startPosition.y;
                    this.labirynthObjects[this.startPosition.x, this.startPosition.y].GetComponent<Renderer>().material = materials.end;
                }
            }
            if ((xEnd >= 0 && xEnd < this.mapSize.x) && (yEnd >= 0 && yEnd < this.mapSize.y))
            {
                if (this.labirynth.GetPlane(xEnd, yEnd).IsWalkable)
                {
                    this.labirynthObjects[this.endPosition.x, this.endPosition.y].GetComponent<Renderer>().material = materials.pasage;
                    this.endPosition = new Vector2Int(xEnd, yEnd);
                    this.labirynth.XEnd = this.endPosition.x;
                    this.labirynth.YEnd = this.endPosition.y;
                    this.labirynthObjects[this.endPosition.x, this.endPosition.y].GetComponent<Renderer>().material = materials.end;
                }
            }
        }
    }

    public void RestartMap()
    {
        StopDrawing();
        DeleteMap();
        TimeController.TimerStart = true;
        CreateMap();
        TimeController.TimerStart = false;
        TimeController.ResetTimer();
    }

    public void DrawPath(int flag)
    {
        ClearMap();
        TimeController.ResetTimer();
        TimeController.TimerStart = true;
        if (flag == 1)
        {
            this.labirynth.LabirynthInitDFS(this.labirynthObjects, this.materials);
            draw = 1;
        }
        else if (flag == 2)
        {
            this.labirynth.LabirynthInitAStar(this.labirynthObjects, this.materials);
            draw = 2;
        }
    }

    private void StopDrawing()
    {
        ClearMap();
        draw = 0;
    }

    private void ClearMap()
    {
        foreach(Plane plane in labirynth.ReturnPassages())
        {
            this.labirynthObjects[plane.Xposition, plane.Yposition].GetComponent<Renderer>().material = materials.pasage;
        }
        this.aStarPath = new List<Plane>();
        this.DFSPath = new List<Plane>();
    }

    private void CreateMap()
    {
        this.startPosition = new Vector2Int(0, 0);
        this.endPosition = new Vector2Int(mapSize.x - 1, mapSize.y - 1);
        this.labirynth = new Labirynth(mapSize.x, mapSize.y, startPosition.x, startPosition.y, endPosition.x, endPosition.y);
        for (int indexX = 0; indexX < mapSize.x; indexX++)
        {
            for (int indexY = 0; indexY < mapSize.y; indexY++)
            {
                timer.text = TimeController.Timer();
                this.labirynthObjects[indexX,indexY] = Instantiate(
                    plane,
                    new Vector3(indexX * this.planeBounds.x, 0, indexY * this.planeBounds.z),
                    Quaternion.identity,
                    this.transform
                    );
                this.labirynthObjects[indexX, indexY].name = "Plane " + indexX + " " + indexY;
                if (labirynth.GetPlane(indexX, indexY).IsWalkable) {
                    this.labirynthObjects[indexX, indexY].GetComponent<Renderer>().material = materials.pasage;
                }
                else
                {
                    this.labirynthObjects[indexX, indexY].GetComponent<Renderer>().material = materials.wall;
                    this.labirynthObjects[indexX, indexY].transform.position += new Vector3(0, 1, 0);
                }
            }
        }
        Borders();
    }

    private void DeleteMap()
    {
        foreach (GameObject child in this.labirynthObjects)
        {
            Destroy(child);
        }
        foreach (Transform child in this.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void Borders()
    {
        for (int indexX = -1; indexX < mapSize.x + 1; indexX++)
        {
            for (int indexY = -1; indexY < mapSize.y + 1; indexY++)
            {
                if (indexX == -1 || indexX == mapSize.x || indexY == -1 || indexY == mapSize.y)
                {
                    GameObject tmp = Instantiate(
                    plane,
                    new Vector3(indexX * this.planeBounds.x, 1, indexY * this.planeBounds.z),
                    Quaternion.identity,
                    this.transform
                    );
                    tmp.name = "Border";
                    tmp.GetComponent<Renderer>().material = materials.wall;
                }
            }
        }
    }
    
}
