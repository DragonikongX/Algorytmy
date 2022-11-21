using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonLogic : MonoBehaviour
{
    [SerializeField] private Maze maze;
    [SerializeField] private Pathfinder pathfinder;
    public Slider slider;
    private bool canSetWalls = false;
    private bool canSetStartAndGoal = false;
    private bool startPathfinder = false;
    float currentTime;
    float second;
    bool startTime = false;

    public bool CanSetWalls
    {
        get
        {
            return canSetWalls;
        }
    }
    public bool StartPathfinder
    {
        get
        {
            return startPathfinder;
        }
        set
        {
            startPathfinder = value;
        }
    }
    public void EnableAStar()
    {
        if (canSetWalls == false && canSetStartAndGoal == false)
        {
            ResetTime();
            StartTime();
            startPathfinder = !startPathfinder;
            pathfinder.pathfinderMode = Mode.AStar;
            maze.Clear();
            maze.StartPathfinder = startPathfinder;
        }
    }

    public void EnableBFS()
    {
        if (canSetWalls == false && canSetStartAndGoal == false)
        {
            ResetTime();
            StartTime();
            startPathfinder = !startPathfinder;
            pathfinder.pathfinderMode = Mode.BreadthFirstSearch;
            maze.Clear();
            maze.StartPathfinder = startPathfinder;
        }
    }

    public void SetPathGenerateSpeed()
    {
        pathfinder.SetSpeed(slider.value);
    }

    [SerializeField] Text timeText;

    public void StartTime()
    {
        startTime = true;
    }

    public void StopTime()
    {
        startTime = false;
    }

    public void ResetTime()
    {
        currentTime = 0.0f;
        second = 0.0f;
        timeText.text = "00:00";
    }

    public void RealTime()
    {
        currentTime += Time.realtimeSinceStartup;
       
        timeText.text = currentTime.ToString();
    }

    private void Update()
    {
        if (startTime)
        {
            currentTime += Time.deltaTime * 1000;
            if (currentTime > 1000)
            {
                second++;
                currentTime = 0.0f;
            }
            timeText.text = second.ToString() + ":" + (currentTime % 1000).ToString();
        }
    }

}
